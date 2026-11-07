using Unmatched.Core.Board;
using Unmatched.Core.Cards;
using Unmatched.Core.Fighters;
using Unmatched.Core.Players;

namespace Unmatched.Core.Game;

public class GameState
{
    private const int ExhaustionDamage = 2;   // SZABÁLY-ELLENŐRZÉS: mi történik üres pakli esetén

    public GameBoard Board { get; }
    public IReadOnlyList<Player> Players { get; }

    public event Action<string>? Logged;

    public GameState(GameBoard board, IEnumerable<Player> players)
    {
        Board = board;
        Players = players.ToList();
    }

    private static bool HasLost(Player p) => p.Fighters.Any(f => f.EndsGameWhenDefeated && f.IsDefeated);

    public bool IsGameOver => Players.Any(HasLost);
    public Player? Winner => IsGameOver ? Players.FirstOrDefault(p => !HasLost(p)) : null;

    public void Log(string message) => Logged?.Invoke(message);

    /// <summary>A kéz a játékoshoz tartozik (a sidekickek is a hős játékosának kezéből játszanak).</summary>
    public List<Card> Hand(Fighter f) => f.Owner.Hand;

    public void DealDamage(Fighter target, int amount)
    {
        if (amount <= 0 || target.IsDefeated) return;
        int dealt = target.TakeDamage(amount);
        Log($"{target.Name} {dealt} sebzést kap ({target.CurrentHealth}/{target.MaxHealth}).");
        if (target.IsDefeated)
        {
            Log($"{target.Name} elesett!");
            Board.Remove(target);
        }
    }

    public void Heal(Fighter target, int amount)
    {
        int healed = target.Heal(amount);
        if (healed > 0) Log($"{target.Name} gyógyul +{healed} ({target.CurrentHealth}/{target.MaxHealth}).");
    }

    public void Draw(Player player, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (player.DrawPile.Count == 0)
            {
                Log($"{player.Name} paklija elfogyott!");
                DealDamage(player.Hero, ExhaustionDamage);
                continue;
            }
            var card = player.DrawPile[^1];
            player.DrawPile.RemoveAt(player.DrawPile.Count - 1);
            player.Hand.Add(card);
        }
    }

    public void Discard(Player player, Card card)
    {
        player.Hand.Remove(card);
        player.DiscardPile.Add(card);
    }

    public void Move(Fighter f, Space target)
    {
        Board.Place(f, target);
        Log($"{f.Name} a(z) {target} mezőre lép.");
    }

    /// <summary>Mozgás kérése (pl. effektből): a játékos választ az elérhető mezők közül, vagy marad.</summary>
    public async Task RequestMoveAsync(Fighter f, int maxSpaces, IPlayerInput input)
    {
        if (f.IsDefeated) return;
        var reachable = Board.ReachableSpaces(f, maxSpaces);
        if (reachable.Count == 0) return;

        var chosen = await input.ChooseSpaceAsync(f.Owner, f, reachable);
        if (chosen == null) return;
        if (!reachable.Contains(chosen))
            throw new InvalidOperationException("Érvénytelen célmező.");
        Move(f, chosen);
    }
}
