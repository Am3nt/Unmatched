using Unmatched.Core.Actions;
using Unmatched.Core.Board;
using Unmatched.Core.Cards;
using Unmatched.Core.Fighters;
using Unmatched.Core.Game;
using Unmatched.Core.Players;

namespace Unmatched.Input;

/// <summary>Egyszerű konzolos bemenet tesztelésre (hot-seat; a lapok titkossága itt nem biztosított).</summary>
public class ConsolePlayerInput : IPlayerInput
{
    private static T Pick<T>(string prompt, IReadOnlyList<T> options, bool optional = false)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine(prompt);
            for (int i = 0; i < options.Count; i++)
                Console.WriteLine($"  {i + 1}) {options[i]}");
            if (optional) Console.WriteLine("  0) (nincs / kihagyom)");
            Console.Write("> ");

            if (int.TryParse(Console.ReadLine(), out int n))
            {
                if (optional && n == 0) return default!;
                if (n >= 1 && n <= options.Count) return options[n - 1];
            }
            Console.WriteLine("Érvénytelen választás.");
        }
    }

    public Task<IGameAction> ChooseActionAsync(Player player, IReadOnlyList<IGameAction> legal)
    {
        Console.WriteLine();
        Console.WriteLine($"[{player.Name}] Kéz: {string.Join(", ", player.Hand)}");
        foreach (var f in player.Fighters) Console.WriteLine($"  {f} @ {f.Space}");
        return Task.FromResult(Pick($"{player.Name}, válassz akciót:", legal));
    }

    public Task<Fighter> ChooseFighterAsync(Player player, IReadOnlyList<Fighter> options, string prompt) =>
        Task.FromResult(Pick($"{player.Name}: {prompt}", options));

    public Task<Fighter> ChooseTargetAsync(Player player, Fighter attacker, IReadOnlyList<Fighter> targets) =>
        Task.FromResult(Pick($"{player.Name}: ki a célpont?", targets));

    public Task<Space?> ChooseSpaceAsync(Player player, Fighter fighter, IReadOnlyList<Space> options) =>
        Task.FromResult<Space?>(Pick($"{player.Name}: hová lép {fighter.Name}?", options, optional: true));

    public Task<Card> ChooseAttackCardAsync(Player player, Fighter attacker, Fighter defender, IReadOnlyList<Card> legal) =>
        Task.FromResult(Pick($"{player.Name}: támadó lap ({attacker.Name} → {defender.Name})", legal));

    public Task<Card?> ChooseDefenseCardAsync(Player player, Fighter defender, Fighter attacker, IReadOnlyList<Card> legal) =>
        Task.FromResult<Card?>(Pick($"{player.Name}: védő lap ({defender.Name})", legal, optional: true));

    public Task<Card> ChooseCardAsync(Player player, IReadOnlyList<Card> options, string prompt) =>
        Task.FromResult(Pick($"{player.Name}: {prompt}", options));

    public Task<Card?> ChooseBoostCardAsync(Player player, IReadOnlyList<Card> hand) =>
        Task.FromResult<Card?>(Pick($"{player.Name}: eldobsz egy lapot boostra?", hand, optional: true));
}
