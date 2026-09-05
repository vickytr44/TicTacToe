import { computed, inject, InjectionToken } from '@angular/core';
import { signalStore, withState, withComputed, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap, catchError, of, timer } from 'rxjs';
import { GameApiService } from '../../../core/services/game-api.service';
import {
  GameMode,
  GameResponse,
  Player,
  ScoreboardResponse
} from '../../../core/models/game.models';

export const COMPUTER_THINKING_DELAY = new InjectionToken<number>('COMPUTER_THINKING_DELAY', {
  factory: () => 400
});

export interface GameState {
  game: GameResponse | null;
  scoreboard: ScoreboardResponse;
  isLoading: boolean;
  isPending: boolean;
  isComputerThinking: boolean;
  error: string | null;
}

const initialState: GameState = {
  game: null,
  scoreboard: { xWins: 0, oWins: 0, draws: 0 },
  isLoading: false,
  isPending: false,
  isComputerThinking: false,
  error: null
};

export const GameStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed(({ game, isPending }) => ({
    board: computed(() => game()?.board ?? [
      [null, null, null],
      [null, null, null],
      [null, null, null]
    ]),
    currentPlayer: computed<Player>(() => game()?.currentPlayer ?? 'X'),
    gameMode: computed<GameMode>(() => game()?.gameMode ?? 'TwoPlayer'),
    status: computed(() => game()?.status ?? 'InProgress'),
    winner: computed(() => game()?.winner ?? null),
    winningCells: computed(() => game()?.winningCells ?? []),
    moves: computed(() => game()?.moves ?? []),
    isGameOver: computed(() => {
      const s = game()?.status;
      return s === 'Won' || s === 'Draw';
    }),
    canUndo: computed(() => {
      const g = game();
      if (!g || isPending()) return false;
      return g.status === 'InProgress' && g.moves.length > 0;
    })
  })),
  withMethods((store, api = inject(GameApiService), delayMs = inject(COMPUTER_THINKING_DELAY)) => ({
    clearError() {
      patchState(store, { error: null });
    },
    setPending(isPending: boolean) {
      patchState(store, { isPending });
    },
    setComputerThinking(isComputerThinking: boolean) {
      patchState(store, { isComputerThinking });
    },
    updateGame(game: GameResponse) {
      patchState(store, { game, isLoading: false, isPending: false, error: null });
    },
    loadScoreboard: rxMethod<void>(
      pipe(
        switchMap(() =>
          api.getScoreboard().pipe(
            tap((scoreboard) => patchState(store, { scoreboard })),
            catchError(() => {
              patchState(store, { error: 'Failed to load scoreboard.' });
              return of(null);
            })
          )
        )
      )
    ),
    createGame: rxMethod<GameMode | undefined>(
      pipe(
        tap(() => patchState(store, { isLoading: true, error: null })),
        switchMap((mode = 'TwoPlayer') =>
          api.createGame({ mode }).pipe(
            tap((game) => patchState(store, { game, isLoading: false, isPending: false })),
            catchError(() => {
              patchState(store, {
                isLoading: false,
                isPending: false,
                error: 'Something went wrong. Please try again.'
              });
              return of(null);
            })
          )
        )
      )
    ),
    switchMode: rxMethod<GameMode>(
      pipe(
        tap(() => patchState(store, { isLoading: true, error: null })),
        switchMap((mode) =>
          api.createGame({ mode }).pipe(
            tap((game) => patchState(store, { game, isLoading: false, isPending: false })),
            catchError(() => {
              patchState(store, {
                isLoading: false,
                isPending: false,
                error: 'Something went wrong. Please try again.'
              });
              return of(null);
            })
          )
        )
      )
    ),
    makeMove: rxMethod<{ row: number; column: number }>(
      pipe(
        tap(() => patchState(store, { isPending: true, error: null })),
        switchMap(({ row, column }) => {
          const game = store.game();
          if (!game) {
            patchState(store, { isPending: false });
            return of(null);
          }

          const player = game.currentPlayer;
          return api.makeMove(game.id, { player, row, column }).pipe(
            switchMap((updatedGame) => {
              const computerMoved =
                updatedGame.gameMode === 'Computer' &&
                updatedGame.moves.length >= 2 &&
                updatedGame.moves[updatedGame.moves.length - 1].player === 'O';

              if (computerMoved) {
                const intermediateBoard = game.board.map((r) => [...r]);
                intermediateBoard[row - 1][column - 1] = 'X';

                const intermediateGame: GameResponse = {
                  ...updatedGame,
                  board: intermediateBoard,
                  currentPlayer: 'O',
                  status: 'InProgress',
                  winner: null,
                  winningCells: [],
                  moves: updatedGame.moves.slice(0, -1)
                };

                patchState(store, {
                  game: intermediateGame,
                  isPending: false,
                  isComputerThinking: true,
                  error: null
                });

                return timer(delayMs).pipe(
                  tap(() => {
                    patchState(store, {
                      game: updatedGame,
                      isComputerThinking: false,
                      error: null
                    });
                    if (updatedGame.status === 'Won' || updatedGame.status === 'Draw') {
                      api.getScoreboard().subscribe({
                        next: (sb) => patchState(store, { scoreboard: sb }),
                        error: () => {}
                      });
                    }
                  })
                );
              }

              patchState(store, {
                game: updatedGame,
                isPending: false,
                isComputerThinking: false,
                error: null
              });
              if (updatedGame.status === 'Won' || updatedGame.status === 'Draw') {
                api.getScoreboard().subscribe({
                  next: (sb) => patchState(store, { scoreboard: sb }),
                  error: () => {}
                });
              }
              return of(updatedGame);
            }),
            catchError((err) => {
              const errorMsg =
                err?.error?.detail || err?.error?.title || 'Something went wrong. Please try again.';
              patchState(store, { isPending: false, isComputerThinking: false, error: errorMsg });
              return of(null);
            })
          );
        })
      )
    ),
    resetGame: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { isPending: true, error: null })),
        switchMap(() => {
          const game = store.game();
          if (!game) {
            patchState(store, { isPending: false });
            return of(null);
          }
          return api.resetGame(game.id).pipe(
            tap((updatedGame) => {
              patchState(store, { game: updatedGame, isPending: false, error: null });
            }),
            catchError((err) => {
              const errorMsg =
                err?.error?.detail || err?.error?.title || 'Something went wrong. Please try again.';
              patchState(store, { isPending: false, error: errorMsg });
              return of(null);
            })
          );
        })
      )
    ),
    undoMove: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { isPending: true, error: null })),
        switchMap(() => {
          const game = store.game();
          if (!game) {
            patchState(store, { isPending: false });
            return of(null);
          }
          return api.undoMove(game.id).pipe(
            tap((updatedGame) => {
              patchState(store, { game: updatedGame, isPending: false, error: null });
            }),
            catchError((err) => {
              const errorMsg =
                err?.error?.detail || err?.error?.title || 'Something went wrong. Please try again.';
              patchState(store, { isPending: false, error: errorMsg });
              return of(null);
            })
          );
        })
      )
    ),
    resetScoreboard: rxMethod<void>(
      pipe(
        switchMap(() =>
          api.resetScoreboard().pipe(
            tap((scoreboard) => patchState(store, { scoreboard })),
            catchError(() => {
              patchState(store, { error: 'Failed to reset scoreboard.' });
              return of(null);
            })
          )
        )
      )
    )
  }))
);
