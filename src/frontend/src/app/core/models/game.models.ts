export type Player = 'X' | 'O';
export type GameMode = 'TwoPlayer' | 'Computer';
export type GameStatus = 'InProgress' | 'Won' | 'Draw';

export interface CellPositionDto {
  row: number;
  column: number;
}

export interface MoveDto {
  moveNumber: number;
  player: Player;
  row: number;
  column: number;
}

export interface GameResponse {
  id: string;
  board: (Player | null)[][];
  currentPlayer: Player;
  gameMode: GameMode;
  status: GameStatus;
  winner: Player | null;
  winningCells: CellPositionDto[];
  moves: MoveDto[];
  createdAt: string;
}

export interface ScoreboardResponse {
  xWins: number;
  oWins: number;
  draws: number;
}

export interface CreateGameRequest {
  mode: GameMode;
}

export interface MakeMoveRequest {
  player: Player;
  row: number;
  column: number;
}
