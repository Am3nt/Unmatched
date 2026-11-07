using Unmatched.Core.Players;

namespace Unmatched.Core.Cards;

public enum CardType { Attack, Defense, Versatile, Scheme }

public class Card
{
    public string Name { get; init; } = "";
    public CardType Type { get; init; }
    public int Value { get; init; }
    public int Boost { get; init; }

    /// <summary>Harci effektek (Attack / Defense / Versatile lapokon).</summary>
    public IReadOnlyList<ICardEffect> Effects { get; init; } = Array.Empty<ICardEffect>();

    /// <summary>Séma effektek (Scheme lapokon).</summary>
    public IReadOnlyList<ISchemeEffect> SchemeEffects { get; init; } = Array.Empty<ISchemeEffect>();

    public Player? Owner { get; set; }

    public bool CanAttackWith => Type is CardType.Attack or CardType.Versatile;
    public bool CanDefendWith => Type is CardType.Defense or CardType.Versatile;

    public override string ToString() => Type == CardType.Scheme
        ? $"{Name} [Scheme, boost {Boost}]"
        : $"{Name} [{Type} {Value}, boost {Boost}]";
}
