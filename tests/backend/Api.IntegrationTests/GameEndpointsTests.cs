namespace TicTacToe.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Application.DTOs;

[Collection("IntegrationTests")]
public class GameEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PostGame_Returns201Created_WithCompleteContract()
    {
        var response = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var game = await response.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(game);
        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal("X", game.CurrentPlayer);
        Assert.Equal("TwoPlayer", game.GameMode);
        Assert.Equal("InProgress", game.Status);
        Assert.Null(game.Winner);
        Assert.Empty(game.WinningCells);
        Assert.Empty(game.Moves);
        Assert.Equal(3, game.Board.Length);
        Assert.Equal(3, game.Board[0].Length);
    }

    [Fact]
    public async Task PostGame_ComputerMode_PersistsAndReturnsComputerGameMode()
    {
        var response = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "Computer" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);
        Assert.Equal("Computer", created.GameMode);
        Assert.Equal("X", created.CurrentPlayer);
        Assert.Equal("InProgress", created.Status);

        // Fetch from database to verify persistence
        var getResponse = await _client.GetAsync($"/api/games/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(fetched);
        Assert.Equal("Computer", fetched.GameMode);
        Assert.Equal(created.Id, fetched.Id);
    }

    [Fact]
    public async Task GetGame_ExistingId_Returns200Ok()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/games/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var game = await getResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(game);
        Assert.Equal(created.Id, game.Id);
    }

    [Fact]
    public async Task GetGame_NonExistentId_Returns404NotFound()
    {
        var response = await _client.GetAsync($"/api/games/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostMove_ValidMove_Returns200Ok_AndUpdatesBoard()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);

        var moveRequest = new MakeMoveRequest { Player = "X", Row = 1, Column = 1 };
        var moveResponse = await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", moveRequest);

        Assert.Equal(HttpStatusCode.OK, moveResponse.StatusCode);

        var updated = await moveResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(updated);
        Assert.Equal("X", updated.Board[0][0]);
        Assert.Equal("O", updated.CurrentPlayer);
        Assert.Single(updated.Moves);
        Assert.Equal(1, updated.Moves[0].MoveNumber);
        Assert.Equal("X", updated.Moves[0].Player);
        Assert.Equal(1, updated.Moves[0].Row);
        Assert.Equal(1, updated.Moves[0].Column);
    }

    [Fact]
    public async Task PostMove_OccupiedCell_Returns400BadRequest()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);

        // Move 1: X to (1,1)
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 1 });

        // Move 2: O tries (1,1)
        var badResponse = await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 1, Column = 1 });

        Assert.Equal(HttpStatusCode.BadRequest, badResponse.StatusCode);
    }

    [Fact]
    public async Task PostMove_WrongPlayer_Returns400BadRequest()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);

        // Turn 1 is X, O tries to move
        var badResponse = await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 1, Column = 1 });

        Assert.Equal(HttpStatusCode.BadRequest, badResponse.StatusCode);
    }

    [Fact]
    public async Task PostMove_OutOfBounds_Returns400BadRequest()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);

        var badResponse = await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 4, Column = 1 });

        Assert.Equal(HttpStatusCode.BadRequest, badResponse.StatusCode);
    }

    [Fact]
    public async Task PostMove_AfterWin_Returns400BadRequest()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);

        // Play sequence for X row 1 win
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 2 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 2 });
        var winResponse = await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 3 });

        var wonGame = await winResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(wonGame);
        Assert.Equal("Won", wonGame.Status);
        Assert.Equal("X", wonGame.Winner);

        // Attempt move after win
        var badResponse = await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 3, Column = 3 });
        Assert.Equal(HttpStatusCode.BadRequest, badResponse.StatusCode);
    }

    [Fact]
    public async Task PostMove_FullBoardDraw_Returns200Ok_WithDrawStatus_AndRejectsFurtherMoves()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);

        // Sequence producing Draw:
        // (1,1)[X], (1,2)[O], (1,3)[X]
        // (2,1)[X], (2,2)[O], (2,3)[O]
        // (3,1)[O], (3,2)[X], (3,3)[X]
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 1, Column = 2 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 3 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 2 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 2, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 3 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 3, Column = 2 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 3, Column = 1 });
        var drawResponse = await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 3, Column = 3 });

        Assert.Equal(HttpStatusCode.OK, drawResponse.StatusCode);
        var drawGame = await drawResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(drawGame);
        Assert.Equal("Draw", drawGame.Status);
        Assert.Null(drawGame.Winner);
        Assert.Empty(drawGame.WinningCells);
        Assert.Equal(9, drawGame.Moves.Count);

        // Subsequent move must be rejected
        var afterDrawResponse = await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 1, Column = 1 });
        Assert.Equal(HttpStatusCode.BadRequest, afterDrawResponse.StatusCode);
    }

    [Fact]
    public async Task PostReset_CompletedGame_Returns200Ok_ResetsBoard_AndPreservesScoreboard()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);

        // Play moves for X to win
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 2 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 2 });
        var winResponse = await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 3 });
        Assert.Equal(HttpStatusCode.OK, winResponse.StatusCode);

        // Verify scoreboard recorded the win
        using var scopeBefore = factory.Services.CreateScope();
        var scoreboardRepo = scopeBefore.ServiceProvider.GetRequiredService<TicTacToe.Domain.Repositories.IScoreboardRepository>();
        var scoreboardBefore = await scoreboardRepo.GetScoreboardAsync();
        var xWinsBefore = scoreboardBefore.XWins;
        Assert.True(xWinsBefore >= 1);

        // Reset the completed game
        var resetResponse = await _client.PostAsync($"/api/games/{created.Id}/reset", null);
        Assert.Equal(HttpStatusCode.OK, resetResponse.StatusCode);

        var resetGame = await resetResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(resetGame);
        Assert.Equal(created.Id, resetGame.Id);
        Assert.Equal("X", resetGame.CurrentPlayer);
        Assert.Equal("InProgress", resetGame.Status);
        Assert.Null(resetGame.Winner);
        Assert.Empty(resetGame.WinningCells);
        Assert.Empty(resetGame.Moves);
        Assert.Equal("TwoPlayer", resetGame.GameMode);
        Assert.All(resetGame.Board, row => Assert.All(row, Assert.Null));

        // Scoreboard must be preserved across game resets
        using var scopeAfter = factory.Services.CreateScope();
        var scoreboardRepoAfter = scopeAfter.ServiceProvider.GetRequiredService<TicTacToe.Domain.Repositories.IScoreboardRepository>();
        var scoreboardAfter = await scoreboardRepoAfter.GetScoreboardAsync();
        Assert.Equal(xWinsBefore, scoreboardAfter.XWins);
    }

    [Fact]
    public async Task PostReset_NonExistentGame_Returns404NotFound()
    {
        var response = await _client.PostAsync($"/api/games/{Guid.NewGuid()}/reset", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostReset_InProgressGame_Returns200Ok_AndClearsMoves()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);

        // Make one move
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 2, Column = 2 });

        // Reset in-progress game
        var resetResponse = await _client.PostAsync($"/api/games/{created.Id}/reset", null);
        Assert.Equal(HttpStatusCode.OK, resetResponse.StatusCode);

        var resetGame = await resetResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(resetGame);
        Assert.Equal("X", resetGame.CurrentPlayer);
        Assert.Equal("InProgress", resetGame.Status);
        Assert.Empty(resetGame.Moves);
        Assert.All(resetGame.Board, row => Assert.All(row, Assert.Null));
    }

    [Fact]
    public async Task PostUndo_ValidMove_Returns200Ok_AndRemovesLastMove()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);

        // Move 1: X at (1,1)
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 1 });
        // Move 2: O at (2,2)
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 2 });

        var undoResponse = await _client.PostAsync($"/api/games/{created.Id}/undo", null);
        Assert.Equal(HttpStatusCode.OK, undoResponse.StatusCode);

        var undoneGame = await undoResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(undoneGame);
        Assert.Equal("X", undoneGame.Board[0][0]);
        Assert.Null(undoneGame.Board[1][1]);
        Assert.Equal("O", undoneGame.CurrentPlayer);
        Assert.Single(undoneGame.Moves);
        Assert.Equal("X", undoneGame.Moves[0].Player);
        Assert.Equal("InProgress", undoneGame.Status);
    }

    [Fact]
    public async Task PostUndo_NoMoves_Returns400BadRequest()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);

        var undoResponse = await _client.PostAsync($"/api/games/{created.Id}/undo", null);
        Assert.Equal(HttpStatusCode.BadRequest, undoResponse.StatusCode);
    }

    [Fact]
    public async Task PostUndo_AfterWin_Returns400BadRequest()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);

        // Complete win for X
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 2 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 2 });
        await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 3 });

        var undoResponse = await _client.PostAsync($"/api/games/{created.Id}/undo", null);
        Assert.Equal(HttpStatusCode.BadRequest, undoResponse.StatusCode);
    }

    [Fact]
    public async Task PostUndo_NonExistentGame_Returns404NotFound()
    {
        var response = await _client.PostAsync($"/api/games/{Guid.NewGuid()}/undo", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostMove_ComputerMode_AutomaticallyExecutesComputerTurn_ReturnsBothMoves()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "Computer" });
        var created = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(created);
        Assert.Equal("Computer", created.GameMode);

        var moveResponse = await _client.PostAsJsonAsync($"/api/games/{created.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 1 });
        Assert.Equal(HttpStatusCode.OK, moveResponse.StatusCode);

        var updated = await moveResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(updated);
        Assert.Equal(2, updated.Moves.Count);

        // First move is X at (1,1)
        Assert.Equal("X", updated.Moves[0].Player);
        Assert.Equal(1, updated.Moves[0].Row);
        Assert.Equal(1, updated.Moves[0].Column);

        // Second move is O taking center (2,2)
        Assert.Equal("O", updated.Moves[1].Player);
        Assert.Equal(2, updated.Moves[1].Row);
        Assert.Equal(2, updated.Moves[1].Column);

        // Board has both marks
        Assert.Equal("X", updated.Board[0][0]);
        Assert.Equal("O", updated.Board[1][1]);

        // Turn returned to X
        Assert.Equal("X", updated.CurrentPlayer);
        Assert.Equal("InProgress", updated.Status);
    }
}
