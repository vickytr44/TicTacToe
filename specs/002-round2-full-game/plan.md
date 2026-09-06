# Implementation Plan: 002-round2-full-game

**Branch**: `002-round2-full-game` | **Date**: 2026-09-05 | **Spec**: [specs/002-round2-full-game/spec.md](file:///c:/Users/vicky/.gemini/antigravity/scratch/TicTakToe/specs/002-round2-full-game/spec.md)

**Input**: Feature specification from `/specs/002-round2-full-game/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Build a full-stack Tic-Tac-Toe application with an ASP.NET Core minimal API backend using Clean Architecture (in-memory/SQLite EF Core) and an Angular 22 standalone frontend using NgRx SignalStore. The application supports Two-Player and Computer modes, win/draw detection, move history, undo (single and pair-based depending on mode), and a session scoreboard. The backend is the single source of truth for all game state and validation.

## Technical Context

**Language/Version**: .NET 10 (C# 13), TypeScript 6.0, Angular 22

**Primary Dependencies**: 
- Backend: `Microsoft.EntityFrameworkCore.Sqlite`, xUnit
- Frontend: `@angular/core`, `@ngrx/signals`, Playwright

**Storage**: SQLite (`Microsoft.EntityFrameworkCore.Sqlite`)

**Testing**: 
- Backend: xUnit (Unit & Integration)
- Frontend: Angular Test Runner
- E2E: Playwright

**Target Platform**: Browser (Local Web App)

**Project Type**: Full-stack Web Application

**Performance Goals**: <200ms response for board updates in Two-Player mode.

**Constraints**: Backend state authority; 100% test coverage on Domain; Strict TDD Red-Green-Refactor; Playwright E2E verification.

**Scale/Scope**: Single user, single session, local execution.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Test-Driven Development**: Adheres. Test suites planned for all layers before implementation.
- **Clean Architecture**: Adheres. 4-project structure enforced. Domain has zero external dependencies.
- **Backend State Authority**: Adheres. All validation, logic, and state live in backend. Frontend is a reactive client.
- **Angular Standalone & Signals**: Adheres. Using standalone components and `@ngrx/signals`.
- **Craftsmanship & Modern Idioms**: Adheres. C# primary constructors, sealed record DTOs, Vanilla CSS styling.

## Project Structure

### Documentation (this feature)

```text
specs/002-round2-full-game/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/           # Phase 1 output (/speckit-plan command)
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
backend/
├── src/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   └── Api/
└── tests/
    └── backend/

frontend/
├── src/
│   ├── core/
│   ├── features/
│   └── shared/
└── tests/
    └── e2e/
```

**Structure Decision**: A Web Application structure cleanly separating the .NET Clean Architecture backend into its 4 distinct layer projects, alongside an Angular frontend workspace and Playwright E2E suite.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

*No violations detected.*
