using Unmatched.Core.Board;
using Unmatched.Core.Cards;
using Unmatched.Core.Game;
using Unmatched.Core.Players;
using Unmatched.Data;
using Unmatched.Input;

namespace Unmatched.Demo;

/// <summary>
/// Kis játszható minta PLACEHOLDER karakterekkel (nem Medúza/Arthur!), hogy a motor kipróbálható legyen.
/// Hívd: await DemoGame.RunAsync();
/// </summary>
public static class DemoGame
{
    // Placeholder A: közelharci hős, 1 sidekick
    public static readonly CharacterDef SampleA = new(
        "Minta A",
        Hero: new FighterDef("A hős", Health: 15, MoveValue: 2, IsRanged: false),
        MaxHandSize: 7,
        Sidekick: new SidekickDef(new FighterDef("A segéd", 5, 3, false), Count: 1),
        Deck: StandardDeck());

    // Placeholder B: távolsági hős, 3 egy-életerős sidekick (a többfigurás sidekick kipróbálásához)
    public static readonly CharacterDef SampleB = new(
        "Minta B",
        Hero: new FighterDef("B hős", Health: 12, MoveValue: 2, IsRanged: true),
        MaxHandSize: 7,
        Sidekick: new SidekickDef(new FighterDef("B segéd", 1, 3, true), Count: 3),
        Deck: StandardDeck());

    private static IReadOnlyList<CardDef> StandardDeck() => new[]
    {
        new CardDef("Csapás",     CardType.Attack,    3, 2, Count: 5),
        new CardDef("Mély vágás", CardType.Attack,    4, 1, Count: 3, Effects: () => new ICardEffect[] { new MoveAfterCombat(2) }),
        new CardDef("Pajzs",      CardType.Defense,   3, 2, Count: 4),
        new CardDef("Elhárítás",  CardType.Defense,   2, 1, Count: 2, Effects: () => new ICardEffect[] { new CancelAttack() }),
        new CardDef("Rögtönzés",  CardType.Versatile, 2, 3, Count: 4, Effects: () => new ICardEffect[] { new BonusValue(1) }),
        new CardDef("Tanulmány",  CardType.Scheme,    0, 1, Count: 2, SchemeEffects: () => new ISchemeEffect[] { new DrawCardsScheme(2) }),
        new CardDef("Pihenő",     CardType.Scheme,    0, 1, Count: 2, SchemeEffects: () => new ISchemeEffect[] { new HealScheme(3) }),
    };

    public static GameState Create(int seed = 1)
    {
        var board = new GameBoard();
        for (int i = 0; i < 8; i++)
        {
            // 0-2: 1. zóna, 5-7: 2. zóna, a 3-as és 4-es mező mindkettő
            var zones = i < 3 ? new[] { 1 } : i > 4 ? new[] { 2 } : new[] { 1, 2 };
            board.AddSpace(i, zones);
        }
        for (int i = 0; i < 7; i++) board.Connect(i, i + 1);

        var a = new Player("Alice");
        var b = new Player("Bob");
        var heroA = CharacterFactory.Create(SampleA, a, new Random(seed));
        var heroB = CharacterFactory.Create(SampleB, b, new Random(seed + 1));

        board.Place(heroA, board.Get(0));
        for (int i = 0; i < heroA.Sidekicks.Count; i++) board.Place(heroA.Sidekicks[i], board.Get(1 + i));

        board.Place(heroB, board.Get(7));
        for (int i = 0; i < heroB.Sidekicks.Count; i++) board.Place(heroB.Sidekicks[i], board.Get(6 - i));

        return new GameState(board, new[] { a, b });
    }

    public static async Task RunAsync()
    {
        var state = Create();
        state.Logged += Console.WriteLine;
        await new TurnManager().RunGameAsync(state, new ConsolePlayerInput());
    }
}
