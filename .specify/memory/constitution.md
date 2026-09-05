<!--
Sync Impact Report
- Version change: 1.0.0 → 1.1.0
- List of modified principles:
  - I. Test-Driven Development (TDD First - NON-NEGOTIABLE) → I. Test-Driven Development & End-to-End Verification (NON-NEGOTIABLE) (expanded to require Playwright E2E testing and Playwright MCP server tooling integration)
- Added sections:
  - End-to-End testing requirements under Technology Stack & Architectural Constraints and Quality Gates
- Removed sections:
  - None
- Follow-up TODOs:
  - None
-->

# Tic-Tac-Toe Constitution

## Core Principles

### I. Test-Driven Development & End-to-End Verification (NON-NEGOTIABLE)
- **Red-Green-Refactor Lifecycle**: Strict TDD is mandatory across backend, frontend, and end-to-end suites.
- **Test-First Order**: Failing unit, integration, or component tests MUST be written and verified failing before implementing any production code.
- **Implementation Constraint**: Production code is authored solely to make the failing test pass, followed immediately by refactoring while maintaining green tests.
- **Coverage Mandate**: Pure domain logic in `src/backend/Domain/` MUST maintain 100% unit test coverage. Frontend components and services MUST rigorously test DOM element bindings, simulated events, and signal state transitions.
- **End-to-End Verification (Playwright)**: Full-stack user flows—such as game initialization, turn alternation, win/draw detection, and session reset—MUST be covered by Playwright E2E tests in `tests/e2e/`.
- **Playwright MCP Tooling Integration**: Live browser interactions, UI visual checks, DOM snapshot inspections, and regression assertions during development MUST leverage the connected Playwright MCP server tools (`browser_navigate`, `browser_click`, `browser_snapshot`, `browser_take_screenshot`, etc.).
- *Rationale*: Eliminates untested edge cases, guarantees regression resilience, ensures cross-browser fidelity, and drives decoupled, highly testable software design.

### II. Clean Architecture & Inward Dependency Flow
- **Strict Inward Flow**: Architecture MUST adhere to inward dependency rules: `Domain` ◄ `Application` ◄ `Infrastructure` & `Api`.
- **Domain Independence**: `src/backend/Domain/` MUST have zero external dependencies—no third-party packages, no EF Core, and no ASP.NET Core references.
- **Application Contracts**: `src/backend/Application/` defines use case orchestration, service contracts, repository interfaces, and sealed DTOs, depending strictly on `Domain`.
- **Infrastructure & Host**: `src/backend/Infrastructure/` encapsulates SQLite EF Core persistence; `src/backend/Api/` encapsulates ASP.NET Core endpoints, middleware, and dependency wiring.
- **Frontend Layering**: Angular code MUST maintain clean directory boundaries: `core/` for singletons and HTTP, `features/` for feature slices, and `shared/` for reusable UI tokens and primitives.
- *Rationale*: Protects enterprise business rules from technological volatility, external framework churn, and database implementation details.

### III. Backend State Authority
- **Single Source of Truth**: The ASP.NET Core backend is the sole authority for game session state, move validation, turn sequencing, win/draw detection, and persistence.
- **Untrusted Client Input**: Frontend requests MUST be treated as untrusted and validated against backend domain rules before state mutations occur.
- **Presentation Role**: The Angular frontend acts purely as a responsive presentation client, immediately synchronizing its reactive state with backend REST API responses.
- *Rationale*: Prevents invalid board positions, race conditions, out-of-order moves, and state divergence between clients.

### IV. Angular Standalone Architecture & Reactive Signal State
- **Standalone Only**: All Angular components, directives, and pipes MUST be standalone (`standalone: true`). Legacy `NgModule` modules are strictly forbidden.
- **Component Separation**:
  - **Smart (Container) Components**: Located in `features/*/containers/`; inject services, coordinate routing, and manage feature state via NgRx SignalStore.
  - **Presentational (Dumb) Components**: Located in `features/*/components/` or `shared/components/`; rely exclusively on `input()`, `input.required()`, and `output()`, with zero direct API service injection.
- **Reactive State Flow**: State management MUST utilize NgRx (`@ngrx/signals` / `signalStore`) integrated with native Angular Signals (`signal`, `computed`). State models MUST remain immutable.
- *Rationale*: Ensures high modularity, tree-shakability, modern signals reactivity, and clean decoupling of UI layout from data orchestration.

### V. Software Craftsmanship, Modern Idioms & Design Excellence
- **SOLID & Clean Code**: Every class, service, and component MUST have a single responsibility. Favor composition, strategy patterns, DRY, and KISS/YAGNI over speculative complexity.
- **Modern C# Idioms**: Classes accepting dependencies MUST use C# primary constructors. API response models MUST be immutable positional `sealed record` types. Request models MUST use `sealed record` with `init` properties and `DateTimeOffset` UTC timestamps. Enums MUST serialize as strings.
- **Design System & Aesthetics**: Frontend styling MUST use Vanilla CSS with centralized design tokens (`shared/styles/tokens.css`), semantic HTML5, accessible ARIA attributes, and subtle micro-animations. TailwindCSS MUST NOT be used unless explicitly approved.
- *Rationale*: Minimizes ceremonial boilerplate, enforces immutability, guarantees accessible UX, and maintains high code readability.

## Technology Stack & Architectural Constraints

- **Backend Architecture**:
  - **Framework**: .NET 10 (`net10.0`), C#.
  - **Application Model**: ASP.NET Core Web API with RESTful conventions and OpenAPI / Swagger documentation.
  - **Database Persistence**: SQLite via Entity Framework Core (`Microsoft.EntityFrameworkCore.Sqlite`) with managed migrations.
  - **Error Handling**: Global exception handling returning standard RFC 7807 Problem Details.
  - **Data Boundary**: Domain entities MUST NEVER be exposed directly in API request or response models.
- **Frontend Architecture**:
  - **Framework**: Angular (v19+ Standalone Components), TypeScript.
  - **State Management**: NgRx SignalStore (`@ngrx/signals`) and Angular Signals (`signal`, `computed`).
  - **Styling & Design System**: Vanilla CSS with custom tokens in `shared/styles/tokens.css` (no TailwindCSS).
  - **Accessibility**: Semantic HTML5 hierarchy (single `h1` per view), descriptive interactive labels, ARIA landmarks, and full keyboard navigation.
- **Testing Suites**:
  - **Backend**: xUnit (.NET 10) in `tests/backend/`.
  - **Frontend Unit & Component**: Angular test runner in `tests/frontend/` or co-located specs.
  - **End-to-End (E2E)**: Playwright in `tests/e2e/`, integrated with the Playwright MCP server for browser automation, inspection, and verification.

## Development Workflow & Quality Gates

- **TDD Quality Gate**: No production code or endpoint logic may be implemented or committed without a preceding failing test demonstrating its requirement.
- **E2E Verification Gate**: Complete end-to-end flows (game creation, alternating turns, win/draw conditions, reset) must pass Playwright test suites before production deployment.
- **Documentation Integrity Gate**: Whenever architectural decisions, new endpoints, or configuration changes occur, the root `README.md` MUST be updated immediately.
- **Verification Gate**: Backend must build cleanly without warnings (`dotnet build`) and pass all tests (`dotnet test tests/backend/`). Frontend must pass unit tests (`npm test`) and E2E tests (`npx playwright test`).
- **Commit Standards**: Git commit messages MUST follow imperative mood, 50 characters or fewer, no conventional prefixes (`feat:`, `fix:`, etc.), and no multi-line bodies.

## Governance

- **Supreme Authority**: This Constitution represents the primary design and quality standard for the Tic-Tac-Toe project, superseding conflicting local or historical practices.
- **Compliance Verification**: All code contributions, pull requests, and automated implementation tasks MUST verify adherence against these constitutional principles prior to completion.
- **Amendment Procedure**: Modifications to this Constitution require explicit justification, team consensus, and an appropriate semantic version bump:
  - **MAJOR**: Incompatible governance changes, principle removals, or structural redefinitions.
  - **MINOR**: Addition of new principles, sections, or materially expanded architectural guidance.
  - **PATCH**: Clarifications, wording refinements, and non-semantic corrections.
- **Runtime Guidance**: Operational implementation guidelines are maintained in [AGENTS.md](file:///c:/Users/vicky/.gemini/antigravity/scratch/TicTakToe/AGENTS.md), [src/backend/AGENTS.md](file:///c:/Users/vicky/.gemini/antigravity/scratch/TicTakToe/src/backend/AGENTS.md), and [src/frontend/AGENTS.md](file:///c:/Users/vicky/.gemini/antigravity/scratch/TicTakToe/src/frontend/AGENTS.md).

**Version**: 1.1.0 | **Ratified**: 2026-09-05 | **Last Amended**: 2026-09-05
