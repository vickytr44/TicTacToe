namespace TicTacToe.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TicTacToe.Application.DTOs;

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
}
