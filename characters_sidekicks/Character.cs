

public enum AttackType
{
	Melee,
	Ranged
}

public enum CardType
{
	Attack,
	Defense,
	Both
}

public abstract class Fighter
{
	public string Name { get; set; }
	public int Health { get; set; }
	public int MoveRange { get; set; }
	public AttackType AttackType { get; set; }
}

public class Hero : Fighter
{
	public List<Sidekick> Sidekicks { get; set; }
}

public class Sidekick : Fighter
{
	public int AmountOfSidekicks { get; set; } 
}

public class CardDeck
{
	public int AmountOfCards { get; set; }
	public CardType Type { get; set; }
}

public class Character
{
	public string Name { get; set; }
	public Hero Hero { get; set; }
	public List<Sidekick> Sidekicks { get; set; }
	public CardDeck Cards { get; set; }
}
//Nem Jo itt minden, csak nem akarok majd konfiktusokat  ezert toltoom fel 