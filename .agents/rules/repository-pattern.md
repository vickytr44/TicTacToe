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
