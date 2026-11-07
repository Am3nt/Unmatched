using Unmatched.Core.Actions;
using Unmatched.Core.Players;

namespace Unmatched.Core.Game;

public class TurnManager
{
    private const int ActionsPerTurn = 2;
    private readonly IReadOnlyList<IGameAction> _actions;

    public Player? Active { get; private set; }
    public int ActionsRemaining { get; private set; }

    public TurnManager(IEnumerable<IGameAction>? actions = null)
    {
        _actions = (actions ?? new IGameAction[]
        {
            new ManeuverAction(), new SchemeAction(), new AttackAction()
        }).ToList();
    }

    public async Task PlayTurnAsync(Player player, GameState state, IPlayerInput input)
    {
        Active = player;
        ActionsRemaining = ActionsPerTurn;
        state.Log($"--- {player.Name} köre ---");

        while (ActionsRemaining > 0 && !state.IsGameOver)
        {
            var legal = _actions.Where(a => a.CanExecute(state, player)).ToList();
            if (legal.Count == 0) break;

            var chosen = await input.ChooseActionAsync(player, legal);
            if (!legal.Contains(chosen)) throw new InvalidOperationException("Érvénytelen akció.");

            await chosen.ExecuteAsync(state, player, input);
            ActionsRemaining--;   // csak sikeres végrehajtás után csökken
        }

        Active = null;
    }

    /// <summary>Körök egymás után, amíg valamelyik hős el nem esik. Visszaadja a győztest.</summary>
    public async Task<Player?> RunGameAsync(GameState state, IPlayerInput input)
    {
        int turn = 0;
        while (!state.IsGameOver)
        {
            await PlayTurnAsync(state.Players[turn % state.Players.Count], state, input);
            turn++;
        }
        state.Log($"Játék vége! Győztes: {state.Winner?.Name}");
        return state.Winner;
    }
}
