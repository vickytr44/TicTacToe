# Data Model: Round 2 — Full Tic-Tac-Toe Game

**Feature Branch**: `002-round2-full-game`
**Date**: 2026-09-05

---

## Enums

### `Player`
Represents the two players in a game.

| Value | Description |
|-------|-------------|
| `X`   | Player X (always goes first) |
| `O`   | Player O (human in Two-Player, computer in Computer mode) |

**Serialization**: String (e.g., `"X"`, `"O"`)

---

### `GameMode`
Represents the mode of a game session.

| Value | Description |
|-------|-------------|
| `TwoPlayer` | Both players are human |
| `Computer`  | Human plays X, computer plays O |

**Serialization**: String (e.g., `"TwoPlayer"`, `"Computer"`)

---

### `GameStatus`
Represents the current status of a game.

| Value | Description |
|-------|-------------|
| `InProgress` | Game is active, moves can be made |
| `Won`        | A player has completed a row, column, or diagonal |
| `Draw`       | All 9 cells filled with no winner |

**Serialization**: String (e.g., `"InProgress"`, `"Won"`, `"Draw"`)

---

## Entities

### `Game` (Aggregate Root)

The central domain entity representing a single Tic-Tac-Toe game session.

| Field | Type | Description | Constraints |
|-------|------|-------------|-------------|
| `Id` | `Guid` | Unique game identifier | Generated on creation, immutable |
| `Board` | `Player?[3][3]` | 3×3 grid of cell values | Each cell is `null` (empty), `X`, or `O` |
| `CurrentPlayer` | `Player` | Whose turn it is | Always starts as `X` after creation/reset |
| `GameMode` | `GameMode` | Game mode | Set on creation, immutable for the session |
| `Status` | `GameStatus` | Current game status | `InProgress` → `Won` or `Draw` (terminal, irreversible) |
| `Winner` | `Player?` | The winning player | `null` unless `Status == Won` |
| `WinningCells` | `List<CellPosition>` | Cells forming the winning line | Empty unless `Status == Won` |
| `Moves` | `List<Move>` | Ordered list of all moves | Append-only during play; pop on undo |
| `CreatedAt` | `DateTimeOffset` | Timestamp of game creation | UTC, set on creation |

**Validation Rules**:
- A move is valid only if:
  - `Status` is `InProgress`
  - The target cell is empty (`Board[row][col] == null`)
  - The `Player` in the move request matches `CurrentPlayer`
  - Row and Column are in range `[0, 2]` (internal) / `[1, 3]` (API)
- After each valid move:
  1. Place the mark on the board
  2. Append to `Moves`
  3. Check for win (8 lines: 3 rows, 3 columns, 2 diagonals)
  4. If win found: set `Status = Won`, `Winner = currentPlayer`, `WinningCells = winning line`
  5. Else if all 9 cells filled: set `Status = Draw`
  6. Else: toggle `CurrentPlayer`

**State Transitions**:

```
                ┌─────────────────┐
                │   InProgress    │
                │ CurrentPlayer=X │
                └────────┬────────┘
                         │ Valid Move
                         ▼
              ┌──────────────────────┐
              │    Check Win/Draw    │
              └──────────┬───────────┘
                    ╱    │    ╲
                  ╱      │      ╲
          Win Found   No Win    All Filled
               │     No Draw       │
               ▼        │          ▼
        ┌──────────┐    │   ┌──────────┐
        │   Won    │    │   │   Draw   │
        │ Winner=P │    │   │          │
        │ (frozen) │    │   │ (frozen) │
        └──────────┘    │   └──────────┘
                        ▼
              ┌──────────────────┐
              │   InProgress     │
              │ Toggle Player    │
              └──────────────────┘
```

---

### `Move` (Value Object)

Represents a single player action within a game.

| Field | Type | Description | Constraints |
|-------|------|-------------|-------------|
| `MoveNumber` | `int` | Sequential move number (1-based) | Assigned based on position in `Moves` list |
| `Player` | `Player` | The player who made this move | Must match `CurrentPlayer` at time of move |
| `Position` | `CellPosition` | The target cell | Must be within bounds and empty |

---

### `CellPosition` (Value Object)

Represents a position on the 3×3 board.

| Field | Type | Description | Constraints |
|-------|------|-------------|-------------|
| `Row` | `int` | Row index | API: `1–3`, Internal: `0–2` |
| `Column` | `int` | Column index | API: `1–3`, Internal: `0–2` |

**Note**: The API uses 1-based indexing (Row 1–3, Column 1–3) per the spec. The domain internally uses 0-based indexing for array access. Conversion happens at the API/Application boundary.

---

### `Scoreboard` (Aggregate)

Session-level aggregate tracking cumulative results across multiple games.

| Field | Type | Description | Constraints |
|-------|------|-------------|-------------|
| `XWins` | `int` | Number of games won by X | `>= 0`, incremented on X win |
| `OWins` | `int` | Number of games won by O | `>= 0`, incremented on O win |
| `Draws` | `int` | Number of drawn games | `>= 0`, incremented on draw |

**Update Rules**:
- Updated exactly once per completed game (`Status == Won` or `Status == Draw`)
- Incomplete/abandoned games (reset, mode switch) do **not** affect the scoreboard
- `ResetScoreboard` sets all counts to `0`
- Persists across game resets within the same server session

---

## Relationships

```
Scoreboard (1) ──── aggregates results from ──── Game (many)
     │
     │  (session-level, independent lifecycle)
     │
Game (1) ──── contains ordered ──── Move (0..9)
     │
     │  Move.Position references ──── CellPosition
     │
     │  Game.WinningCells references ──── CellPosition (0 or 3)
```

---

## Computer Move Strategy (Domain Logic)

The computer opponent selects a move based on the following priority, evaluated sequentially:

| Priority | Strategy | Description |
|----------|----------|-------------|
| 1 | **Win** | If O can complete a row/column/diagonal, play there |
| 2 | **Block** | If X can complete a row/column/diagonal, block it |
| 3 | **Center** | If cell (2,2) (1-indexed) is empty, take it |
| 4 | **Corner** | Take any empty corner: (1,1), (1,3), (3,1), (3,3) |
| 5 | **Any** | Take any remaining empty cell |

**Input**: Current board state (`Player?[3][3]`)
**Output**: `CellPosition` — the selected cell for O's move
**Determinism**: The same board state always produces the same move (first match in priority order, corners evaluated in fixed order).
