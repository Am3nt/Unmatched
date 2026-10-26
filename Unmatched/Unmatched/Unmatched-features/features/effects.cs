public enum EffectTiming { Immediately, DuringCombat, AfterCombat }

public interface ICardEffect
{
    EffectTiming Timing { get; }
    void Apply(Combat combat, GameState state, Fighter owner);
}

public class Card
{
    public string Name { get; init; }
    public CardType Type { get; init; }       
    public int Value { get; init; }
    public int Boost { get; init; }
    public IReadOnlyList<ICardEffect> Effects { get; init; }
    public Fighter Owner { get; init; }        
}

public class MoveAfterCombat : ICardEffect
{
    public EffectTiming Timing => EffectTiming.AfterCombat;
    public void Apply(Combat c, GameState s, Fighter owner) => s.RequestMove(owner, maxSpaces: 2);
}