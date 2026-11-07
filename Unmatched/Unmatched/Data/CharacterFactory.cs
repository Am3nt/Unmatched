using Unmatched.Core.Cards;
using Unmatched.Core.Fighters;
using Unmatched.Core.Players;

namespace Unmatched.Data;

public static class CharacterFactory
{
    /// <summary>Létrehozza a hőst, a sidekickeket és a megkevert paklit, majd kioszt kezdőkezet. A lerakásról (Board) a hívó gondoskodik.</summary>
    public static Hero Create(CharacterDef def, Player player, Random rng, int startingHandSize = 5)
    {
        var hero = new Hero(def.Hero.Name, player, def.Hero.Health, def.Hero.MoveValue,
                            def.Hero.IsRanged, def.MaxHandSize);

        var sk = def.Sidekick;
        for (int i = 0; i < sk.Count; i++)
        {
            string name = sk.Count > 1 ? $"{sk.Stats.Name} {i + 1}" : sk.Stats.Name;
            new Sidekick(name, hero, sk.Stats.Health, sk.Stats.MoveValue, sk.Stats.IsRanged);
        }

        foreach (var cd in def.Deck)
        {
            for (int i = 0; i < cd.Count; i++)
            {
                player.DrawPile.Add(new Card
                {
                    Name = cd.Name,
                    Type = cd.Type,
                    Value = cd.Value,
                    Boost = cd.Boost,
                    Effects = cd.Effects?.Invoke() ?? Array.Empty<ICardEffect>(),
                    SchemeEffects = cd.SchemeEffects?.Invoke() ?? Array.Empty<ISchemeEffect>(),
                    Owner = player
                });
            }
        }

        player.Shuffle(rng);
        for (int i = 0; i < startingHandSize && player.DrawPile.Count > 0; i++)
        {
            player.Hand.Add(player.DrawPile[^1]);
            player.DrawPile.RemoveAt(player.DrawPile.Count - 1);
        }
        return hero;
    }
}
