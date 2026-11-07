using Unmatched.Core.Game;
using Unmatched.Core.Players;

namespace Unmatched.Core.Actions;

public class ManeuverAction : IGameAction
{
    public ActionType Type => ActionType.Maneuver;
    public string Name => "Manőver";
    public override string ToString() => Name;

    public bool CanExecute(GameState state, Player player) => true;

    public async Task ExecuteAsync(GameState state, Player player, IPlayerInput input)
    {
        // 1) Húzás (SZABÁLY-ELLENŐRZÉS: sorrend; kézlimit = Hero.MaxHandSize, ha az nálatok a kézlimit)
        state.Draw(player, 1);
        while (player.Hand.Count > player.Hero.MaxHandSize)
        {
            var toDiscard = await input.ChooseCardAsync(player, player.Hand.ToList(), "Kézlimit: dobj el egy lapot.");
            if (!player.Hand.Contains(toDiscard)) throw new InvalidOperationException("Érvénytelen lap.");
            state.Discard(player, toDiscard);
        }
        if (state.IsGameOver) return;

        // 2) Opcionális boost: egy lap eldobása, a boost értéke hozzáadódik a mozgáshoz
        int boost = 0;
        if (player.Hand.Count > 0)
        {
            var boostCard = await input.ChooseBoostCardAsync(player, player.Hand.ToList());
            if (boostCard != null)
            {
                if (!player.Hand.Contains(boostCard)) throw new InvalidOperationException("Érvénytelen lap.");
                boost = boostCard.Boost;
                state.Discard(player, boostCard);
                state.Log($"{player.Name} boostol: +{boost} mozgás ({boostCard.Name}).");
            }
        }

        // 3) Minden harcos mozoghat (tetszőleges sorrendben, legfeljebb a mozgásértékéig)
        var remaining = player.AliveFighters.ToList();
        while (remaining.Count > 0)
        {
            var fighter = remaining.Count == 1
                ? remaining[0]
                : await input.ChooseFighterAsync(player, remaining, "Melyik harcosoddal mozogsz?");
            if (!remaining.Contains(fighter)) throw new InvalidOperationException("Érvénytelen harcos.");
            remaining.Remove(fighter);

            await state.RequestMoveAsync(fighter, fighter.MoveValue + boost, input);
        }
    }
}
