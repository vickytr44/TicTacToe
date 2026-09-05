# Frontend Guidelines (Angular Standalone & Signals)

## Overview & Technology Stack
- **Framework**: Modern Angular (Standalone Components)
- **State Management**: **NgRx** (`@ngrx/signals` / `signalStore` or NgRx Store) integrating with Angular Signals
- **Styling**: Vanilla CSS with custom design tokens (Avoid TailwindCSS unless explicitly requested)
- **Testing**:
  - Unit & Component: Angular test runner with TDD (Red-Green-Refactor)
  - End-to-End: Playwright E2E test suite (`tests/e2e/`) with Playwright MCP server integration

---

## Directory Structure (`src/frontend/src/app/`)

```
src/app/
├── core/        # Singleton services, API clients, state stores, models, HTTP interceptors, guards
├── features/    # Feature modules containing smart containers and presentational components
└── shared/      # Reusable UI primitives, design tokens, utility pipes, shared layout components
```

---

## Component Architecture

### 1. Standalone Components Only
- All components, directives, and pipes must be standalone (`standalone: true`).
- Legacy `NgModule` patterns are strictly prohibited.

### 2. Smart (Container) vs Presentational (Dumb) Separation
- **Smart / Container Components**:
  - Located in `features/*/containers/` or feature root.
  - Inject services, dispatch API calls, and manage feature-level state using Signals.
  - Coordinate routing and pass signal values to presentational child components.
- **Presentational / Dumb Components**:
  - Located in `features/*/components/` or `shared/components/`.
  - Rely exclusively on signal inputs (`input()`, `input.required()`) and event outputs (`output()`).
  - Zero direct API service injection; purely focused on UI rendering and emitting user interactions.

### 3. State Management (NgRx & Signals)
- **NgRx Architecture**: Use **NgRx** for application and feature state management, favoring `@ngrx/signals` (`signalStore`) to seamlessly align with Angular Signals.
- **Store Location**: Feature stores reside in `features/*/state/` and global session stores reside in `core/state/`.
- **State Design**:
  - Keep state models immutable with explicit types.
  - Expose state properties as signals and derive computed values using `withComputed` / `computed()`.
  - Encapsulate async backend API interactions, loading states, and error handling inside store methods (e.g. `withMethods` or NgRx Effects).
- Smart/container components inject the NgRx store to read signals and dispatch actions or state update methods.

### 4. Backend Communication & State Synchronization
- The frontend acts as a responsive presentation client; the backend remains the single source of truth.
- Synchronize local signal state immediately with backend REST API responses.
- Implement clear loading, error, and optimistic state feedback for user actions.

### 5. File Separation Standard (Mandatory: .ts, .html, .css)
- **Three-File Component Pattern**: Every Angular component MUST be structured into three separate dedicated files for maximum readability and maintainability:
  1. `<name>.component.ts`: TypeScript class, component decorator using `templateUrl` and `styleUrl`, signal inputs (`input()`), outputs (`output()`), and injected services/stores.
  2. `<name>.component.html`: Pure HTML template with native control flow (`@if`, `@for`, `@switch`).
  3. `<name>.component.css`: Component-scoped Vanilla CSS styles referencing tokens from `tokens.css`.
- **No Inline Templates or Styles**: Never use inline `template: \`...\`` or `styles: [\`...\`]` in component metadata.

---

## Design System & Styling
- **CSS Architecture**: Use Vanilla CSS with scoped component styles and centralized design tokens in `shared/styles/tokens.css`.
- **Aesthetics**: Premium, modern interface with curated color palettes, accessible contrast ratios, responsive flex/grid layouts, and subtle micro-animations.
- **Semantic HTML**: Proper heading hierarchy (`h1` per view), descriptive button text, ARIA attributes, and accessible keyboard navigation.

---

## Testing Standards (TDD & E2E)
- **Unit & Component Tests**:
  - **Location**: Component and service test specs in `tests/frontend/` (or co-located `*.spec.ts`).
  - **Lifecycle**: TDD Red-Green-Refactor:
    1. Write failing component/service unit tests first.
    2. Implement the minimum component template/logic to satisfy the test.
    3. Refactor component code and styling while maintaining green tests.
  - Test user interactions by querying DOM elements, simulating click/input events, and asserting signal state changes.
- **End-to-End (E2E) Tests with Playwright**:
  - **Location**: Test scripts in `tests/e2e/`.
  - **Scope**: Validate end-to-end user journeys (game creation, turns, victory, draw, board reset, responsiveness).
- **Playwright MCP Server Tooling**:
  - The connected `playwright` MCP server tools (`browser_navigate`, `browser_click`, `browser_find`, `browser_snapshot`, `browser_take_screenshot`, `browser_console_messages`, etc.) must be used during development for live browser testing, DOM tree inspection, visual assertions, and debugging E2E flows.
