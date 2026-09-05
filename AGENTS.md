# AGENTS.md

## Project Overview
This repository contains a full-stack web application consisting of:
- **Backend**: ASP.NET Core Web API built with .NET 10 (`net10.0`) following Clean Architecture.
- **Frontend**: Modern Angular single-page application using standalone components and signals.
- **Testing**: Strict Test-Driven Development (TDD) across both backend and frontend suites.

---

## Project Structure

```
├── src/
│   ├── backend/
│   │   ├── Domain/          # Enterprise business logic, entities, value objects, enums (no external dependencies)
│   │   ├── Application/     # Use cases, interfaces, DTOs, business workflows
│   │   ├── Infrastructure/  # Repositories, data persistence, external service implementations
│   │   └── Api/             # ASP.NET Core Web API, controllers/endpoints, middleware, CORS, Swagger
│   └── frontend/
│       └── src/app/
│           ├── core/        # API services, state management, models, HTTP interceptors
│           ├── features/    # Feature-based smart containers and presentational components
│           └── shared/      # Reusable UI components, design tokens, layout primitives
└── tests/
    ├── backend/             # xUnit unit and integration test suites (.NET 10)
    └── frontend/            # Angular unit and component test suites
```

---

## Non-Negotiable Core Principles

### 1. Test-Driven Development (TDD First)
- Follow the Red-Green-Refactor lifecycle for all new code and endpoints:
  1. Write failing unit/integration tests first.
  2. Implement the minimum code necessary to make the tests pass.
  3. Refactor while maintaining green tests.
- Maintain high test coverage across both backend domain/application layers and frontend components/services.

### 2. Backend Clean Architecture
- Follow inward dependency flow strictly:
  - `Domain` has zero external framework or third-party dependencies.
  - `Application` depends only on `Domain`.
  - `Infrastructure` and `Api` depend on `Application` and implement abstractions.
- Business rules and state machines reside exclusively in `Domain`.
- Orchestration and use-case execution reside in `Application`.

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

### 5. Backend State Authority
- The backend serves as the single source of truth for application state and session data.
- The frontend acts as a responsive presentation client, communicating via REST APIs and synchronizing its state with backend responses.

### 6. Frontend Architecture & Design System
- **Standalone Architecture**: Modern Angular standalone components without legacy `NgModule` patterns.
- **Reactive State Flow**: Angular Signals (`signal`, `computed`) for transparent, performant state management.
- **Component Separation**:
  - Smart (container) components orchestrate state and API communication.
  - Presentational (dumb) components handle UI rendering and user interactions via pure inputs and outputs.
- **Design Excellence**: Modern UI with responsive layouts, accessible controls, smooth micro-animations, and curated design tokens.

---

## Git Commit Rules
- Imperative mood only (`Add`, `Fix`, `Remove`, `Update` — not `Added` or `Fixes`).
- Subject line must be 50 characters or fewer.
- Describe only what changed, not why or how.
- Do NOT add a body, bullet points, or explanations — subject line only.
- Do NOT use prefixes like `feat:`, `fix:`, or `chore:`.
- Do NOT use generic messages like `update code` or `fix bug`.
