namespace Unmatched.UI.Services;

public record RoomInfo(string Code, string HostName);

/// <summary>
/// What the UI needs from the networking layer. Implement this against the real server later;
/// the screens never need to change.
/// </summary>
public interface IRoomService
{
    Task<RoomInfo> CreateRoomAsync(string playerName, CancellationToken cancellationToken = default);
    Task<RoomInfo> JoinRoomAsync(string roomCode, string playerName, CancellationToken cancellationToken = default);
}