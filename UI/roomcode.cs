namespace Unmatched.UI.Services;

/// <summary>Room codes are 5 characters, without look-alikes (no 0/O or 1/I).</summary>
public static class RoomCode
{
    public const int Length = 5;
    public const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static string Generate() =>
        new(Enumerable.Range(0, Length).Select(_ => Alphabet[Random.Shared.Next(Alphabet.Length)]).ToArray());

    public static bool IsValid(string? code) =>
        code is { Length: Length } && code.All(c => Alphabet.Contains(c));
}