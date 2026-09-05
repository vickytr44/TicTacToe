import { computed, inject } from '@angular/core';
import { signalStore, withState, withComputed, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap, catchError, of } from 'rxjs';
import { GameApiService } from '../../../core/services/game-api.service';
import {
  GameMode,
  GameResponse,
  Player,
  ScoreboardResponse
} from '../../../core/models/game.models';

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
  withMethods((store, api = inject(GameApiService)) => ({
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
      patchState(store, { game, error: null });
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
            tap((game) => patchState(store, { game, isLoading: false })),
            catchError(() => {
              patchState(store, {
                isLoading: false,
                error: 'Failed to create game session. Please try again.'
              });
              return of(null);
            })
          )
        )
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
