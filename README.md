# Tic-Tac-Toe Full-Stack Application

A modern, full-stack Tic-Tac-Toe web application built with an **ASP.NET Core Clean Architecture** (.NET 10) backend and a reactive **Angular Standalone** (v19) frontend, crafted under strict **Test-Driven Development (TDD)** and **SOLID** software craftsmanship principles.

---

## 1. Project Overview

This repository delivers an enterprise-grade, full-featured Tic-Tac-Toe web application. It combines an ASP.NET Core Web API backend that acts as the single authority of truth for all game rules and state persistence with an Angular Single-Page Application (SPA) utilizing NgRx SignalStore, Angular Signals, and a cyber-dark design system.

The project was developed following:
- **Clean Architecture**: Domain (zero external dependencies), Application (use cases & sealed record DTOs), Infrastructure (SQLite EF Core persistence), and Api (thin minimal endpoints).
- **Test-Driven Development (TDD)**: Red-Green-Refactor across backend unit tests, API integration tests, Angular component tests, and Playwright end-to-end automation.
- **Backend State Authority**: All move validations, win/draw evaluations, AI computations, and undo state transformations are strictly enforced by the backend.
- **Rich User Experience**: Cyber-dark glassmorphism aesthetics, glowing player markers, smooth micro-animations, ARIA accessibility, and sub-200ms latency.

---

## 2. Tech Stack

| Area | Technology | Purpose |
| :--- | :--- | :--- |
| **Backend Framework** | .NET 10 (`net10.0`) / C# 13 | High-performance Web API runtime with primary constructors |
| **Architecture** | Clean Architecture | Strict layer decoupling with inward dependency rule |
| **Database & ORM** | SQLite / EF Core 10 | Local persistence for games, moves, and session scoreboard |
| **Frontend Framework** | Angular 19+ (Standalone) | Modern SPA with standalone components (no `NgModule`) |
| **Reactive State** | NgRx SignalStore & Angular Signals | Declarative, boilerplate-free state management and computed signals |
| **Styling & Design** | Vanilla CSS & Custom Design Tokens | Cyber-dark glassmorphism, responsive CSS grid, zero Tailwind |
| **Backend Testing** | xUnit, FluentAssertions, `WebApplicationFactory` | Unit tests for domain logic and integration tests for API endpoints |
| **Frontend Testing** | Angular Testing Utilities & Karma/Jasmine | Isolated unit and component DOM interaction tests |
| **End-to-End Testing** | Playwright & Playwright MCP | Cross-browser automated user journeys and latency performance tests |

---

## 3. Features Implemented

1. **Two-Player Mode**:
   - Interactive 3×3 grid with 1-indexed coordinates (Row 1–3, Column 1–3).
   - Real-time turn tracking (Player X starts first, alternating turns).
   - Automatic win detection evaluating all 8 winning combinations (3 rows, 3 columns, 2 diagonals).
   - Winning cells visual highlight with pulsating emerald glow and victory banner.
   - Draw detection triggering when all 9 cells are occupied without a winner.
   - Board locking upon game completion preventing further moves.

2. **Play Against Computer Mode**:
   - Single-player mode with computer playing as Player O.
   - Strategic heuristic AI evaluation prioritizing:
     1. **Win**: Complete an open 3-in-a-row.
     2. **Block**: Prevent an immediate opponent win.
     3. **Center**: Claim square (2,2) if open.
     4. **Corner**: Claim any open corner (1,1), (1,3), (3,1), (3,3).
     5. **Any**: Claim any remaining open cell.
   - Artificial 300–500ms "thinking" delay with spinning indicator and UI interaction locking for authentic gameplay feel.

3. **Move History**:
   - Chronological list of all executed moves displaying move number, player badge (`X` or `O`), and board coordinates `(Row, Col)`.
   - Dynamic counter badge and scrollable history container.

4. **Move Undo**:
   - **Two-Player Mode**: Rolls back the single most recent move, frees the board cell, reverts the turn indicator, and updates history.
   - **Computer Mode**: Atomically rolls back move-pairs (both the computer's response and the player's preceding move) so the human remains on their turn.
   - Automatically disabled when no moves exist on the board or after game completion.

5. **Game Reset**:
   - Clears board, empties move history, resets turn to Player X, and unlocks grid while preserving the selected game mode and session scoreboard.

6. **Session Scoreboard**:
   - Persistent session tracking for Player X Wins, Player O Wins, and Draws.
   - Independent Scoreboard Reset button resetting all counts to zero without resetting active game board.

7. **Error Handling & Resilience**:
   - Backend RFC 7807 Problem Details for 400 Bad Request (occupied cells, out-of-bounds moves), 404 Not Found, and 409 Conflict.
   - Frontend dismissible error banner alerting users if API communication fails without desynchronizing board state.

8. **Accessibility & Responsive Layout**:
   - Full ARIA semantics: `role="grid"`, `role="row"`, `role="gridcell"`, `role="radiogroup"`, `role="radio"`, `role="toolbar"`, `role="region"`, `aria-live="polite"`, and `aria-live="assertive"`.
   - Keyboard navigable controls and screen-reader descriptive labels.
   - Responsive layouts optimized for mobile, tablet, and desktop/laptop viewports without horizontal scrolling.

---

## 4. How to Run Backend

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Steps
1. Navigate to the backend API directory:
   ```bash
   cd src/backend/Api
   ```
2. Restore dependencies and run the Web API:
   ```bash
   dotnet run
   ```
3. The API will start on:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5001`
   - Swagger / OpenAPI UI: `http://localhost:5000/swagger`

*Note: Database creation and EF Core migrations run automatically on startup via `EnsureCreated()` / migration initialization.*

---

## 5. How to Run Frontend

### Prerequisites
- [Node.js](https://nodejs.org/) (v20+ LTS recommended) & `npm`

### Steps
1. Navigate to the frontend directory:
   ```bash
   cd src/frontend
   ```
2. Install dependencies:
   ```cmd
   cmd /c npm install
   ```
3. Start the Angular development server:
   ```cmd
   cmd /c npm start
   ```
4. Open your browser and navigate to `http://localhost:4200`.

### Quick Launch (Concurrent Full-Stack)
On Windows, you can launch both backend and frontend simultaneously with a single command from the project root:
```cmd
run.bat
```

---

## 6. API Endpoint Summary

All API responses follow standard HTTP semantics and return RFC 7807 Problem Details on error.

| HTTP Method | Endpoint | Request Body | Status Code | Description |
| :--- | :--- | :--- | :--- | :--- |
| `POST` | `/api/games` | `{ "mode": "TwoPlayer" \| "Computer" }` | `201 Created` | Creates a new game session with selected mode. |
| `GET` | `/api/games/{id}` | *(None)* | `200 OK` | Retrieves game state, board cells, current player, status, and moves. |
| `POST` | `/api/games/{id}/moves` | `{ "row": 1..3, "column": 1..3 }` | `200 OK` | Applies a move. In Computer mode, automatically computes and applies AI response. |
| `POST` | `/api/games/{id}/reset` | *(None)* | `200 OK` | Clears game board and resets turn to X while maintaining mode and scoreboard. |
| `POST` | `/api/games/{id}/undo` | *(None)* | `200 OK` | Undoes 1 move in Two-Player mode, or 2 moves (move-pair) in Computer mode. |
| `GET` | `/api/scoreboard` | *(None)* | `200 OK` | Fetches session-level scoreboard counts (`xWins`, `oWins`, `draws`). |
| `POST` | `/api/scoreboard/reset` | *(None)* | `200 OK` | Resets session scoreboard counts back to zero. |

---

## 7. How to Run Tests

### Backend Tests (xUnit)
Run all backend unit and integration test suites:
```bash
dotnet test
```
Or run individual test projects:
```bash
# Domain Unit Tests (Core rules, win conditions, undo, computer strategy)
dotnet test tests/backend/Domain.UnitTests/Domain.UnitTests.csproj

# API Integration Tests (Endpoint routing, DB persistence, validation, error handling)
dotnet test tests/backend/Api.IntegrationTests/Api.IntegrationTests.csproj
```

### Frontend Tests (Karma / Jasmine)
Run all Angular unit and component tests:
```cmd
cd src/frontend
cmd /c npm test -- --watch=false
```

### End-to-End Tests (Playwright)
Run the Playwright E2E suite covering all user journeys and latency performance tests:
```cmd
cd tests/e2e
cmd /c npm test
```

---

## 8. AI Tools and Prompt Summary

This project was engineered using the **Google Antigravity AI Agent** leveraging the **SpecKit** methodology across iterative phases:

1. **Specification & Clarification (`speckit-specify`, `speckit-clarify`)**:
   - Synthesized natural language requirements into formal specifications (`spec.md`).
   - Clarified edge cases (e.g., move-pair undo in Computer mode, independent scoreboard reset, 1-indexed coordinates).
2. **Architectural Planning (`speckit-plan`)**:
   - Structured Clean Architecture layers with inward dependency rules.
   - Designed immutable sealed record DTOs, repository contracts in Domain, and NgRx SignalStore state structure.
3. **Task Breakdown (`speckit-tasks`)**:
   - Decomposed user stories into granular, test-first tasks (`tasks.md`) following strict Red-Green-Refactor sequence.
4. **Iterative Implementation (`speckit-implement`)**:
   - **Phases 1–2**: Foundations, domain entities, SQLite EF Core repositories, and minimal API.
   - **Phases 3–6**: Two-Player gameplay loop, draw evaluation, game reset, and mode selection.
   - **Phases 7–8**: Move history tracking and single-move undo.
   - **Phases 9–10**: Session scoreboard persistence and strategic computer opponent with thinking delay.
   - **Phases 11–12**: Computer move-pair undo, Playwright E2E testing, ARIA accessibility, and documentation.
5. **Playwright MCP Integration**:
   - Leveraged the Playwright MCP server to automate browser sessions, verify click-to-render latency under 200ms, and validate UI micro-interactions.

---

## 9. Design Decisions

1. **Clean Architecture with Zero External Dependencies in Domain**:
   - The `Domain` layer contains pure C# entities (`Game`, `Move`, `Scoreboard`), value objects, and repository interfaces. It has no dependencies on ASP.NET Core, EF Core, or third-party libraries, ensuring high testability and enterprise maintainability.
2. **Backend as the Single Source of Truth**:
   - The frontend never calculates win conditions, valid moves, or AI moves client-side. The backend validates all moves and returns the canonical game state, avoiding client/server state drift.
3. **Event-Sourced Replay Undo (Option A)**:
   - Moves are recorded sequentially. An undo pops the target move(s) from the collection and recalculates the board grid and current player turn from the remaining history. This provides guaranteed state consistency and eliminates desynchronization bugs.
4. **Move-Pair Rollback in Computer Mode**:
   - In vs Computer mode, an undo removes both the computer's automated response (Player O) and the user's preceding move (Player X), returning control immediately to the human player without leaving the game in an unnatural state.
5. **Angular Standalone Components with NgRx SignalStore**:
   - Adopted modern Angular Signals (`signal`, `computed`) combined with `@ngrx/signals` `signalStore`. This delivers declarative reactivity, fine-grained change detection, and zero boilerplate compared to legacy NgRx Store or RxJS subjects.
6. **Strict Three-File Component Architecture**:
   - Every component is encapsulated in its own directory with dedicated `.ts`, `.html`, and `.css` files. Inline templates and inline CSS are strictly prohibited.
7. **Curated Vanilla CSS Design Tokens**:
   - Utilized native CSS variables in `shared/styles/tokens.css` for a cohesive cyber-dark theme, glowing neon accents, and smooth cubic-bezier transitions without external heavyweight CSS frameworks.

---

## 10. Clarifications and Assumptions

1. **Coordinate Indexing**:
   - Board coordinates are 1-indexed: Row `1..3` and Column `1..3` to match user intuition and spec requirements.
2. **Player Roles**:
   - Player `X` always takes the first turn in every new game and after board resets.
   - In vs Computer mode, the human player is always `X` and the computer is always `O`.
3. **Computer Opponent Delay**:
   - An artificial delay of 300–500ms is introduced on the frontend to simulate "thinking" time, during which board inputs are locked to prevent race conditions.
4. **Scoreboard Scope**:
   - The scoreboard persists across board resets and mode switches within the active session. It is only reset when the user explicitly clicks the "Reset Scoreboard" button.
5. **Game Over Invariants**:
   - Once a game enters `Won` or `Draw` status, no further moves can be placed. The board must be reset to start a new game.

---

## 11. Known Limitations

1. **Single Concurrent Game**:
   - The current SQLite storage implementation tracks active game sessions by ID, but the UI is optimized for a single active game container per browser session.
2. **Fixed AI Heuristic**:
   - The computer opponent uses a deterministic priority algorithm (Win > Block > Center > Corner > Any). It does not currently feature difficulty toggles (e.g. Easy / Random vs Impossible Minimax).
3. **Local Multiplayer Only**:
   - Two-Player mode is pass-and-play on the same machine. Networked multiplayer over WebSockets / SignalR is not included in the current scope.
4. **Session-Scoped Scoreboard**:
   - Scoreboard data is stored in the local SQLite database for the session; it does not require user authentication or cloud profile sync.

---

## 12. Future Improvements

1. **Configurable AI Difficulty Levels**:
   - Add an AI difficulty selector: Easy (random moves), Medium (current heuristic), and Hard (unbeatable Minimax algorithm).
2. **Real-Time Remote Multiplayer**:
   - Integrate ASP.NET Core SignalR hubs for real-time WebSocket communication, allowing two players on different devices to match and play in shared rooms.
3. **Custom Sound Effects & Audio Themes**:
   - Implement Web Audio API sound effects for cell clicks, victory fanfare, draw alerts, and undo actions with a mute toggle.
4. **Theme Switcher**:
   - Introduce customizable visual themes (e.g., Cyberpunk Neon, Minimal Light, Retro Arcade, Emerald Matrix) using CSS custom properties.
5. **User Profiles & Leaderboard**:
   - Add optional ASP.NET Core Identity authentication to track win/loss statistics, achievements, and global leaderboards.
