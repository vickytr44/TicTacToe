import { TestBed } from '@angular/core/testing';
import { ScoreboardStore } from './scoreboard.store';
import { GameApiService } from '../services/game-api.service';
import { of, throwError } from 'rxjs';
import { ScoreboardResponse } from '../models/game.models';

describe('ScoreboardStore', () => {
  let store: InstanceType<typeof ScoreboardStore>;
  let apiSpy: {
    getScoreboard: ReturnType<typeof vi.fn>;
    resetScoreboard: ReturnType<typeof vi.fn>;
  };

  const mockScoreboard: ScoreboardResponse = {
    xWins: 3,
    oWins: 2,
    draws: 1
  };

  beforeEach(() => {
    apiSpy = {
      getScoreboard: vi.fn().mockReturnValue(of(mockScoreboard)),
      resetScoreboard: vi.fn().mockReturnValue(of({ xWins: 0, oWins: 0, draws: 0 }))
    };

    TestBed.configureTestingModule({
      providers: [
        ScoreboardStore,
        { provide: GameApiService, useValue: apiSpy }
      ]
    });

    store = TestBed.inject(ScoreboardStore);
  });

  it('should initialize with default zero counts', () => {
    expect(store.scoreboard()).toEqual({ xWins: 0, oWins: 0, draws: 0 });
    expect(store.isLoading()).toBe(false);
    expect(store.error()).toBeNull();
  });

  it('should load scoreboard and update state', () => {
    store.loadScoreboard();
    expect(apiSpy.getScoreboard).toHaveBeenCalled();
    expect(store.scoreboard()).toEqual(mockScoreboard);
    expect(store.isLoading()).toBe(false);
  });

  it('should handle loadScoreboard error', () => {
    apiSpy.getScoreboard.mockReturnValue(throwError(() => new Error('API error')));
    store.loadScoreboard();
    expect(store.error()).toBe('Failed to load scoreboard.');
  });

  it('should reset scoreboard and update state', () => {
    store.setScoreboard(mockScoreboard);
    expect(store.scoreboard().xWins).toBe(3);

    store.resetScoreboard();
    expect(apiSpy.resetScoreboard).toHaveBeenCalled();
    expect(store.scoreboard()).toEqual({ xWins: 0, oWins: 0, draws: 0 });
  });

  it('should handle resetScoreboard error', () => {
    apiSpy.resetScoreboard.mockReturnValue(throwError(() => new Error('Reset failed')));
    store.resetScoreboard();
    expect(store.error()).toBe('Failed to reset scoreboard.');
  });
});
