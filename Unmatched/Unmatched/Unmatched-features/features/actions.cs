public enum ActionType { Maneuver, Scheme, Attack }

public class TurnManager
{
    private const int ActionsPerTurn = 2;
    public Player Active { get; private set; }
    public int ActionsRemaining { get; private set; }

    public void StartTurn(Player p) { Active = p; ActionsRemaining = ActionsPerTurn; }

    public void PerformAction(ActionType type, GameState state)
    {
        if (ActionsRemaining <= 0) throw new InvalidOperationException();
        ActionsRemaining--;
        if (state.IsGameOver) return;
        if (ActionsRemaining == 0) EndTurn(state);
    }
}

public enum CombatPhase { Declared, AwaitingDefense, Revealed, Immediately, DuringCombat, Damage, AfterCombat, Done }

public class Combat
{
    public Fighter Attacker { get; }
    public Fighter Defender { get; }
    public Card AttackCard { get; set; }
    public Card DefenseCard { get; set; }   // null = no defense
    public CombatPhase Phase { get; private set; } = CombatPhase.Declared;
    public int AttackValue { get; set; }
    public int DefenseValue { get; set; }
    public bool Cancelled { get; set; }     // some effects cancel the attack

    public Combat(Fighter a, Fighter d) { Attacker = a; Defender = d; }
}