using Unmatched.Core.Actions;
using Unmatched.Core.Board;
using Unmatched.Core.Cards;
using Unmatched.Core.Fighters;
using Unmatched.Core.Players;

namespace Unmatched.Core.Game;

/// <summary>
/// A játékmotor ezen keresztül kérdez a játékosoktól. Konzol, UI, AI vagy hálózat is implementálhatja.
/// Mindig a legális opciók listájából kell választani.
/// </summary>
public interface IPlayerInput
{
    Task<IGameAction> ChooseActionAsync(Player player, IReadOnlyList<IGameAction> legal);

    Task<Fighter> ChooseFighterAsync(Player player, IReadOnlyList<Fighter> options, string prompt);

    Task<Fighter> ChooseTargetAsync(Player player, Fighter attacker, IReadOnlyList<Fighter> targets);

    /// <summary>null = a harcos marad a helyén.</summary>
    Task<Space?> ChooseSpaceAsync(Player player, Fighter fighter, IReadOnlyList<Space> options);

    Task<Card> ChooseAttackCardAsync(Player player, Fighter attacker, Fighter defender, IReadOnlyList<Card> legal);

    /// <summary>null = nincs védekezés. A támadó lapját a védő NEM kapja meg (egyszerre, titokban választanak).</summary>
    Task<Card?> ChooseDefenseCardAsync(Player player, Fighter defender, Fighter attacker, IReadOnlyList<Card> legal);

    /// <summary>Kötelező választás (pl. séma kijátszása, kézlimit miatti eldobás).</summary>
    Task<Card> ChooseCardAsync(Player player, IReadOnlyList<Card> options, string prompt);

    /// <summary>Opcionális boost lap eldobása a mozgáshoz. null = nem boostol.</summary>
    Task<Card?> ChooseBoostCardAsync(Player player, IReadOnlyList<Card> hand);
}
