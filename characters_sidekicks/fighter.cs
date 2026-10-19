using Unmatched.Core.Board;
using Unmatched.Core.Players;

namespace Unmatched.Core.Fighters;


// Anything that can stand on the board, attack, and be attacked.
// Heroes and sidekicks both derive from this.
public abstract class Fighter
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }
    public Player Owner { get; }
    public int MaxHealth { get; }
    public int CurrentHealth { get; private set; }
    public bool IsRanged { get; }
    public Space? Space { get; private set; }

    public bool IsDefeated => CurrentHealth <= 0;

    //True if this fighter being defeated ends the game (heroes yes, sidekicks no)
    public abstract bool EndsGameWhenDefeated { get; }

    //Raised once, when health first reaches 0
    public event Action<Fighter>? Defeated;

    protected Fighter(string name, Player owner, int maxHealth, bool isRanged)
    {
        if (maxHealth <= 0) throw new ArgumentOutOfRangeException(nameof(maxHealth));

        Name = name;
        Owner = owner;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        IsRanged = isRanged;
    }

    //Puts the fighter on a space (used for setup, movement, and push/displace effects)
    public void PlaceOn(Space space) => Space = space ?? throw new ArgumentNullException(nameof(space));

    //Removes the fighter from the board (e.g. when defeated)
    public void RemoveFromBoard() => Space = null;

    //Applies damage and returns the amount actually dealt
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (IsDefeated || amount == 0) return 0;

        int dealt = Math.Min(amount, CurrentHealth);
        CurrentHealth -= dealt;

        if (IsDefeated) Defeated?.Invoke(this);
        return dealt;
    }

    //Restores health up to MaxHealth and returns the amount actually healed
    public int Heal(int amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (IsDefeated) return 0;

        int healed = Math.Min(amount, MaxHealth - CurrentHealth);
        CurrentHealth += healed;
        return healed;
    }

    public override string ToString() => $"{Name} ({CurrentHealth}/{MaxHealth})";
}