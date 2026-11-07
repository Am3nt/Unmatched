using Unmatched.Core.Battles;
using Unmatched.Core.Fighters;
using Unmatched.Core.Game;

namespace Unmatched.Core.Cards;

public enum EffectTiming { Immediately, DuringCombat, AfterCombat }

/// <summary>Harci effekt (támadó/védő/versatile lapokon).</summary>
public interface ICardEffect
{
    EffectTiming Timing { get; }
    Task ApplyAsync(Combat combat, GameState state, Fighter owner, IPlayerInput input);
}

/// <summary>Séma effekt: nincs mögötte harc, ezért külön interfész.</summary>
public interface ISchemeEffect
{
    Task ApplyAsync(GameState state, Fighter owner, IPlayerInput input);
}
