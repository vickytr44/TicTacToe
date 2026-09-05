# Research: Round 2 — Full Tic-Tac-Toe Game

**Feature Branch**: `002-round2-full-game`
**Date**: 2026-09-05

---

## 1. Backend Architecture — .NET 10 Clean Architecture with SQLite

### Decision: 4-Project Clean Architecture Layout
- **Rationale**: The constitution mandates strict inward dependency flow (`Domain ◄ Application ◄ Infrastructure & Api`). A 4-project solution provides compile-time enforcement of layer boundaries.
- **Alternatives considered**:
  - **Single project with folder separation**: Simpler but allows accidental cross-layer references. Rejected because the constitution explicitly requires zero external dependencies in Domain.
  - **3-project (merging Infrastructure + Api)**: Fewer projects but conflates persistence with HTTP host concerns. Rejected for SRP reasons.

### Decision: In-Memory SQLite for Dev, File-Based SQLite Optional
- **Rationale**: The spec allows in-memory or SQLite. Using EF Core with SQLite provides persistence across requests within a server session, while keeping setup trivial (no external DB server). In-memory SQLite (`Data Source=:memory:`) loses data on connection close, so a file-based SQLite (`Data Source=tictactoe.db`) is preferred for dev. The `DbContext` lifetime is scoped per request, but the SQLite file persists across requests.
- **Alternatives considered**:
  - **Pure in-memory dictionaries (no EF Core)**: Faster and simpler but loses the ability to demonstrate real persistence patterns required by the constitution's Infrastructure layer expectations. Rejected.
  - **PostgreSQL/SQL Server**: Overkill for a local single-user app. Rejected.

### Decision: Minimal API Endpoints (not Controllers)
- **Rationale**: .NET 10 minimal APIs are the modern recommended pattern for focused RESTful endpoints. They align well with the 7 endpoints defined in the spec (POST/GET game, POST move, POST undo, POST reset, GET scoreboard, POST scoreboard/reset) without the ceremony of MVC controllers.
- **Alternatives considered**:
  - **MVC Controllers**: More boilerplate with `[ApiController]`, `[HttpPost]`, etc. Viable but more ceremonial for this scope. Rejected for KISS.

### Decision: Domain Game Logic as Pure Functions
- **Rationale**: The constitution mandates 100% unit test coverage on Domain with zero external dependencies. Game rules (win detection, draw detection, move validation, computer AI) should be pure methods on the `Game` entity or static helper classes, operating on immutable board state.
- **Alternatives considered**:
  - **Domain services with injected dependencies**: Unnecessary since game logic has no external I/O. Rejected for simplicity.

---

## 2. Frontend Architecture — Angular Standalone with NgRx SignalStore

### Decision: NgRx SignalStore (`@ngrx/signals`) for State Management
- **Rationale**: The constitution mandates NgRx with `@ngrx/signals` / `signalStore` integrated with native Angular Signals. Research confirms `signalStore` with `withState`, `withComputed`, `withMethods`, and `patchState` provides a clean, boilerplate-free approach. The store encapsulates async API calls, loading states, and error handling.
- **Alternatives considered**:
  - **NgRx Store (classic Redux pattern)**: More boilerplate with actions, reducers, effects. Rejected because the constitution specifically favors `@ngrx/signals`.
  - **Plain Angular Signals only (no NgRx)**: Loses the structured store pattern, computed derivation, and DI integration. Rejected per constitution mandate.

### Decision: Single Feature Module — `game`
- **Rationale**: The entire application is a single-page Tic-Tac-Toe game. A single `features/game/` module with container and presentational components keeps things cohesive without over-engineering feature boundaries.
- **Alternatives considered**:
  - **Multiple feature modules (board, scoreboard, history)**: Over-segmentation for a single-page app with tightly coupled state. Rejected for YAGNI.

### Decision: Smart/Dumb Component Split
- **Rationale**: Constitution mandates smart containers in `features/*/containers/` and dumb components in `features/*/components/`. The game page container orchestrates state; the board, scoreboard, move history, and controls are presentational.
- **Components planned**:
  - **Smart**: `GamePageComponent` (container) — injects `GameStore`, coordinates API flow.
  - **Dumb**: `BoardComponent`, `CellComponent`, `ScoreboardComponent`, `MoveHistoryComponent`, `GameControlsComponent`, `GameModeSelector`, `ErrorBannerComponent`.

### Decision: HttpClient Service for API Communication
- **Rationale**: Angular best practice is to encapsulate HTTP calls in a dedicated injectable service (`GameApiService` in `core/services/`). The store's `withMethods` calls this service for all backend interactions.
- **Alternatives considered**:
  - **Direct `httpResource` / `resource`**: Newer Angular primitives but less control over error handling and loading state management needed here. Rejected for this use case.

---

## 3. Computer Opponent Strategy

### Decision: Deterministic Priority-Based Strategy in Domain Layer
- **Rationale**: The spec defines a fixed priority: (1) win, (2) block, (3) center, (4) corner, (5) any. This is a straightforward sequential evaluation — no minimax or tree search needed. The algorithm lives in the `Domain` layer as a pure function that takes board state and returns a cell position.
- **Alternatives considered**:
  - **Minimax algorithm**: Spec explicitly says advanced AI is not required. Rejected.
  - **Random move selection**: Doesn't meet the spec's priority requirements. Rejected.

---

## 4. Undo Implementation

### Decision: Move Stack with Pop-Based Undo
- **Rationale**: The game stores moves as an ordered list. Undo pops the last move (Two-Player) or last two moves (Computer Mode) and reconstructs the board from remaining moves. This is simpler and more reliable than trying to reverse-apply individual cell changes.
- **Alternatives considered**:
  - **Event sourcing with full replay**: Architecturally elegant but over-engineered for a 9-cell board. Rejected for KISS.
  - **Board snapshot history**: Memory-wasteful for such a small state. Rejected.

---

## 5. Session & Scoreboard Management

### Decision: In-Memory Singleton Scoreboard, Game Sessions via Repository
- **Rationale**: The spec states single-session scope. The scoreboard is a singleton aggregate (X wins, O wins, draws) that persists across game resets but not across server restarts. Game sessions are created/retrieved via a repository backed by SQLite (or in-memory store).
- **Alternatives considered**:
  - **Scoreboard persisted in SQLite**: Possible but spec says in-memory is acceptable. We use SQLite for game state to demonstrate the pattern, and a simple in-memory or SQLite-backed scoreboard.

---

## 6. Error Handling & Frontend Resilience

### Decision: RFC 7807 Problem Details + Dismissible Error Banner
- **Rationale**: The constitution mandates RFC 7807 Problem Details for backend errors. The spec requires a dismissible inline error banner on the frontend. The Angular HTTP interceptor or service-level error handler catches HTTP errors and updates a store-level error signal, which the `ErrorBannerComponent` reads.
- **Alternatives considered**:
  - **Toast/snackbar notifications**: Less discoverable and may auto-dismiss before user reads. Rejected per spec requirement (dismissible inline banner).

---

## 7. Testing Strategy

### Decision: Three-Tier TDD Testing
- **Rationale**: Constitution mandates TDD across all layers plus Playwright E2E.
  1. **Backend xUnit**: Domain pure logic (100% coverage), Application use cases (mocked repos), Integration tests (in-memory SQLite).
  2. **Frontend Angular Tests**: Component DOM interaction tests, store method tests, service mock tests.
  3. **Playwright E2E**: Full user journeys — create game, play moves, win/draw, undo, reset, scoreboard, computer mode.

---

## 8. Performance Considerations

### Decision: Synchronous Domain Logic, Async Only at I/O Boundary
- **Rationale**: The spec requires <200ms for Two-Player moves. Since domain logic is pure computation on a 3×3 board, it's effectively instantaneous. Async is only needed at the persistence (EF Core SaveChanges) and HTTP boundaries. The 300–500ms computer delay is an artificial `Task.Delay` in the frontend, not actual computation time.
- **Note**: The artificial delay is a frontend concern — the backend computes the computer move synchronously and returns it. The frontend introduces the delay before displaying it.
