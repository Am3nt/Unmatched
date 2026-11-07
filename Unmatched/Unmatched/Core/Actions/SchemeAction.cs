using Unmatched.Core.Cards;
using Unmatched.Core.Game;
using Unmatched.Core.Players;

namespace Unmatched.Core.Actions;

public class SchemeAction : IGameAction
{
    public ActionType Type => ActionType.Scheme;
    public string Name => "Séma";
    public override string ToString() => Name;

    private static List<Card> SchemeCards(Player p) => p.Hand.Where(c => c.Type == CardType.Scheme).ToList();

    public bool CanExecute(GameState state, Player player) => SchemeCards(player).Count > 0;

    public async Task ExecuteAsync(GameState state, Player player, IPlayerInput input)
    {
        var options = SchemeCards(player);
        if (options.Count == 0) throw new InvalidOperationException("Nincs séma lapod.");

        var card = await input.ChooseCardAsync(player, options, "Melyik sémát játszod ki?");
        if (!options.Contains(card)) throw new InvalidOperationException("Érvénytelen séma lap.");

        player.Hand.Remove(card);
        state.Log($"{player.Name} kijátssza: {card.Name}.");

        // SZABÁLY-ELLENŐRZÉS: melyik harcos a séma "tulajdonosa" – itt a hős.
        foreach (var effect in card.SchemeEffects)
            await effect.ApplyAsync(state, player.Hero, input);

        player.DiscardPile.Add(card);
    }
}
