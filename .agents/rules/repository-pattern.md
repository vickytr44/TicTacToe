# Repository Pattern & Clean Architecture Rules

## Mandatory Rules for Backend Persistence & Endpoints

1. **Repository Interfaces Reside in Domain**:
   - All repository interfaces for aggregate roots and entities (e.g., `IGameRepository`, `IScoreboardRepository`) MUST be defined in the Domain layer under `src/backend/Domain/Repositories/`.
   - The Domain layer defines the persistence contracts needed by the domain without referencing or depending on any database technology, ORM, or EF Core.

2. **Concrete Repositories Reside in Infrastructure**:
   - All concrete repository implementations (e.g., `GameRepository`, `ScoreboardRepository`) MUST be implemented in the Infrastructure layer under `src/backend/Infrastructure/Repositories/`.
   - Repositories encapsulate all Entity Framework Core (`TicTacToeDbContext`) queries, tracking, migrations, and I/O persistence operations.
   - Use C# primary constructors for injecting `TicTacToeDbContext` into repositories.

3. **No Direct `DbContext` Injection**:
   - Never inject `DbContext` (or `TicTacToeDbContext`) directly into Minimal API endpoints, controllers, or application services.
   - Always inject repository interfaces (`IGameRepository`, `IScoreboardRepository`) via primary constructors.

4. **Use Case Orchestration in Application Services**:
   - All use case workflows and orchestration (e.g., `IGameService` / `GameService`) MUST reside in the Application layer under `src/backend/Application/Services/`.
   - Application services coordinate repositories, invoke domain methods on entities, manage cross-aggregate interactions (e.g., updating scoreboard on win/draw), and return DTOs.
   - Minimal API endpoints and controllers MUST NOT contain orchestration or repository calls directly; they MUST inject Application services (`IGameService`) via primary constructors and act as thin HTTP delegators.

5. **Rich Domain Models (No Anemic Entities)**:
   - All core business rules (move validation, turn switching, win/draw detection, board calculation) MUST remain inside Domain entities (`Game.cs`, `Scoreboard.cs`) or Domain services.
   - Application services MUST NOT steal or duplicate domain logic; they only orchestrate calls to the Domain entities.
