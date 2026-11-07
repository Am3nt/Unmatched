using Unmatched.Core.Battles;
using Unmatched.Core.Fighters;
using Unmatched.Core.Game;

namespace Unmatched.Core.Cards;

// Példa effektek. Minden hős egyedi képessége hasonló osztály lehet.

/// <summary>Harc után a lap tulajdonosa mozoghat legfeljebb N mezőt.</summary>
public class MoveAfterCombat : ICardEffect
{
    private readonly int _spaces;
    public MoveAfterCombat(int spaces = 2) { _spaces = spaces; }

    public EffectTiming Timing => EffectTiming.AfterCombat;

    public Task ApplyAsync(Combat c, GameState s, Fighter owner, IPlayerInput input) =>
        s.RequestMoveAsync(owner, _spaces, input);
}

/// <summary>Támadóként a támadó, védőként a védő értékét növeli.</summary>
public class BonusValue : ICardEffect
{
    private readonly int _amount;
    public EffectTiming Timing { get; }

    public BonusValue(int amount, EffectTiming timing = EffectTiming.DuringCombat)
    {
        _amount = amount;
        Timing = timing;
    }

    public Task ApplyAsync(Combat c, GameState s, Fighter owner, IPlayerInput input)
    {
        if (owner == c.Attacker) c.AttackValue += _amount;
        else c.DefenseValue += _amount;
        s.Log($"{owner.Name} lapja +{_amount} értéket ad.");
        return Task.CompletedTask;
    }
}

/// <summary>A támadás azonnal megszakad.</summary>
public class CancelAttack : ICardEffect
{
    public EffectTiming Timing => EffectTiming.Immediately;

    public Task ApplyAsync(Combat c, GameState s, Fighter owner, IPlayerInput input)
    {
        c.Cancelled = true;
        s.Log("A támadás megszakadt!");
        return Task.CompletedTask;
    }
}

/// <summary>Séma: húzz N lapot.</summary>
public class DrawCardsScheme : ISchemeEffect
{
    private readonly int _count;
    public DrawCardsScheme(int count) { _count = count; }

    public Task ApplyAsync(GameState s, Fighter owner, IPlayerInput input)
    {
        s.Draw(owner.Owner, _count);
        return Task.CompletedTask;
    }
}

/// <summary>Séma: a lap használója gyógyul N-et.</summary>
public class HealScheme : ISchemeEffect
{
    private readonly int _amount;
    public HealScheme(int amount) { _amount = amount; }

    public Task ApplyAsync(GameState s, Fighter owner, IPlayerInput input)
    {
        s.Heal(owner, _amount);
        return Task.CompletedTask;
    }
}
