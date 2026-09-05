# Tasks: 002-round2-full-game

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure.

- [ ] T001 Create backend Clean Architecture projects (Domain, Application, Infrastructure, Api) and solution file
- [ ] T002 Initialize Angular standalone project in `src/frontend`
- [ ] T003 Configure SQLite and EF Core dependencies in Infrastructure/Api projects
- [ ] T004 [P] Configure Playwright E2E testing framework in `tests/e2e/`
- [ ] T005 [P] Configure xUnit test projects for backend (`tests/backend/`)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T006 [P] Create domain enums (`Player`, `GameMode`, `GameStatus`) in `src/backend/Domain/Enums/`
- [ ] T007 [P] Create value objects (`CellPosition`, `Move`) in `src/backend/Domain/ValueObjects/`
- [ ] T008 [P] Create aggregate roots (`Game`, `Scoreboard`) in `src/backend/Domain/Entities/`
- [ ] T009 Create Application layer DTOs (e.g., `GameResponse`, `CreateGameRequest`) in `src/backend/Application/DTOs/`
- [ ] T010 Setup `TicTacToeDbContext` with SQLite and EF Core migrations in `src/backend/Infrastructure/Data/`
- [ ] T011 Create global exception handler middleware for RFC 7807 Problem Details in `src/backend/Api/Middleware/`
- [ ] T012 Set up Angular global styles, design tokens (`shared/styles/tokens.css`), and root component layout
- [ ] T013 Create frontend `GameApiService` in `src/frontend/src/app/core/services/`
- [ ] T014 Create frontend NgRx SignalStore `GameStore` in `src/frontend/src/app/features/game/state/`

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Two-Player Game with Win Detection (Priority: P1) 🎯 MVP

**Goal**: Core game loop — placing marks, alternating turns, and detecting a winner.

**Independent Test**: Create game, play sequence, X wins, board freezes.

### Tests for User Story 1 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T015 [P] [US1] Unit tests for win detection logic explicitly asserting winning-cell coordinates for row, column, and diagonal wins in `tests/backend/Domain.UnitTests/GameTests.cs`
- [ ] T016 [P] [US1] Integration tests for GET `/api/games/{id}`, POST `/api/games`, and POST `/api/games/{id}/moves` (explicitly testing rejection of out-of-bounds, occupied cell, wrong player, and completed game moves) in `tests/backend/Api.IntegrationTests/GameEndpointsTests.cs`
- [ ] T016a [P] [US1] Integration test verifying complete API response contract (id, board, currentPlayer, gameMode, status, winner, winningCells, moves) in `tests/backend/Api.IntegrationTests/GameEndpointsTests.cs`
- [ ] T017 [P] [US1] Component tests for `BoardComponent` click interactions in `tests/frontend/BoardComponent.spec.ts`
- [ ] T017a [P] [US1] Component tests for FR-024 error handling (dismissible banner, keep valid state, no optimistic mutation) in `tests/frontend/ErrorBannerComponent.spec.ts`

### Implementation for User Story 1

- [ ] T018 [P] [US1] Implement `Game` win detection and move validation logic in `src/backend/Domain/Entities/Game.cs`
- [ ] T019 [US1] Implement POST `/api/games` minimal API endpoint in `src/backend/Api/Endpoints/GameEndpoints.cs`
- [ ] T019a [US1] Implement GET `/api/games/{id}` endpoint in `src/backend/Api/Endpoints/GameEndpoints.cs`
- [ ] T020 [US1] Implement POST `/api/games/{id}/moves` endpoint in `src/backend/Api/Endpoints/GameEndpoints.cs`
- [ ] T021 [US1] Implement `GameStore` methods `createGame` and `makeMove` in `src/frontend/src/app/features/game/state/game.store.ts`
- [ ] T022 [P] [US1] Create `BoardComponent` and `CellComponent` in `src/frontend/src/app/features/game/components/`
- [ ] T023 [US1] Create `GamePageComponent` (smart container) combining board and state in `src/frontend/src/app/features/game/containers/`
- [ ] T023a [US1] Create `ErrorBannerComponent` and wire up HTTP error interceptor to display dismissible banner and prevent optimistic mutation in `src/frontend/src/app/features/game/components/`

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Draw Detection (Priority: P1)

**Goal**: Draw detection when all cells are filled without a winner.

**Independent Test**: Play a nine-move sequence filling the board without a win, verify draw state.

### Tests for User Story 2 ⚠️

- [ ] T024 [P] [US2] Unit tests for draw detection logic in `tests/backend/Domain.UnitTests/GameTests.cs`
- [ ] T024a [P] [US2] Integration test verifying API returns Draw status and frontend correctly renders the draw state in `tests/backend/Api.IntegrationTests/GameEndpointsTests.cs`

### Implementation for User Story 2

- [ ] T025 [US2] Implement draw detection logic in `src/backend/Domain/Entities/Game.cs`
- [ ] T026 [US2] Update `GamePageComponent` to display "It's a draw" message when status is Draw in `src/frontend/src/app/features/game/containers/game-page.component.ts`

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - Game Reset (Priority: P1)

**Goal**: Reset the board to start a fresh game.

**Independent Test**: Complete a game, click Reset Game, verify board clears and turn resets to X.

### Tests for User Story 3 ⚠️

- [ ] T027 [P] [US3] Integration test for POST `/api/games/{id}/reset` including explicit assertion that the scoreboard is preserved in `tests/backend/Api.IntegrationTests/GameEndpointsTests.cs`
- [ ] T028 [P] [US3] Component test for reset button in `tests/frontend/GameControlsComponent.spec.ts`

### Implementation for User Story 3

- [ ] T029 [US3] Implement POST `/api/games/{id}/reset` minimal API endpoint in `src/backend/Api/Endpoints/GameEndpoints.cs`
- [ ] T030 [US3] Implement `resetGame` method in `GameStore` in `src/frontend/src/app/features/game/state/game.store.ts`
- [ ] T031 [US3] Create `GameControlsComponent` with Reset button in `src/frontend/src/app/features/game/components/` and integrate into `GamePageComponent`

---

## Phase 6: User Story 9 - Game Mode Selection (Priority: P1)

**Goal**: Select between Two-Player and Computer Mode before or during a game.

**Independent Test**: Switch modes, verify current game is discarded and a new session begins.

### Tests for User Story 9 ⚠️

- [ ] T032 [P] [US9] Component/Integration test for mode selector verifying discard of current game, clear board/history, preserve scoreboard, and new session creation in `tests/frontend/GameModeSelectorComponent.spec.ts`
- [ ] T032a [P] [US9] Integration test for POST `/api/games` verifying `GameMode` is correctly persisted and returned in the response in `tests/backend/Api.IntegrationTests/GameEndpointsTests.cs`

### Implementation for User Story 9

- [ ] T033 [US9] Update POST `/api/games` to accept and set GameMode.
- [ ] T034 [US9] Create `GameModeSelectorComponent` in `src/frontend/src/app/features/game/components/`
- [ ] T035 [US9] Integrate mode switching into `GameStore` and `GamePageComponent` to start new game on mode change.

---

## Phase 7: User Story 4 - Move History (Priority: P2)

**Goal**: View a running log of all moves made so far.

**Independent Test**: Make three moves, verify move history panel shows three entries.

### Tests for User Story 4 ⚠️

- [ ] T036 [P] [US4] Component test for move history rendering in `tests/frontend/MoveHistoryComponent.spec.ts`

### Implementation for User Story 4

- [ ] T037 [US4] Implement chronological move history tracking (move number, player, row, column) populated after every valid move in `src/backend/Domain/Entities/Game.cs` and `GameResponse` mapping.
- [ ] T038 [US4] Create `MoveHistoryComponent` in `src/frontend/src/app/features/game/components/` to render `Move[]` from store.

---

## Phase 8: User Story 5 - Undo Last Move in Two-Player Mode (Priority: P2)

**Goal**: Undo the most recent move in Two-Player mode.

**Independent Test**: Make two moves, click Undo, verify last move is removed and turn reverts.

### Tests for User Story 5 ⚠️

- [ ] T039 [P] [US5] Unit tests for Two-Player undo logic explicitly asserting restoration of `InProgress` status from pre-terminal state, accurate history, and correct turn in `tests/backend/Domain.UnitTests/GameUndoTests.cs`
- [ ] T040 [P] [US5] Integration test for POST `/api/games/{id}/undo` in `tests/backend/Api.IntegrationTests/GameEndpointsTests.cs`

### Implementation for User Story 5

- [ ] T041 [US5] Implement `UndoMove` logic in `src/backend/Domain/Entities/Game.cs` (handling Option A: disabled if won/draw, removes single move for TwoPlayer mode).
- [ ] T042 [US5] Implement POST `/api/games/{id}/undo` endpoint in `src/backend/Api/Endpoints/GameEndpoints.cs`
- [ ] T043 [US5] Add `undoMove` to `GameStore` and wire up Undo button in `GameControlsComponent` (disabled when history empty or game over).

---

## Phase 9: User Story 6 - Scoreboard (Priority: P2)

**Goal**: Session-level scoreboard tracking wins/draws.

**Independent Test**: Win a game, draw a game, verify scoreboard updates exactly once per completion.

### Tests for User Story 6 ⚠️

- [ ] T044 [P] [US6] Unit tests for Scoreboard explicitly asserting exact-once increment for X/O/Draw, preservation on Reset Game, clearing on Reset Scoreboard, and ignoring incomplete games in `tests/backend/Domain.UnitTests/ScoreboardTests.cs`
- [ ] T045 [P] [US6] Integration tests for GET `/api/scoreboard` and POST `/api/scoreboard/reset` in `tests/backend/Api.IntegrationTests/ScoreboardEndpointsTests.cs`

### Implementation for User Story 6

- [ ] T046 [US6] Implement singleton `ScoreboardService` in `src/backend/Application/Services/ScoreboardService.cs` (updating upon terminal game states).
- [ ] T047 [US6] Implement GET `/api/scoreboard` and POST `/api/scoreboard/reset` endpoints in `src/backend/Api/Endpoints/ScoreboardEndpoints.cs`
- [ ] T048 [US6] Create `ScoreboardStore` in `src/frontend/src/app/core/state/scoreboard.store.ts`
- [ ] T049 [US6] Create `ScoreboardComponent` in `src/frontend/src/app/features/game/components/`

---

## Phase 10: User Story 7 - Computer Opponent Mode (Priority: P2)

**Goal**: Computer plays as O using priority strategy after a human move.

**Independent Test**: Make a move in Computer mode, verify computer responds (Win > Block > Center > Corner > Any) after 300-500ms delay.

### Tests for User Story 7 ⚠️

- [ ] T050 [P] [US7] Unit tests for Computer strategy explicitly covering all 5 priorities (Win, Block, Center, Corner, Any) in `tests/backend/Domain.UnitTests/ComputerStrategyTests.cs`
- [ ] T051 [P] [US7] Component test for computer "thinking" indicator and delay UI lock in `tests/frontend/GamePageComponent.spec.ts`

### Implementation for User Story 7

- [ ] T052 [US7] Implement pure `ComputerStrategy` logic in `src/backend/Domain/Services/ComputerStrategy.cs`
- [ ] T053 [US7] Update `MakeMove` in Game entity / Application logic to automatically apply computer move if GameMode == Computer.
- [ ] T054 [US7] Implement 300-500ms artificial delay, "thinking" UI indicator, and UI locking (disable board clicks) during computer turn in `GameStore` / `GamePageComponent`.

---

## Phase 11: User Story 8 - Undo in Computer Mode (Priority: P2)

**Goal**: Undo removes move-pairs in Computer mode.

**Independent Test**: Click undo in Computer mode, verify both O and X moves are removed.

### Tests for User Story 8 ⚠️

- [ ] T055 [P] [US8] Unit tests for move-pair undo logic in `tests/backend/Domain.UnitTests/GameUndoTests.cs`
- [ ] T055a [P] [US8] Integration test for POST `/api/games/{id}/undo` in Computer mode verifying both X and O moves are removed in `tests/backend/Api.IntegrationTests/GameEndpointsTests.cs`

### Implementation for User Story 8

- [ ] T056 [US8] Update `UndoMove` logic in `Game.cs` to pop two moves if GameMode == Computer and turn is X.

---

## Phase 12: Polish & Cross-Cutting Concerns

**Purpose**: E2E testing, error handling UX, and final styling.

- [ ] T058 Implement Playwright E2E tests for core flows (Two-Player win, Computer mode, Reset, Scoreboard) in `tests/e2e/game.spec.ts`.
- [ ] T059 Refine styling, ensure ARIA attributes, and add micro-animations to `tokens.css` and components.
- [ ] T060 Run quickstart.md validation to confirm E2E compliance.

---

## Dependencies & Execution Order

### Phase Dependencies
- **Setup (Phase 1)**: No dependencies
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: Depend on Foundational. Some P2 stories (like US8, US7) build upon US1, US5, US9 logically, though implementation is additive.
- **Polish (Final Phase)**: Depends on user stories complete.

### Implementation Strategy

1. MVP Delivery: Execute Phases 1, 2, and 3. This yields a playable Two-Player game.
2. Fast Follows: Add Draw, Reset, and Game Mode logic (Phases 4-6).
3. Value Adds: Move History and Undo (Phases 7-8).
4. Meta & Single Player: Scoreboard and Computer Opponent (Phases 9-11).
5. Final Polish (Phase 12).
