using Unmatched.Core.Cards;
using Unmatched.Core.Fighters;

namespace Unmatched.Core.Players;

// Ha nálatok már van Player osztály, ezt ne másold be: csak a jelölt tagokat egészítsd ki.
public class Player
{
    private Hero? _hero;

    public string Name { get; }

    // --- szükséges tagok ---
    public Hero Hero => _hero ?? throw new InvalidOperationException($"{Name} játékosnak nincs hőse.");
    public IEnumerable<Fighter> Fighters => _hero == null
        ? Enumerable.Empty<Fighter>()
        : new Fighter[] { _hero }.Concat(_hero.Sidekicks);
    public IEnumerable<Fighter> AliveFighters => Fighters.Where(f => !f.IsDefeated);

    public List<Card> Hand { get; } = new();
    public List<Card> DrawPile { get; } = new();     // a lista vége a pakli teteje
    public List<Card> DiscardPile { get; } = new();
    // -----------------------

    public Player(string name) { Name = name; }

    internal void AssignHero(Hero hero)
    {
        if (_hero != null) throw new InvalidOperationException($"{Name} játékosnak már van hőse.");
        _hero = hero;
    }

    public void Shuffle(Random rng)
    {
        for (int i = DrawPile.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (DrawPile[i], DrawPile[j]) = (DrawPile[j], DrawPile[i]);
        }
    }

    public override string ToString() => Name;
}
