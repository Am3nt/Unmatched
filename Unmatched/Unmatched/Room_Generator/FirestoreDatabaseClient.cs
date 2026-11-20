using Google.Cloud.Firestore;

namespace Unmatched.Room_Generator;

/// <summary>
/// Firestore alapú IDatabaseClient. A RoomData modelleket kézzel képezi le Dictionary-kre,
/// így a Room_generator.cs-ben lévő [JsonPropertyName] modellek változatlanok maradhatnak.
/// Séma: rooms/{roomCode} dokumentum, mezők: map_id, heroes, sidekicks, users.
/// </summary>
public class FirestoreDatabaseClient : IDatabaseClient
{
    private const string RoomsCollection = "rooms";   // FELTÉTELEZÉS: egyeztesd a valós gyűjtemény nevével
    private readonly FirestoreDb _db;

    public FirestoreDatabaseClient(FirestoreDb db) => _db = db;

    /// <summary>Hitelesítés: GOOGLE_APPLICATION_CREDENTIALS környezeti változó a kulcsfájlra mutat.</summary>
    public static FirestoreDatabaseClient Create(string projectId = "szoftfejleszt") =>
        new(FirestoreDb.Create(projectId));

    private DocumentReference RoomDoc(string roomCode) =>
        _db.Collection(RoomsCollection).Document(roomCode);

    public async Task<bool> RoomExistsAsync(string roomCode)
    {
        var snap = await RoomDoc(roomCode).GetSnapshotAsync();
        return snap.Exists;
    }

    public async Task SaveRoomAsync(string roomCode, RoomData roomData)
    {
        await RoomDoc(roomCode).SetAsync(ToMap(roomData));
    }

    /// <summary>Szoba beolvasása (pl. csatlakozáskor). Null, ha nem létezik.</summary>
    public async Task<RoomData?> GetRoomAsync(string roomCode)
    {
        var snap = await RoomDoc(roomCode).GetSnapshotAsync();
        return snap.Exists ? FromMap(snap.ToDictionary()) : null;
    }

    // ---------- RoomData -> Firestore ----------

    private static Dictionary<string, object> ToMap(RoomData r) => new()
    {
        ["map_id"] = r.MapId,
        ["heroes"] = r.Heroes.ToDictionary(
            h => h.Key,
            h => (object)new Dictionary<string, object>
            {
                ["sidekick_id"] = h.Value.SidekickId,
                ["values"] = ValuesToMap(h.Value.Values)
            }),
        ["sidekicks"] = r.Sidekicks.ToDictionary(
            s => s.Key,
            s => (object)new Dictionary<string, object>
            {
                ["hero_id"] = s.Value.HeroId,
                ["values"] = ValuesToMap(s.Value.Values)
            }),
        ["users"] = r.Users.ToDictionary(
            u => u.Key,
            u => (object)new Dictionary<string, object>
            {
                ["name"] = u.Value.Name,
                ["hero"] = u.Value.Hero,
                ["sidekick"] = u.Value.Sidekick
            })
    };

    private static Dictionary<string, object> ValuesToMap(UnitValues v) => new()
    {
        ["health"] = v.Health,
        ["position"] = v.Position
    };

    // ---------- Firestore -> RoomData ----------

    private static RoomData FromMap(Dictionary<string, object> d) => new()
    {
        MapId = Str(d, "map_id"),
        Heroes = Sub(d, "heroes").ToDictionary(
            e => e.Key,
            e =>
            {
                var m = AsMap(e.Value);
                return new HeroData { SidekickId = Str(m, "sidekick_id"), Values = ValuesFromMap(m) };
            }),
        Sidekicks = Sub(d, "sidekicks").ToDictionary(
            e => e.Key,
            e =>
            {
                var m = AsMap(e.Value);
                return new SidekickData { HeroId = Str(m, "hero_id"), Values = ValuesFromMap(m) };
            }),
        Users = Sub(d, "users").ToDictionary(
            e => e.Key,
            e =>
            {
                var m = AsMap(e.Value);
                return new UserData { Name = Str(m, "name"), Hero = Str(m, "hero"), Sidekick = Str(m, "sidekick") };
            })
    };

    private static UnitValues ValuesFromMap(Dictionary<string, object> parent)
    {
        var v = Sub(parent, "values");
        return new UnitValues
        {
            Health = v.TryGetValue("health", out var h) ? Convert.ToInt32(h) : 0,
            Position = v.TryGetValue("position", out var p) ? Convert.ToInt32(p) : 0
        };
    }

    private static Dictionary<string, object> AsMap(object o) => (Dictionary<string, object>)o;

    private static Dictionary<string, object> Sub(Dictionary<string, object> d, string key) =>
        d.TryGetValue(key, out var o) && o is Dictionary<string, object> m ? m : new();

    private static string Str(Dictionary<string, object> d, string key) =>
        d.TryGetValue(key, out var o) ? o?.ToString() ?? "" : "";
}