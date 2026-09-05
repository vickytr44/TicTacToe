# Feature Specification: Round 2 — Full Tic-Tac-Toe Game

**Feature Branch**: `002-round2-full-game`

**Created**: 2026-09-05

**Status**: Draft

**Input**: User description: "Build a browser-based Tic Tac Toe application with an Angular frontend and a .NET backend running locally. The application should allow users to play Tic Tac Toe, track moves, undo moves, maintain a scoreboard, and support a basic computer opponent mode."

## User Scenarios & Testing *(mandatory)*

### User Story 1 — Two-Player Game with Win Detection (Priority: P1)

Two users sit down to play a standard game of Tic Tac Toe against each other. Player X goes first, and they alternate turns by clicking empty cells on the board. When one player completes a row, column, or diagonal, the application announces the winner, highlights the winning cells, and prevents further moves. The scoreboard updates to reflect the win.

**Why this priority**: The core game loop — placing marks, alternating turns, and detecting a winner — is the fundamental value proposition. Without it nothing else matters.

**Independent Test**: Create a new game in Two-Player mode, play a sequence of moves that results in a row win for X, and verify the winner is displayed, winning cells are highlighted, further moves are blocked, and the scoreboard increments X wins by one.

**Acceptance Scenarios**:

1. **Given** a new game in Two-Player mode, **When** Player X clicks an empty cell, **Then** an X mark appears in that cell, the cell becomes locked, and the turn indicator switches to Player O.
2. **Given** it is Player O's turn, **When** Player O clicks an empty cell, **Then** an O mark appears in that cell, the cell becomes locked, and the turn indicator switches to Player X.
3. **Given** Player X occupies three cells forming a complete row, **When** the last winning move is placed, **Then** the application displays "Player X wins", highlights the three winning cells, prevents any additional moves on the board, and increments X wins on the scoreboard.
4. **Given** Player O occupies three cells forming a complete column, **When** the last winning move is placed, **Then** the application displays "Player O wins", highlights the three winning cells, prevents any additional moves, and increments O wins on the scoreboard.
5. **Given** Player X occupies three cells forming a diagonal, **When** the last winning move is placed, **Then** the application displays "Player X wins", highlights the diagonal cells, and updates the scoreboard.
6. **Given** a player clicks an already-occupied cell, **When** the click is registered, **Then** no change occurs on the board and the current turn does not change.
7. **Given** a game is won, **When** a player clicks any cell on the board, **Then** nothing happens — the board remains frozen.

---

### User Story 2 — Draw Detection (Priority: P1)

Two players fill all nine cells without either player completing a row, column, or diagonal. The application declares a draw, prevents further moves, and updates the scoreboard.

**Why this priority**: Draw is one of the two possible game outcomes and must be handled alongside wins for a complete game experience.

**Independent Test**: Play a nine-move sequence that fills the board without a win and verify the draw message, board freeze, and scoreboard draw count increment.

**Acceptance Scenarios**:

1. **Given** all nine cells are filled and no player has three in a row, column, or diagonal, **When** the last move is placed, **Then** the application displays "It's a draw", prevents additional moves, and increments the draw count on the scoreboard.
2. **Given** a draw has been declared, **When** a player clicks any cell, **Then** nothing happens.

---

### User Story 3 — Game Reset (Priority: P1)

After a game is completed (won or drawn), or in the middle of a game, the user can reset the board to start a fresh game. The scoreboard is preserved across resets.

**Why this priority**: Users need to play multiple games in sequence without refreshing the browser. Reset enables session continuity.

**Independent Test**: Complete a game, click Reset Game, and verify the board is cleared, move history is empty, turn resets to X, and the scoreboard retains its previous values.

**Acceptance Scenarios**:

1. **Given** a completed or in-progress game, **When** the user clicks Reset Game, **Then** the board clears, move history empties, the winner/draw message disappears, current player resets to X, and a new game session begins.
2. **Given** the scoreboard shows X: 2, O: 1, Draws: 1, **When** the user clicks Reset Game, **Then** the scoreboard continues to show X: 2, O: 1, Draws: 1.

---

### User Story 4 — Move History (Priority: P2)

During a game, the user can see a running log of all moves made so far, showing the move number, which player made it, and the board position.

**Why this priority**: Move history adds transparency and helps users understand the sequence of play. It supports undo functionality and builds trust in the backend state.

**Independent Test**: Start a game, make three moves, and verify the move history panel lists three entries in order with correct move numbers, player marks, and positions.

**Acceptance Scenarios**:

1. **Given** a new game with no moves, **When** the game starts, **Then** the move history panel is empty.
2. **Given** Player X places a mark at Row 1 Column 1, **When** the move is completed, **Then** the move history shows one entry: Move 1, Player X, Row 1 Column 1.
3. **Given** three moves have been made, **When** the user views move history, **Then** the history shows three sequentially numbered entries with correct players and positions.
4. **Given** a game is reset, **When** the user views the move history, **Then** it is empty.

---

### User Story 5 — Undo Last Move in Two-Player Mode (Priority: P2)

In Two-Player mode, the user can undo the most recent move, restoring the board, the correct turn, and the move history to their previous state.

**Why this priority**: Undo is a must-have requirement that improves the user experience by allowing mistake correction. Two-player undo is simpler than computer-mode undo.

**Independent Test**: Make two moves, click Undo, and verify the second move is removed, the board reflects only the first move, the turn reverts, and the move history shows one entry.

**Acceptance Scenarios**:

1. **Given** Player X plays, then Player O plays (two moves exist), **When** the user clicks Undo, **Then** Player O's move is removed from the board, the turn reverts to Player O, and the move history shows only one entry.
2. **Given** no moves have been made, **When** the user views the Undo button, **Then** the Undo button is disabled.
3. **Given** a game was won and undo after completion is disabled (Option A), **When** the user views the Undo button, **Then** the Undo button is disabled.

---

### User Story 6 — Scoreboard (Priority: P2)

A session-level scoreboard tracks the number of wins for X, wins for O, and draws across multiple games. The scoreboard updates once per completed game and can be independently reset.

**Why this priority**: The scoreboard adds long-term engagement and allows players to track their competitive performance across a session.

**Independent Test**: Play two games (one X win, one draw), verify the scoreboard shows X: 1, Draws: 1. Click Reset Scoreboard and verify all counts return to zero.

**Acceptance Scenarios**:

1. **Given** a new session, **When** the scoreboard loads, **Then** X wins is 0, O wins is 0, and Draws is 0.
2. **Given** Player X wins a game, **When** the win is declared, **Then** X wins increments by 1.
3. **Given** the scoreboard shows X: 3, O: 2, Draws: 1, **When** the user clicks Reset Scoreboard, **Then** all counts reset to 0.
4. **Given** a game is completed, **When** Reset Game is clicked, **Then** the scoreboard does not change.
5. **Given** a completed game, **When** the scoreboard is updated, **Then** it updates exactly once for that game result.

---

### User Story 7 — Computer Opponent Mode (Priority: P2)

The user can choose to play against a computer opponent. The human plays as X and the computer plays as O. After the human makes a move, the computer automatically responds with a strategic move following a defined priority order.

**Why this priority**: Computer mode adds single-player replayability and demonstrates backend game logic capabilities. It depends on the core game loop being fully functional.

**Independent Test**: Select Computer Mode, make a move as X, and verify the computer immediately responds as O with a valid move. Verify the computer's move follows the defined priority: win if possible, block opponent's win, take center, take corner, take any available cell.

**Acceptance Scenarios**:

1. **Given** Computer Mode is selected and it is the human's turn, **When** the human places X, **Then** the computer automatically places O in a valid cell following the strategy priority, the board updates to show both moves, and the turn indicator returns to X.
2. **Given** the computer can win with one move, **When** the computer's turn arrives, **Then** the computer plays the winning move.
3. **Given** the human can win with one move, **When** the computer's turn arrives, **Then** the computer blocks the human's winning cell.
4. **Given** no immediate win or block is needed and the center is empty, **When** the computer's turn arrives, **Then** the computer takes the center cell.
5. **Given** the game is already won or drawn, **When** the computer's turn would normally trigger, **Then** the computer does not make any move.
6. **Given** no winning or blocking move exists and the center is occupied, **When** the computer's turn arrives, **Then** the computer selects an available corner cell.
7. **Given** no winning, blocking, center, or corner move is available, **When** the computer's turn arrives, **Then** the computer selects any remaining available cell.

---

### User Story 8 — Undo in Computer Mode (Priority: P2)

In Computer Mode, undo removes both the computer's last move and the human's preceding move together, effectively rolling back one full "round" so the human can re-play their turn.

**Why this priority**: Undo must behave differently in computer mode to provide a meaningful correction opportunity. Undoing only the computer's move would immediately re-trigger the computer, making undo useless.

**Independent Test**: In Computer Mode, make a move (X), let the computer respond (O), click Undo, and verify both the O and X moves are removed, the turn returns to X, and move history reflects the rollback.

**Acceptance Scenarios**:

1. **Given** Computer Mode with X at Row 1 Col 1 and O at Row 2 Col 2, **When** the user clicks Undo, **Then** both moves are removed, the board shows no marks, and it is X's turn.
2. **Given** Computer Mode with no moves made, **When** the user views the Undo button, **Then** the Undo button is disabled.

---

### User Story 9 — Game Mode Selection (Priority: P1)

Before starting a game, the user selects a game mode: Two-Player or Play Against Computer. The selected mode determines the behavior of player turns and undo for the entire game session.

**Why this priority**: Mode selection is a prerequisite for computer mode and must be available from the start.

**Independent Test**: Start the application, select each mode, and verify the game behaves according to the selected mode.

**Acceptance Scenarios**:

1. **Given** the application loads, **When** the user sees the game interface, **Then** a mode selector is visible offering Two-Player and Play Against Computer options.
2. **Given** the user selects Two-Player mode, **When** a new game starts, **Then** both X and O are controlled by users and undo removes a single move.
3. **Given** the user selects Computer mode, **When** a new game starts, **Then** the human plays as X, the computer plays as O automatically, and undo removes a move pair.

---

### Edge Cases

- What happens when the user rapidly clicks multiple cells before the backend responds? **Design decision**: The UI will prevent duplicate or concurrent move submissions while a move request is pending. This ensures consistency with the backend as the single source of truth.
- What happens when the user clicks Undo immediately after a win is declared? Under Option A (the assumed default), undo is disabled after game completion.
- What happens if the user switches game modes mid-game? **Design decision**: Changing game mode during an active game starts a new game session (clearing the board and move history) and preserves the scoreboard. This avoids undefined states such as a computer needing to retroactively respond to moves made in Two-Player mode.
- What happens when the computer's strategic priorities all point to the same cell? The computer simply plays that cell — priority order is evaluated sequentially and the first matching cell is selected.
- What happens if the user clicks Reset Scoreboard during an active game? The scoreboard resets to zero; the current in-progress game continues unaffected.
- What happens when undo is clicked multiple times consecutively in Two-Player mode? Each click removes one move until no moves remain, at which point the Undo button is disabled.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a standard 3×3 game board where each cell is clickable when empty and displays X or O after a move is placed.
- **FR-002**: System MUST alternate turns between Player X and Player O, starting with Player X after every game reset.
- **FR-003**: System MUST prevent moves on occupied cells, moves by the wrong player, and moves after game completion, rejecting them without changing game state.
- **FR-004**: System MUST detect a win when a player completes any row, column, or diagonal and immediately display the winner, highlight winning cells, and freeze the board.
- **FR-005**: System MUST detect a draw when all nine cells are filled without a winner and immediately display a draw message, freeze the board, and update the scoreboard.
- **FR-006**: System MUST provide a Reset Game action that clears the board, clears move history, clears game status, resets the current player to X, starts a new game session, and preserves the scoreboard.
- **FR-007**: System MUST display a move history for the current game showing move number, player, and cell position for each move, updated after every valid move.
- **FR-008**: System MUST provide an Undo Last Move action that restores the board, correct player turn, and move history to the previous state and recalculates game status.
- **FR-009**: In Two-Player mode, Undo MUST remove only the single most recent move.
- **FR-010**: In Computer mode, Undo MUST remove both the computer's last move and the preceding human move together as a pair.
- **FR-011**: Undo MUST be disabled when no moves exist to undo.
- **FR-012**: System MUST maintain a session-level scoreboard tracking X wins, O wins, and draws, updating exactly once per completed game.
- **FR-013**: System MUST provide a Reset Scoreboard action that resets all scoreboard counts to zero.
- **FR-014**: System MUST support two game modes: Two-Player (both human) and Play Against Computer (human X, computer O).
- **FR-015**: In Computer mode, the computer MUST automatically make a valid move after each human move, following this priority: (1) win if possible, (2) block opponent's win, (3) take center, (4) take a corner, (5) take any available cell.
- **FR-016**: The computer MUST NOT make a move after the game is already completed.
- **FR-017**: The backend MUST own all game state including session, move validation, game status, move history, and scoreboard. The frontend renders backend responses.
- **FR-018**: The backend MUST expose REST API endpoints for all game operations. The suggested API scope is as follows (exact endpoint names may vary, but the solution must clearly document the API contract):

  | Method | Endpoint | Purpose |
  | ------ | -------- | ------- |
  | POST | /api/games | Create a new game session |
  | GET | /api/games/{id} | Get current game state |
  | POST | /api/games/{id}/moves | Submit a player move |
  | POST | /api/games/{id}/undo | Undo last move |
  | POST | /api/games/{id}/reset | Reset the current game |
  | GET | /api/scoreboard | Get scoreboard |
  | POST | /api/scoreboard/reset | Reset scoreboard |

- **FR-019**: The backend MUST validate all incoming move requests and reject invalid moves (out-of-bounds, occupied cell, wrong player, game already completed) with appropriate error information.
- **FR-020**: A move request MUST contain: the game ID, the player making the move (X or O), and the cell position represented as a row and column pair (each ranging from 1 to 3).
- **FR-021**: The game state response MUST include: game ID, board state, current player, game mode, game status, winner (if any), winning cells (if any), and move history. The current scoreboard MUST be either included in the game state response or retrievable via a separate dedicated mechanism.
- **FR-022**: The frontend MUST display: game board, current player indicator, selected game mode, winner/draw message, highlighted winning cells, move history, scoreboard, Reset Game button, Undo button, and Reset Scoreboard button.
- **FR-023**: After game completion, undo MUST be disabled to keep the scoreboard final for that game (Option A: Disable Undo After Completion).

### Key Entities

- **Game Session**: Represents a single game instance. Contains the board state (3×3 grid), current player (X or O), game mode (Two-Player or Computer), game status (In Progress, Won, Draw), winner (if any), winning cells (if any), and the ordered list of moves.
- **Move**: Represents a single player action within a game. Contains the move sequence number, the player who made it (X or O), and the target cell position (row and column).
- **Scoreboard**: Represents session-level aggregate results. Contains counts for X wins, O wins, and draws. Exists independently of any single game and persists across game resets.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: The game flow — placing marks, alternating turns, and reaching a win or draw — works smoothly without perceptible delays or interruptions in a standard local development environment.
- **SC-002**: Winning cells are visually highlighted immediately upon win detection, clearly distinguishing them from non-winning cells.
- **SC-003**: In Computer mode, the computer responds automatically after the human's move without requiring any additional user action.
- **SC-004**: Undo restores the board to its correct previous state with 100% accuracy across all game modes.
- **SC-005**: The scoreboard accurately reflects the correct count of wins and draws after any sequence of games and resets.
- **SC-006**: All core game logic test cases pass, covering: valid move, invalid move, turn switching, row/column/diagonal wins, draw, reset, undo in both modes, scoreboard update, computer move selection, and move after completion.
- **SC-007**: The application is usable on a standard laptop browser without horizontal scrolling or layout breakage.
- **SC-008**: The application can be started locally by following the README instructions without additional troubleshooting or undocumented steps.

## Assumptions

- **Users have a modern desktop or laptop browser** (Chrome, Firefox, Edge) with JavaScript enabled. Mobile-optimized layouts are not required, though the UI should be comfortable on a laptop screen.
- **Single-session, single-user scope**: The application runs locally for one user at a time. Multi-user networking, authentication, and persistent storage across browser sessions are out of scope.
- **Option A for Undo After Completion**: Undo is disabled once a game is won or drawn. The scoreboard remains final for that game. This simplifies state management and aligns with the most intuitive user expectation.
- **In-memory or SQLite storage**: Game state and scoreboard persist only for the lifetime of the backend server process. Restarting the server resets all state. SQLite may be used if preferred but in-memory is acceptable.
- **Design decision — mode switching during an active game**: Changing game mode mid-game starts a new game session and preserves the scoreboard. The mode selector remains accessible at all times. This prevents undefined states when transitioning between human and computer-controlled players.
- **No authentication or user accounts**: Players are identified only as X and O within a game session.
- **Standard web performance expectations**: API response times under 500ms, page load under 3 seconds on a local development machine.
- **Computer opponent strategy is deterministic**: The defined priority list (win → block → center → corner → any) produces consistent, predictable moves. Advanced AI (minimax, alpha-beta pruning) is not required.
- **Design decision — silent ignore for occupied-cell clicks**: When a user clicks an already-occupied cell through the UI, the frontend will silently ignore the click rather than display an error modal. The backend still validates and rejects invalid move requests independently.
