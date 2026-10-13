public enum CombatPhase { Declared, AwaitingDefense, Revealed, Immediately, DuringCombat, Damage, AfterCombat, Done }

public class Combat
{
    public Fighter Attacker { get; } //get the attackers
    public Fighter Defender { get; } //get the defender
    public Card AttackCard { get; set; } //get the cards, and time of it to take effect
    public Card DefenseCard { get; set; }   // null = no defense
    public CombatPhase Phase { get; private set; } = CombatPhase.Declared;
    public int AttackValue { get; set; }
    public int DefenseValue { get; set; }
    public bool Cancelled { get; set; }     // some effects cancel the attack

    public Combat(Fighter a, Fighter d) { Attacker = a; Defender = d; }
}