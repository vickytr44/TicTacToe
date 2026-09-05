namespace TicTacToe.Domain.Entities;

using TicTacToe.Domain.Enums;

public class Scoreboard
{
    public int Id { get; set; } = 1;
    public int XWins { get; private set; }
    public int OWins { get; private set; }
    public int Draws { get; private set; }

    public Scoreboard() { }

    public Scoreboard(int xWins, int oWins, int draws)
    {
        XWins = xWins;
        OWins = oWins;
        Draws = draws;
    }

    public void RecordWin(Player winner)
    {
        if (winner == Player.X)
        {
            XWins++;
        }
        else
        {
            OWins++;
        }
    }

    public void RecordDraw()
    {
        Draws++;
    }

    public void Reset()
    {
        XWins = 0;
        OWins = 0;
        Draws = 0;
    }
}
