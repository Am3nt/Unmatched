using Unmatched.Core.Cards;
using Unmatched.Core.Fighters;

namespace Unmatched.Core.Battles;

public enum CombatPhase
{
    Declared,       // támadó és célpont kiválasztva
    AwaitingCards,  // mindkét fél lapot választ (titokban, egyszerre)
    Revealed,       // lapok felfordítva
    Immediately,
    DuringCombat,
    Damage,
    AfterCombat,
    Done
}

public class Combat
{
    public Fighter Attacker { get; }
    public Fighter Defender { get; }
    public Card? AttackCard { get; set; }
    public Card? DefenseCard { get; set; }   // null = nincs védekezés
    public CombatPhase Phase { get; internal set; } = CombatPhase.Declared;
    public int AttackValue { get; set; }
    public int DefenseValue { get; set; }
    public int DamageDealt { get; internal set; }
    public bool Cancelled { get; set; }      // egyes effektek megszakítják a támadást

    public Combat(Fighter attacker, Fighter defender)
    {
        Attacker = attacker;
        Defender = defender;
    }
}
