namespace Unmatched.UI.Services;

/// <summary>Fake service so the UI is usable before any server exists. Always succeeds after a short delay.</summary>
public class StubRoomService : IRoomService
{
    public async Task<RoomInfo> CreateRoomAsync(string playerName, CancellationToken cancellationToken = default)
    {
        await Task.Delay(400, cancellationToken);
        return new RoomInfo(RoomCode.Generate(), playerName);
    }

    public async Task<RoomInfo> JoinRoomAsync(string roomCode, string playerName, CancellationToken cancellationToken = default)
    {
        await Task.Delay(400, cancellationToken);

        if (!RoomCode.IsValid(roomCode))
            throw new ArgumentException("That is not a valid room code.");

        return new RoomInfo(roomCode, "Host");
    }
}