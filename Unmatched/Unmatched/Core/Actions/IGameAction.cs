using Unmatched.Core.Game;
using Unmatched.Core.Players;

namespace Unmatched.Core.Actions;

public enum ActionType { Maneuver, Scheme, Attack }

public interface IGameAction
{
    ActionType Type { get; }
    string Name { get; }
    bool CanExecute(GameState state, Player player);
    Task ExecuteAsync(GameState state, Player player, IPlayerInput input);
}
