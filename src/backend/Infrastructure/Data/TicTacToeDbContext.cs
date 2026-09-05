namespace TicTacToe.Infrastructure.Data;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TicTacToe.Domain.Entities;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.ValueObjects;

public class TicTacToeDbContext(DbContextOptions<TicTacToeDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Scoreboard> Scoreboards => Set<Scoreboard>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var jsonOptions = (JsonSerializerOptions?)null;

        var boardComparer = new ValueComparer<Player?[][]>(
            (c1, c2) => JsonSerializer.Serialize(c1, jsonOptions) == JsonSerializer.Serialize(c2, jsonOptions),
            c => JsonSerializer.Serialize(c, jsonOptions).GetHashCode(),
            c => JsonSerializer.Deserialize<Player?[][]>(JsonSerializer.Serialize(c, jsonOptions), jsonOptions)!
        );

        var winningCellsComparer = new ValueComparer<List<CellPosition>>(
            (c1, c2) => JsonSerializer.Serialize(c1, jsonOptions) == JsonSerializer.Serialize(c2, jsonOptions),
            c => JsonSerializer.Serialize(c, jsonOptions).GetHashCode(),
            c => JsonSerializer.Deserialize<List<CellPosition>>(JsonSerializer.Serialize(c, jsonOptions), jsonOptions)!
        );

        var movesComparer = new ValueComparer<List<Move>>(
            (c1, c2) => JsonSerializer.Serialize(c1, jsonOptions) == JsonSerializer.Serialize(c2, jsonOptions),
            c => JsonSerializer.Serialize(c, jsonOptions).GetHashCode(),
            c => JsonSerializer.Deserialize<List<Move>>(JsonSerializer.Serialize(c, jsonOptions), jsonOptions)!
        );

        modelBuilder.Entity<Game>(b =>
        {
            b.HasKey(g => g.Id);

            b.Property(g => g.CurrentPlayer)
                .HasConversion<string>();

            b.Property(g => g.GameMode)
                .HasConversion<string>();

            b.Property(g => g.Status)
                .HasConversion<string>();

            b.Property(g => g.Winner)
                .HasConversion<string>();

            b.Property(g => g.Board)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<Player?[][]>(v, jsonOptions) ?? new Player?[3][]
                )
                .Metadata.SetValueComparer(boardComparer);

            b.Property(g => g.WinningCells)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<List<CellPosition>>(v, jsonOptions) ?? new List<CellPosition>()
                )
                .Metadata.SetValueComparer(winningCellsComparer);

            b.Property(g => g.Moves)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<List<Move>>(v, jsonOptions) ?? new List<Move>()
                )
                .Metadata.SetValueComparer(movesComparer);
        });

        modelBuilder.Entity<Scoreboard>(b =>
        {
            b.HasKey(s => s.Id);
            b.HasData(new { Id = 1, XWins = 0, OWins = 0, Draws = 0 });
        });
    }
}
