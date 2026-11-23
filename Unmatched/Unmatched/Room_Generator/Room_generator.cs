using System.Text.Json.Serialization;
using Unmatched.UI.Services;

namespace Unmatched.Room_Generator
{
    internal class Room_generator
    {
        private const string HostUserId = "U_0001";

        private readonly IDatabaseClient _dbClient;

        public Room_generator(IDatabaseClient dbClient)
        {
            _dbClient = dbClient;
        }

        /// <summary>
        /// Új szoba létrehozása és mentése. A hős és a sidekick opcionális: a UI "Create" gombja még
        /// csak nevet kér, a hőst a játékos később (lobbyban) választja.
        /// </summary>
        public async Task<string> CreateRoomAsync(string hostUsername, string mapId,
                                                  string? chosenHero = null, string? heroSidekick = null)
        {
            // A UI ugyanezt a kódformátumot (RoomCode) ellenőrzi csatlakozáskor.
            string roomCode;
            do
            {
                roomCode = RoomCode.Generate();
            } while (await _dbClient.RoomExistsAsync(roomCode));

            var room = new RoomData { MapId = mapId };

            room.Users[HostUserId] = new UserData
            {
                Name = hostUsername,
                Hero = chosenHero ?? "",
                Sidekick = heroSidekick ?? ""
            };

            // A Realtime Database nem tárol üres objektumot, ezért hős nélkül a "heroes"/"sidekicks" ág meg sem jelenik.
            if (!string.IsNullOrEmpty(chosenHero))
            {
                room.Heroes[chosenHero] = new HeroData
                {
                    SidekickId = heroSidekick ?? "",
                    Values = new UnitValues { Health = 1, Position = 1 }   // ideiglenes kezdőérték
                };

                if (!string.IsNullOrEmpty(heroSidekick))
                {
                    room.Sidekicks[heroSidekick] = new SidekickData
                    {
                        HeroId = chosenHero,
                        Values = new UnitValues { Health = 1, Position = 1 }
                    };
                }
            }

            await _dbClient.SaveRoomAsync(roomCode, room);
            return roomCode;
        }
    }

    // --- Data Models & Interfaces ---

    public class RoomData
    {
        [JsonPropertyName("map_id")]
        public string MapId { get; set; } = "";

        [JsonPropertyName("heroes")]
        public Dictionary<string, HeroData> Heroes { get; set; } = new();

        [JsonPropertyName("sidekicks")]
        public Dictionary<string, SidekickData> Sidekicks { get; set; } = new();

        [JsonPropertyName("users")]
        public Dictionary<string, UserData> Users { get; set; } = new();
    }

    public class HeroData
    {
        [JsonPropertyName("sidekick_id")]
        public string SidekickId { get; set; } = "";

        [JsonPropertyName("values")]
        public UnitValues Values { get; set; } = new();
    }

    public class SidekickData
    {
        [JsonPropertyName("hero_id")]
        public string HeroId { get; set; } = "";

        [JsonPropertyName("values")]
        public UnitValues Values { get; set; } = new();
    }

    public class UnitValues
    {
        [JsonPropertyName("health")]
        public int Health { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }
    }

    public class UserData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("hero")]
        public string Hero { get; set; } = "";

        [JsonPropertyName("sidekick")]
        public string Sidekick { get; set; } = "";
    }

    public interface IDatabaseClient
    {
        Task<bool> RoomExistsAsync(string roomCode);
        Task SaveRoomAsync(string roomCode, RoomData roomData);
    }
}