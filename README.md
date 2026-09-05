# Tic-Tac-Toe Full-Stack Application

A modern, full-stack Tic-Tac-Toe application built with an **ASP.NET Core Clean Architecture** backend and a reactive **Angular Standalone** frontend, developed under strict **Test-Driven Development (TDD)** principles.

---

## Tech Stack

| Area | Technologies |
| :--- | :--- |
| **Backend** | .NET 10 (`net10.0`), C#, ASP.NET Core Web API, Clean Architecture |
| **Database** | SQLite via Entity Framework Core (`Microsoft.EntityFrameworkCore.Sqlite`) |
| **Frontend** | Angular (v19+ Standalone Components), TypeScript |
| **State Management** | NgRx SignalStore (`@ngrx/signals`) & Angular Signals (`signal`, `computed`) |
| **Styling** | Vanilla CSS with custom design tokens (no TailwindCSS) |
| **Testing** | xUnit (.NET 10), Angular Test Runner, Playwright E2E |

---

## Project Structure

```
├── src/
│   ├── backend/             # ASP.NET Core Clean Architecture (.NET 10)
│   │   ├── Domain/          # Core business logic, entities, value objects (zero external deps)
│   │   ├── Application/     # Use cases, interfaces, sealed record DTOs, orchestration
│   │   ├── Infrastructure/  # SQLite EF Core persistence, migrations, external adapters
│   │   └── Api/             # Web API endpoints/controllers, middleware, Swagger/OpenAPI
│   └── frontend/            # Angular Standalone Application
│       └── src/app/
│           ├── core/        # Singleton services, API clients, state stores, interceptors
│           ├── features/    # Feature smart containers and presentational components
│           └── shared/      # UI primitives, design tokens, layout components
└── tests/
    ├── backend/             # xUnit unit and integration test suites (.NET 10)
    ├── frontend/            # Angular unit and component test suites
    └── e2e/                 # Playwright end-to-end test suite (MCP enabled)
```

---

## Architectural Principles & Conventions

### Backend (.NET 10)
* **Clean Architecture**: Inward dependency flow (`Domain` ➔ `Application` ➔ `Infrastructure` & `Api`).
* **Application Services**: Use case workflows and multi-aggregate orchestration reside in `Application/Services/` (e.g., `IGameService`, `GameService`).
* **Rich Domain Models**: Domain business rules (move validity, turns, win/draw detection) stay in domain entities (`Game.cs`, `Scoreboard.cs`).
* **Repository Pattern**: Persistence interfaces reside in `Domain/Repositories/` (e.g., `IGameRepository`, `IScoreboardRepository`), while concrete implementations reside in `Infrastructure/Repositories/`.
* **Thin Endpoints**: API endpoints inject Application services via primary constructors—never `DbContext` or repositories directly.
* **State Authority**: The backend is the single source of truth for board state, move validation, win/draw detection, and session persistence.
* **Primary Constructors**: Always use C# primary constructors for classes, services, repositories, and dependency injection.
* **DTO Standards**: Immutable `sealed record` types with `DateTimeOffset` timestamps and string-serialized enums.
* **Database**: SQLite database with EF Core migrations.

### Frontend (Angular)
* **Standalone Architecture**: 100% standalone components (`standalone: true`); no legacy `NgModule`.
* **State Management**: NgRx SignalStore (`@ngrx/signals`) with native Angular Signals for reactive, boilerplate-free state flow.
* **Component Separation**: Smart containers orchestrate state and API dispatches; dumb presentational components use pure `input()` and `output()`.
* **Design System**: Vanilla CSS design tokens in `shared/styles/tokens.css` with accessible, modern aesthetics and micro-animations.

---

## Subsystem Documentation
* **Global Rules & Standards**: [AGENTS.md](AGENTS.md)
* **Backend Guidelines**: [src/backend/AGENTS.md](src/backend/AGENTS.md)
* **Frontend Guidelines**: [src/frontend/AGENTS.md](src/frontend/AGENTS.md)

---

## Getting Started

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/)
* [Node.js](https://nodejs.org/) (LTS) & npm
* Angular CLI (`npm install -g @angular/cli`)

### Running Tests (TDD & E2E)
```bash
# Run all backend unit and integration tests
dotnet test

# Run frontend unit and component tests
cd src/frontend && npm test

# Run Playwright end-to-end tests
cd tests/e2e && npx playwright test
```

### Playwright MCP Integration
The workspace is configured with the **Playwright MCP Server** (`playwright`), enabling direct browser navigation, live snapshot verification, DOM queries, and interactive E2E testing during agentic sessions.

