import { inject } from '@angular/core';
import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap, catchError, of } from 'rxjs';
import { GameApiService } from '../services/game-api.service';
import { ScoreboardResponse } from '../models/game.models';

export interface ScoreboardState {
  scoreboard: ScoreboardResponse;
  isLoading: boolean;
  error: string | null;
}

const initialScoreboardState: ScoreboardState = {
  scoreboard: { xWins: 0, oWins: 0, draws: 0 },
  isLoading: false,
  error: null
};

export const ScoreboardStore = signalStore(
  { providedIn: 'root' },
  withState(initialScoreboardState),
  withMethods((store, api = inject(GameApiService)) => ({
    clearError() {
      patchState(store, { error: null });
    },
    setScoreboard(scoreboard: ScoreboardResponse) {
      patchState(store, { scoreboard, error: null });
    },
    loadScoreboard: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { isLoading: true, error: null })),
        switchMap(() =>
          api.getScoreboard().pipe(
            tap((scoreboard) => patchState(store, { scoreboard, isLoading: false, error: null })),
            catchError(() => {
              patchState(store, {
                isLoading: false,
                error: 'Failed to load scoreboard.'
              });
              return of(null);
            })
          )
        )
      )
    ),
    resetScoreboard: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { isLoading: true, error: null })),
        switchMap(() =>
          api.resetScoreboard().pipe(
            tap((scoreboard) => patchState(store, { scoreboard, isLoading: false, error: null })),
            catchError(() => {
              patchState(store, {
                isLoading: false,
                error: 'Failed to reset scoreboard.'
              });
              return of(null);
            })
          )
        )
      )
    )
  }))
);
