namespace TicTacToe.Application.Services;

using TicTacToe.Application.DTOs;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Repositories;

public class ScoreboardService(IScoreboardRepository scoreboardRepository) : IScoreboardService
{
    public async Task<ScoreboardResponse> GetScoreboardAsync(CancellationToken cancellationToken = default)
    {
        var scoreboard = await scoreboardRepository.GetScoreboardAsync(cancellationToken);
        return new ScoreboardResponse(scoreboard.XWins, scoreboard.OWins, scoreboard.Draws);
    }

    public async Task<ScoreboardResponse> ResetScoreboardAsync(CancellationToken cancellationToken = default)
    {
        await scoreboardRepository.ResetAsync(cancellationToken);
        var scoreboard = await scoreboardRepository.GetScoreboardAsync(cancellationToken);
        return new ScoreboardResponse(scoreboard.XWins, scoreboard.OWins, scoreboard.Draws);
    }

    public async Task RecordWinAsync(Player winner, CancellationToken cancellationToken = default)
    {
        var scoreboard = await scoreboardRepository.GetScoreboardAsync(cancellationToken);
        scoreboard.RecordWin(winner);
        await scoreboardRepository.UpdateAsync(scoreboard, cancellationToken);
    }

    public async Task RecordDrawAsync(CancellationToken cancellationToken = default)
    {
        var scoreboard = await scoreboardRepository.GetScoreboardAsync(cancellationToken);
        scoreboard.RecordDraw();
        await scoreboardRepository.UpdateAsync(scoreboard, cancellationToken);
    }
}
