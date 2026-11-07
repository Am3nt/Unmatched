using Unmatched.Core.Players;

namespace Unmatched.Core.Fighters;

//A player's main character. If it is defeated, the game ends
public class Hero : Fighter
{
    public int MaxHandSize { get; }

    //VÁLTOZÁS: több sidekick is lehet (pl. egy sidekick-csoport több figurával)
    private readonly List<Sidekick> _sidekicks = new();
    public IReadOnlyList<Sidekick> Sidekicks => _sidekicks;

    public override bool EndsGameWhenDefeated => true;

    public Hero(string name, Player owner, int maxHealth, int moveValue, bool isRanged, int maxHandSize = 5)
        : base(name, owner, maxHealth, moveValue, isRanged)
    {
        MaxHandSize = maxHandSize;
        owner.AssignHero(this);
    }

    internal void AddSidekick(Sidekick sidekick)
    {
        if (_sidekicks.Contains(sidekick)) throw new InvalidOperationException($"{sidekick.Name} már hozzá van rendelve.");
        _sidekicks.Add(sidekick);
    }
}
