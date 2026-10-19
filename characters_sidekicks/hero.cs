using Unmatched.Core.Players;

namespace Unmatched.Core.Fighters;

//A player's main character. If it is defeated, the game ends
public class Hero : Fighter
{
    public int MoveValue { get; }
    public int MaxHandSize { get; }
    public Sidekick? Sidekick { get; private set; }

    public override bool EndsGameWhenDefeated => true;

    public Hero(string name, Player owner, int maxHealth, int moveValue, bool isRanged, int maxHandSize = 5)
        : base(name, owner, maxHealth, isRanged)
    {
        MoveValue = moveValue;
        MaxHandSize = maxHandSize;
    }

    public void AssignSidekick(Sidekick sidekick)
    {
        if (Sidekick != null) throw new InvalidOperationException($"{Name} already has a sidekick.");
        Sidekick = sidekick;
    }
}