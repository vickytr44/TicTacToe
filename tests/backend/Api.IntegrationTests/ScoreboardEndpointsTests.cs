namespace TicTacToe.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TicTacToe.Application.DTOs;
using Xunit;

[Collection("IntegrationTests")]
public class ScoreboardEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetScoreboard_Returns200Ok_WithScoreboardContract()
    {
        // Act
        var response = await _client.GetAsync("/api/scoreboard");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var scoreboard = await response.Content.ReadFromJsonAsync<ScoreboardResponse>();
        Assert.NotNull(scoreboard);
        Assert.True(scoreboard.XWins >= 0);
        Assert.True(scoreboard.OWins >= 0);
        Assert.True(scoreboard.Draws >= 0);
    }

    [Fact]
    public async Task PostScoreboardReset_Returns200Ok_WithZeroCounts()
    {
        // Act
        var response = await _client.PostAsync("/api/scoreboard/reset", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var scoreboard = await response.Content.ReadFromJsonAsync<ScoreboardResponse>();
        Assert.NotNull(scoreboard);
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public async Task GameWin_UpdatesScoreboard_ExactlyOnce()
    {
        // Reset scoreboard first for a clean state
        await _client.PostAsync("/api/scoreboard/reset", null);

        // Create game
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var game = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(game);

        // Play winning sequence for X (Row 1):
        // X: (1,1), O: (2,1)
        // X: (1,2), O: (2,2)
        // X: (1,3) -> Win!
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 2 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 2 });
        var winningMoveResponse = await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 3 });

        var wonGame = await winningMoveResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(wonGame);
        Assert.Equal("Won", wonGame.Status);
        Assert.Equal("X", wonGame.Winner);

        // Verify scoreboard has 1 win for X
        var sbResponse = await _client.GetAsync("/api/scoreboard");
        var scoreboard = await sbResponse.Content.ReadFromJsonAsync<ScoreboardResponse>();
        Assert.NotNull(scoreboard);
        Assert.Equal(1, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public async Task GameDraw_UpdatesScoreboard_ExactlyOnce()
    {
        // Reset scoreboard first
        await _client.PostAsync("/api/scoreboard/reset", null);

        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var game = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(game);

        // Play draw sequence:
        // X O X
        // X X O
        // O X O
        // (1,1)=X, (1,2)=O, (1,3)=X, (2,1)=X, (2,3)=O, (2,2)=X, (3,1)=O, (3,2)=X, (3,3)=O
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "O", Row = 1, Column = 2 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 3 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 3 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "X", Row = 2, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "O", Row = 3, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "X", Row = 2, Column = 2 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "O", Row = 3, Column = 3 });
        var drawMoveResponse = await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "X", Row = 3, Column = 2 });

        var drawGame = await drawMoveResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(drawGame);
        Assert.Equal("Draw", drawGame.Status);

        var sbResponse = await _client.GetAsync("/api/scoreboard");
        var scoreboard = await sbResponse.Content.ReadFromJsonAsync<ScoreboardResponse>();
        Assert.NotNull(scoreboard);
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(1, scoreboard.Draws);
    }

    [Fact]
    public async Task GameReset_PreservesScoreboard()
    {
        // Reset scoreboard first
        await _client.PostAsync("/api/scoreboard/reset", null);

        // Win a game for X
        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var game = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(game);

        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 2 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "O", Row = 2, Column = 2 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 3 });

        // Reset the game
        var resetResponse = await _client.PostAsync($"/api/games/{game.Id}/reset", null);
        Assert.Equal(HttpStatusCode.OK, resetResponse.StatusCode);

        // Verify scoreboard still has 1 win for X
        var sbResponse = await _client.GetAsync("/api/scoreboard");
        var scoreboard = await sbResponse.Content.ReadFromJsonAsync<ScoreboardResponse>();
        Assert.NotNull(scoreboard);
        Assert.Equal(1, scoreboard.XWins);
    }

    [Fact]
    public async Task IncompleteGame_DoesNotAffectScoreboard()
    {
        // Reset scoreboard first
        await _client.PostAsync("/api/scoreboard/reset", null);

        var createResponse = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest { Mode = "TwoPlayer" });
        var game = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
        Assert.NotNull(game);

        // Make non-terminal moves
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "X", Row = 1, Column = 1 });
        await _client.PostAsJsonAsync($"/api/games/{game.Id}/moves", new MakeMoveRequest { Player = "O", Row = 1, Column = 2 });

        // Verify scoreboard remains 0
        var sbResponse = await _client.GetAsync("/api/scoreboard");
        var scoreboard = await sbResponse.Content.ReadFromJsonAsync<ScoreboardResponse>();
        Assert.NotNull(scoreboard);
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }
}
