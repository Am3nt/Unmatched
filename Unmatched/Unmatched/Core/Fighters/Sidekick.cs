using Unmatched.Core.Players;

namespace Unmatched.Core.Fighters;

//A secondary fighter tied to a hero. Defeating it does not end the game
public class Sidekick : Fighter
{
    public Hero Master { get; }

    public override bool EndsGameWhenDefeated => false;

    public Sidekick(string name, Hero master, int maxHealth, int moveValue, bool isRanged)
        : base(name, master.Owner, maxHealth, moveValue, isRanged)
    {
        Master = master;
        master.AddSidekick(this);
    }
}
