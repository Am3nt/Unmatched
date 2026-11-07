using Unmatched.Core.Battles;
using Unmatched.Core.Fighters;
using Unmatched.Core.Game;
using Unmatched.Core.Players;

namespace Unmatched.Core.Actions;

public class AttackAction : IGameAction
{
    private readonly CombatResolver _resolver;

    public AttackAction(CombatResolver? resolver = null) { _resolver = resolver ?? new CombatResolver(); }

    public ActionType Type => ActionType.Attack;
    public string Name => "Támadás";
    public override string ToString() => Name;

    private IEnumerable<Fighter> PossibleAttackers(GameState state, Player player) =>
        player.AliveFighters.Where(f => state.Board.TargetsFor(f).Count > 0);

    public bool CanExecute(GameState state, Player player) =>
        _resolver.LegalAttackCards(player.Hero).Count > 0 && PossibleAttackers(state, player).Any();

    public async Task ExecuteAsync(GameState state, Player player, IPlayerInput input)
    {
        var attackers = PossibleAttackers(state, player).ToList();
        if (attackers.Count == 0 || _resolver.LegalAttackCards(player.Hero).Count == 0)
            throw new InvalidOperationException("A támadás most nem hajtható végre.");

        var attacker = attackers.Count == 1
            ? attackers[0]
            : await input.ChooseFighterAsync(player, attackers, "Melyik harcosoddal támadsz?");
        if (!attackers.Contains(attacker)) throw new InvalidOperationException("Érvénytelen támadó.");

        var targets = state.Board.TargetsFor(attacker);
        var target = targets.Count == 1
            ? targets[0]
            : await input.ChooseTargetAsync(player, attacker, targets);
        if (!targets.Contains(target)) throw new InvalidOperationException("Érvénytelen célpont.");

        var combat = new Combat(attacker, target);
        await _resolver.ResolveAsync(combat, state, input);
    }
}
