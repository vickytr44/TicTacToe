# API Contracts: Round 2 — Full Tic-Tac-Toe Game

**Feature Branch**: `002-round2-full-game`
**Date**: 2026-09-05

---

## 1. DTOs (Data Transfer Objects)

The API relies on these immutable `sealed record` DTOs.

### `CreateGameRequest`
Payload for starting a new game session.
```json
{
  "mode": "TwoPlayer" // "TwoPlayer" | "Computer"
}
```

### `MakeMoveRequest`
Payload for executing a move.
```json
{
  "player": "X", // "X" | "O"
  "row": 1,      // 1-3
  "column": 1    // 1-3
}
```

### `MoveDto`
Represents a single move in the history.
```json
{
  "moveNumber": 1,
  "player": "X", // "X" | "O"
  "row": 1,      // 1-3
  "column": 1    // 1-3
}
```

### `CellPositionDto`
Represents a coordinate on the board.
```json
{
  "row": 1,    // 1-3
  "column": 2  // 1-3
}
```

### `GameResponse`
The complete state of a game session. Returned by most game endpoints.
```json
{
  "id": "c1f7a8b4-92e1-4c3a-8b11-d9354a7c1b12",
  "board": [
    ["X", null, "O"],
    [null, "X", null],
    [null, null, null]
  ],
  "currentPlayer": "X", // "X" | "O"
  "gameMode": "TwoPlayer", // "TwoPlayer" | "Computer"
  "status": "InProgress", // "InProgress" | "Won" | "Draw"
  "winner": null, // "X" | "O" | null
  "winningCells": [], // Array of CellPositionDto
  "moves": [ /* Array of MoveDto */ ],
  "createdAt": "2026-09-05T10:00:00Z"
}
```

### `ScoreboardResponse`
The state of the session scoreboard.
```json
{
  "xWins": 2,
  "oWins": 1,
  "draws": 1
}
```

### Error Response (RFC 7807 Problem Details)
Returned for all 400 Bad Request validations.
```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Invalid Move",
  "status": 400,
  "detail": "Cell (1,1) is already occupied.",
  "instance": "/api/games/c1f7a8b4/moves"
}
```

---

## 2. API Endpoints

All endpoints respond with `application/json`.

### `POST /api/games`
Creates a new game session.
- **Request Body**: `CreateGameRequest`
- **Response (201 Created)**: `GameResponse`

### `GET /api/games/{id}`
Retrieves the current state of a game.
- **Path Parameter**: `id` (Guid)
- **Response (200 OK)**: `GameResponse`
- **Response (404 Not Found)**: Standard problem details

### `POST /api/games/{id}/moves`
Submits a player move and evaluates win/draw conditions. In Computer Mode, this triggers the computer's response synchronously before returning.
- **Path Parameter**: `id` (Guid)
- **Request Body**: `MakeMoveRequest`
- **Response (200 OK)**: `GameResponse`
- **Response (400 Bad Request)**: Problem Details (e.g., cell occupied, game over, out of bounds)

### `POST /api/games/{id}/undo`
Undoes the last move (or move pair in Computer Mode).
- **Path Parameter**: `id` (Guid)
- **Response (200 OK)**: `GameResponse`
- **Response (400 Bad Request)**: Problem Details (e.g., no moves to undo, game already completed if Option A enforced)

### `POST /api/games/{id}/reset`
Resets the specified game to a clean board while retaining the Game Mode.
- **Path Parameter**: `id` (Guid)
- **Response (200 OK)**: `GameResponse`

### `GET /api/scoreboard`
Retrieves the global session scoreboard.
- **Response (200 OK)**: `ScoreboardResponse`

### `POST /api/scoreboard/reset`
Resets the global session scoreboard to zeros.
- **Response (200 OK)**: `ScoreboardResponse`
