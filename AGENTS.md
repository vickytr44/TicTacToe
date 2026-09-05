# AGENTS.md

## Project Overview
This repository contains a full-stack web application consisting of:
- **Backend**: ASP.NET Core Web API built with .NET 10 (`net10.0`) following Clean Architecture with SQLite.
- **Frontend**: Modern Angular single-page application using standalone components, signals, and NgRx.
- **Testing**: Strict Test-Driven Development (TDD) across backend, frontend, and Playwright end-to-end suites.

---

## Project Structure

```
├── src/
│   ├── backend/             # ASP.NET Core Clean Architecture (.NET 10) -> see src/backend/AGENTS.md
│   │   ├── Domain/          # Enterprise business logic, entities, value objects (zero external deps)
│   │   ├── Application/     # Use cases, interfaces, DTOs, business workflows
│   │   ├── Infrastructure/  # Repositories, SQLite EF Core persistence, external adapters
│   │   └── Api/             # ASP.NET Core Web API, endpoints, middleware, Swagger
│   └── frontend/            # Angular Standalone SPA -> see src/frontend/AGENTS.md
│       └── src/app/
│           ├── core/        # API services, state management, models, HTTP interceptors
│           ├── features/    # Feature-based smart containers and presentational components
│           └── shared/      # Reusable UI primitives, design tokens, layout components
└── tests/
    ├── backend/             # xUnit unit and integration test suites (.NET 10)
    ├── frontend/            # Angular unit and component test suites
    └── e2e/                 # Playwright end-to-end test suite
```

---

## Subsystem Rule Files
Detailed guidelines and architectural rules are scoped per subsystem and loaded automatically:
- **Backend Guidelines**: [src/backend/AGENTS.md](src/backend/AGENTS.md) (.NET 10, Clean Architecture layers, sealed record DTOs, xUnit testing)
- **Frontend Guidelines**: [src/frontend/AGENTS.md](src/frontend/AGENTS.md) (Angular Standalone, NgRx state management, Signals reactive state, Smart/Dumb components, design tokens)

---

## Non-Negotiable Core Principles

### 1. Test-Driven Development (TDD First & E2E Verification)
- Follow the Red-Green-Refactor lifecycle for all new code, components, and endpoints:
  1. Write failing unit/integration/component tests first.
  2. Implement the minimum code necessary to make the tests pass.
  3. Refactor while maintaining green tests.
- Maintain high test coverage across both backend domain/application layers and frontend components/services.
- **Playwright End-to-End Testing**: Cover critical user journeys (game creation, turns, win/draw conditions, board reset) with Playwright E2E suites under `tests/e2e/`.
- **Playwright MCP Tooling**: Use the connected Playwright MCP server tools (`browser_navigate`, `browser_click`, `browser_snapshot`, `browser_take_screenshot`, etc.) during development for live browser automation, UI inspection, and regression verification.

### 2. Backend State Authority
- The backend serves as the single source of truth for application state and session data.
- The frontend acts as a responsive presentation client, communicating via REST APIs and synchronizing its state with backend responses.

### 3. Strict SOLID & Software Craftsmanship
- **Single Responsibility Principle (SRP)**: Each class, service, and component must have exactly one reason to change.
- **Open/Closed Principle (OCP)**: Favor composition and strategy patterns over rigid branching so behavior is extensible without modifying verified core logic.
- **Liskov Substitution Principle (LSP)**: Interface implementations must be completely substitutable without altering expected behavior.
- **Interface Segregation Principle (ISP)**: Create narrow, role-specific interfaces rather than broad, bloated contracts.
- **Dependency Inversion Principle (DIP)**: High-level modules must depend on abstractions; concrete dependencies are injected via DI.
- **DRY (Don't Repeat Yourself)**: Eliminate duplicate logic; centralize shared calculations, coordinate transforms, validation, and error mappings.
- **KISS & YAGNI**: Keep solutions simple, purposeful, and free of speculative features.

### 4. Code Quality: Testable, Maintainable, Readable
- **Testability**: Isolate side effects, favor pure domain functions, and design for test doubles via dependency injection.
- **Maintainability**: Keep methods and components small and cohesive; avoid tightly coupled classes.
- **Readability**: Use clear, ubiquitous domain language and self-documenting code over obscure tricks.

---

## Documentation & README Maintenance
- **Always Keep `README.md` Updated**: Whenever changes are made that are worthy of mention (e.g., architectural decisions, new features/endpoints, setup requirements, configuration keys, or major workflow changes), always update the root [README.md](README.md) file.
- Maintain documentation integrity: Ensure project setup instructions, architectural diagrams, and feature lists reflect active codebase realities.

---

## Git Commit Rules
- Imperative mood only (`Add`, `Fix`, `Remove`, `Update` — not `Added` or `Fixes`).
- Subject line must be 50 characters or fewer.
- Describe only what changed, not why or how.
- Do NOT add a body, bullet points, or explanations — subject line only.
- Do NOT use prefixes like `feat:`, `fix:`, or `chore:`.
- Do NOT use generic messages like `update code` or `fix bug`.
