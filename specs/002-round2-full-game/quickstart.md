# Quickstart Validation Guide: Round 2 — Full Tic-Tac-Toe Game

**Feature Branch**: `002-round2-full-game`
**Date**: 2026-09-05

This guide provides runnable validation scenarios to prove the feature works end-to-end after implementation.

---

## Prerequisites

1. Ensure the .NET 10 SDK and Node.js are installed.
2. Both the ASP.NET Core backend and Angular frontend development servers must be running.

### Setup Commands (To run after implementation)
```bash
# Start Backend (runs on http://localhost:5000; applies SQLite migrations automatically on startup via Database.Migrate())
dotnet run --project src/backend/Api

# (Optional) Manually apply EF Core migrations outside startup:
# dotnet ef database update --project src/backend/Infrastructure --startup-project src/backend/Api

# Start Frontend (runs on http://localhost:4200)
cd src/frontend
npm start
```

---

## Validation Scenario 1: Two-Player Game & Win Detection

**Objective**: Verify the core game loop, win detection, board freeze, and scoreboard update.

1. Open `http://localhost:4200` in a browser.
2. Ensure **Two-Player** mode is selected.
3. Click the board cells in this sequence:
   - (Row 1, Col 1) -> X
   - (Row 2, Col 1) -> O
   - (Row 1, Col 2) -> X
   - (Row 2, Col 2) -> O
   - (Row 1, Col 3) -> X
4. **Expected Outcome**:
   - The UI immediately displays "Player X wins!".
   - Cells (1,1), (1,2), and (1,3) are visually highlighted.
   - The board is frozen (clicking other cells does nothing).
   - The Scoreboard updates to `X: 1, O: 0, Draws: 0`.

---

## Validation Scenario 2: Undo Functionality

**Objective**: Verify that moves can be undone and board state accurately rolls back.

1. Click **Reset Game**.
2. Click (Row 2, Col 2) -> X
3. Click (Row 3, Col 3) -> O
4. Click the **Undo** button.
5. **Expected Outcome**:
   - The O mark at (3,3) disappears.
   - The turn indicator returns to Player O.
   - The Move History removes the second entry.
6. Click the **Undo** button again.
7. **Expected Outcome**:
   - The X mark at (2,2) disappears (board is empty).
   - The turn indicator returns to Player X.
   - The Undo button becomes disabled.

---

## Validation Scenario 3: Computer Mode & Strategy

**Objective**: Verify the computer opponent responds automatically and follows strategic priorities (win > block > center > corner > any).

1. Select **Play Against Computer** from the mode selector. (This discards any active game and starts a new one).
2. Click (Row 1, Col 1) -> X.
3. **Expected Outcome**:
   - The UI shows "Computer thinking..." for 300-500ms.
   - The computer automatically places O in (Row 2, Col 2) [Center priority].
4. Click (Row 3, Col 3) -> X.
5. **Expected Outcome**:
   - The computer automatically places O in (Row 1, Col 3) or (Row 3, Col 1) or (Row 1, Col 2) or (Row 2, Col 1) to prevent an immediate X win on a diagonal or to build its own line, or takes a corner depending on exact priority evaluation, but importantly, it responds automatically.
   *Specifically to test Block:*
   - X (1,1) -> O (2,2) [Center]
   - X (1,2) -> O (1,3) [Block row 1]
6. Click **Undo**.
7. **Expected Outcome**:
   - **Both** the computer's last move and your preceding X move are removed.
   - The turn returns to X.

---

## Validation Scenario 4: Error Handling

**Objective**: Verify frontend resilience when making invalid moves or if the backend is down.

1. Click (Row 1, Col 1) -> X.
2. While the backend is running, attempt to bypass UI validation by sending a direct API request (e.g., via Swagger at `https://localhost:5001/swagger`) to place a move on (Row 1, Col 1).
3. **Expected Outcome**: The API returns a `400 Bad Request` Problem Details JSON indicating the cell is occupied.
4. Stop the backend server (`Ctrl+C` in the terminal).
5. Click any empty cell on the board.
6. **Expected Outcome**:
   - A dismissible error banner appears above the board ("Something went wrong. Please try again.").
   - The board does not optimisticly render the X (it stays empty).
