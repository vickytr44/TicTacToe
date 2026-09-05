namespace TicTacToe.Domain.Exceptions;

public class InvalidMoveException(string message) : Exception(message);
