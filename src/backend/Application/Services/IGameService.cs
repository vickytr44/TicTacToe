namespace TicTacToe.Application.Services;

using TicTacToe.Application.DTOs;

public interface IGameService
{
    Task<GameResponse> CreateGameAsync(CreateGameRequest request, CancellationToken cancellationToken = default);
    Task<GameResponse> GetGameByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GameResponse> MakeMoveAsync(Guid id, MakeMoveRequest request, CancellationToken cancellationToken = default);
    Task<GameResponse> ResetGameAsync(Guid id, CancellationToken cancellationToken = default);
}
