# Backend Guidelines (.NET 10 & Clean Architecture)

## Overview & Technology Stack
- **Target Framework**: .NET 10 (`net10.0`), C#
- **Application Type**: ASP.NET Core Web API
- **Database**: SQLite (`Microsoft.EntityFrameworkCore.Sqlite`)
- **Coding Conventions**: Always use C# **primary constructors** for dependency injection and class definitions
- **Architecture**: Clean Architecture with strict inward dependency flow
- **Testing Framework**: xUnit with TDD (Red-Green-Refactor)

---

## Architecture & Layer Responsibilities

```
Domain (Core) ──◄── Application ──◄── Infrastructure & Api
```

### 1. Domain Layer (`src/backend/Domain/`)
- **Role**: Enterprise business logic, entities, value objects, domain events, enums, pure business state machines, and **repository interfaces**.
- **Dependencies**: **Zero external dependencies** — no third-party libraries, no EF Core, no ASP.NET Core references.
- **Authority**: All core business rules and state machines reside exclusively here.
- **Repository Contracts**: All aggregate repository interfaces (`IGameRepository`, `IScoreboardRepository`) **must reside in Domain** under `src/backend/Domain/Repositories/`.
- **Purity**: Favor pure domain functions and immutable state transitions.

### 2. Application Layer (`src/backend/Application/`)
- **Role**: Use case orchestration, application services (`IGameService`, `GameService`), workflow handlers, and DTOs.
- **Dependencies**: Depends **only on Domain**.
- **Use Case Orchestration**: All multi-aggregate workflows, repository queries, cross-aggregate updates (e.g. scoreboard on game finish), and domain entity invocations MUST reside in Application services under `src/backend/Application/Services/`.
- **Preserve Rich Domain**: Application services coordinate domain entities; they MUST NOT strip or duplicate business logic out of entities into procedural service code.
- **DTO Standards**:
  - Always use `sealed record` for request and response DTOs (enforces immutability and value equality).
  - Use positional records for response DTOs: `public sealed record GameResponse(...)`.
  - Use `init` properties for request DTOs with DataAnnotations: `public sealed record CreateGameRequest { ... }`.
  - Use `DateTimeOffset` for all timestamp properties (preserves UTC offset).
  - Enum properties must serialize as strings by default.
  - Never expose Domain entities directly in API request or response models.

### 3. Infrastructure Layer (`src/backend/Infrastructure/`)
- **Role**: Data persistence, repository implementations (`GameRepository`, `ScoreboardRepository`), database contexts (`TicTacToeDbContext`), external service integrations.
- **Dependencies**: Implements repository interfaces defined in `Domain`; depends on `Domain` and `Application`.
- **Repository Implementations**: Concrete repository implementations **must reside in Infrastructure** under `src/backend/Infrastructure/Repositories/`. They encapsulate EF Core / `DbContext` queries and persistence.
- **Database Engine**: **SQLite** via EF Core (`Microsoft.EntityFrameworkCore.Sqlite`).
  - Configure `DbContext` with SQLite connection string (e.g., `Data Source=tictactoe.db`).
  - Encapsulates database migrations, entity type configurations, and I/O-bound operations.

### 4. Api Layer (`src/backend/Api/`)
- **Role**: ASP.NET Core host, controllers/endpoints, middleware, OpenAPI/Swagger, CORS configuration.
- **Dependencies**: Depends on `Application` (for services, DTOs) and wires `Infrastructure` & `Application` implementations via Dependency Injection in `Program.cs`.
- **Thin Endpoints**: Endpoints and controllers MUST remain thin HTTP adapters. Always inject Application services (`IGameService`) via primary constructors—never repository interfaces or `DbContext` directly into endpoints.
- **Endpoint Design**:
  - Maintain consistent HTTP semantics (proper status codes, REST conventions).
  - Centralize error handling via global exception handling middleware or Problem Details (`RFC 7807`).
  - Validate input payloads and return standard `400 Bad Request` Problem Details on failure.

---

## Coding Standards: Primary Constructors
- **Always use C# primary constructors** for all classes, services, repositories, DbContexts, and controllers accepting dependencies.
  ```csharp
  // Standard dependency injection via primary constructor:
  public class GameService(IGameRepository gameRepository, ILogger<GameService> logger) : IGameService
  {
      // Parameters gameRepository and logger are in scope across the entire class body
  }
  ```
- **DTOs / Value Objects**: Use positional primary constructors for immutable `sealed record` output types:
  ```csharp
  public sealed record GameResponse(Guid Id, string Status, string Turn, DateTimeOffset CreatedAt);
  ```
- **No Boilerplate**: Do not declare manual backing fields (`private readonly IGameRepository _repo;`) or traditional constructor assignment bodies (`_repo = repo;`) unless explicitly required.

---

## State Authority & Business Rules
- The backend is the **single source of truth** for application state, move validation, win conditions, and game session persistence.
- Frontend input is treated as untrusted and must be fully validated against domain rules.

---

## Testing Standards (TDD)
- **Framework**: xUnit (located in `tests/backend/`).
- **Lifecycle**: Red-Green-Refactor strictly enforced:
  1. Write failing unit/integration tests first.
  2. Implement the minimum code necessary to make tests pass.
  3. Refactor while maintaining green tests.
- High test coverage required across `Domain` logic (100% pure unit tests) and `Application` use cases.
- Use dependency injection to isolate side effects with test doubles/mocks.
