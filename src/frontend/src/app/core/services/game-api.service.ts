import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  GameResponse,
  ScoreboardResponse,
  CreateGameRequest,
  MakeMoveRequest
} from '../models/game.models';

@Injectable({
  providedIn: 'root'
})
export class GameApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5000/api';

  createGame(request: CreateGameRequest): Observable<GameResponse> {
    return this.http.post<GameResponse>(`${this.baseUrl}/games`, request);
  }

  getGame(id: string): Observable<GameResponse> {
    return this.http.get<GameResponse>(`${this.baseUrl}/games/${id}`);
  }

  makeMove(id: string, request: MakeMoveRequest): Observable<GameResponse> {
    return this.http.post<GameResponse>(`${this.baseUrl}/games/${id}/moves`, request);
  }

  undoMove(id: string): Observable<GameResponse> {
    return this.http.post<GameResponse>(`${this.baseUrl}/games/${id}/undo`, {});
  }

  resetGame(id: string): Observable<GameResponse> {
    return this.http.post<GameResponse>(`${this.baseUrl}/games/${id}/reset`, {});
  }

  getScoreboard(): Observable<ScoreboardResponse> {
    return this.http.get<ScoreboardResponse>(`${this.baseUrl}/scoreboard`);
  }

  resetScoreboard(): Observable<ScoreboardResponse> {
    return this.http.post<ScoreboardResponse>(`${this.baseUrl}/scoreboard/reset`, {});
  }
}
