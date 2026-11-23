using Unmatched.Room_Generator;

namespace Unmatched.UI.Services;

/// <summary>
/// A UI Create/Join gombjai ezen keresztül hoznak létre és találnak meg szobákat
/// a Firebase Realtime Database /rooms ágában.
/// </summary>
public class RealtimeRoomService : IRoomService
{
    private const string DefaultMapId = "B.P.R.D. HQ";
    private const int MaxPlayers = 2;

    private readonly RealtimeDatabaseClient _db;
    private readonly Room_generator _generator;

    public RealtimeRoomService() : this(new RealtimeDatabaseClient()) { }

    public RealtimeRoomService(RealtimeDatabaseClient db)
    {
        _db = db;
        _generator = new Room_generator(db);
    }

    public async Task<RoomInfo> CreateRoomAsync(string playerName, CancellationToken cancellationToken = default)
    {
        // A hőst és a sidekicket a játékos később választja, ezért itt csak a pálya és a házigazda kerül be.
        var code = await _generator.CreateRoomAsync(playerName, DefaultMapId);
        return new RoomInfo(code, playerName);
    }

    public async Task<RoomInfo> JoinRoomAsync(string roomCode, string playerName, CancellationToken cancellationToken = default)
    {
        var room = await _db.GetRoomAsync(roomCode)
                   ?? throw new InvalidOperationException("Room not found.");

        if (room.Users.Count >= MaxPlayers)
            throw new InvalidOperationException("The room is full.");

        // Egyszerű azonosító: U_0001 a házigazda, U_0002 a csatlakozó.
        string userId = $"U_{room.Users.Count + 1:D4}";
        await _db.SaveUserAsync(roomCode, userId, new UserData { Name = playerName });

        string hostName = room.Users.Values.FirstOrDefault()?.Name ?? "Host";
        return new RoomInfo(roomCode, hostName);
    }
}