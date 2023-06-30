﻿namespace Codebreaker.GameAPIs.Errors;

public record class GameError(string Code, string Message, string Target, string[]? Details = default);

public class ErrorCodes
{
    public const string InvalidGuessNumber = nameof(InvalidGuessNumber);
    public const string UnexpectedMoveNumber = nameof(UnexpectedMoveNumber);
    public const string InvalidGuess = nameof(InvalidGuess);
    public const string InvalidMove = nameof(InvalidMove);
}
