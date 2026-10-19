using Unmatched.Core.Players;

namespace Unmatched.Core.Fighters;

//A secondary fighter tied to a hero. Defeating it does not end the game
public class Sidekick : Fighter
{
    public Hero Master { get; }

    public override bool EndsGameWhenDefeated => false;

    public Sidekick(string name, Hero master, int maxHealth, bool isRanged)
        : base(name, master.Owner, maxHealth, isRanged)
    {
        Master = master;
        master.AssignSidekick(this);
    }
}