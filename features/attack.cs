public enum CombatPhase { Declared, AwaitingDefense, Revealed, Immediately, DuringCombat, Damage, AfterCombat, Done }

public class Combat
{
    public Fighter Attacker { get; }
    public Fighter Defender { get; }
    public Card AttackCard { get; set; }
    public Card DefenseCard { get; set; }   
    public CombatPhase Phase { get; private set; } = CombatPhase.Declared;
    public int AttackValue { get; set; }
    public int DefenseValue { get; set; }
    public bool Cancelled { get; set; }    

    public Combat(Fighter a, Fighter d) { Attacker = a; Defender = d; }
}