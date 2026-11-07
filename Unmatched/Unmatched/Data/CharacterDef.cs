using Unmatched.Core.Cards;

namespace Unmatched.Data;

// Adatleírók: egy karakter (hős + sidekickek + pakli) kódként vagy később JSON-ból megadva.
// Ez váltja ki a Character.cs vázlatot (AmountOfSidekicks -> SidekickDef.Count, CardDeck -> CardDef lista).

public record FighterDef(string Name, int Health, int MoveValue, bool IsRanged);

/// <summary>Count: hány figura tartozik ide (pl. sidekick-csoport). Több figura esetén "Név 1", "Név 2"...</summary>
public record SidekickDef(FighterDef Stats, int Count = 1);

/// <summary>Effects/SchemeEffects Func, hogy minden lappéldány saját effekt-példányokat kapjon.</summary>
public record CardDef(
    string Name,
    CardType Type,
    int Value,
    int Boost,
    int Count,
    Func<ICardEffect[]>? Effects = null,
    Func<ISchemeEffect[]>? SchemeEffects = null);

public record CharacterDef(
    string Name,
    FighterDef Hero,
    int MaxHandSize,
    SidekickDef Sidekick,
    IReadOnlyList<CardDef> Deck);
