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

