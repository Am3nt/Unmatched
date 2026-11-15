using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Unmatched.Room_Generator
{
    internal class Room_generator
    {
        private readonly IDatabaseClient _dbClient;

        // Constructor
        public Room_generator(IDatabaseClient dbClient)
        {
            _dbClient = dbClient;
        }

        // The room generator function
        public async Task<string> CreateRoomAsync(string hostUsername, string chosenHero, string heroSidekick, string mapId)
        {
            string roomCode;
            bool isDuplicate;
            Random random = new Random();

            do
            {
                string randomNumbers = random.Next(0, 10000).ToString("D4");
                roomCode = $"R_{randomNumbers}";
                isDuplicate = await _dbClient.RoomExistsAsync(roomCode);

            } while (isDuplicate);

            string hostUserId = "U_0001";

            var newRoom = new RoomData
            {
                MapId = mapId,
                Heroes = new Dictionary<string, HeroData>
                {
                    { chosenHero, new HeroData { SidekickId = heroSidekick, Values = new UnitValues { Health = 1, Position = 1 } } }
                },
                Sidekicks = new Dictionary<string, SidekickData>
                {
                    { heroSidekick, new SidekickData { HeroId = chosenHero, Values = new UnitValues { Health = 1, Position = 1 } } }
                },
                Users = new Dictionary<string, UserData>
                {
                    { hostUserId, new UserData { Name = hostUsername, Hero = chosenHero, Sidekick = heroSidekick } }
                }
            };

            await _dbClient.SaveRoomAsync(roomCode, newRoom);

            return roomCode;
        }
    }

    // --- Data Models & Interfaces (Placed below the main class) ---

    public class RoomData
    {
        [JsonPropertyName("map_id")]
        public string MapId { get; set; }

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
        public string SidekickId { get; set; }

        [JsonPropertyName("values")]
        public UnitValues Values { get; set; }
    }

    public class SidekickData
    {
        [JsonPropertyName("hero_id")]
        public string HeroId { get; set; }

        [JsonPropertyName("values")]
        public UnitValues Values { get; set; }
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
        public string Name { get; set; }

        [JsonPropertyName("hero")]
        public string Hero { get; set; }

        [JsonPropertyName("sidekick")]
        public string Sidekick { get; set; }
    }

    public interface IDatabaseClient
    {
        Task<bool> RoomExistsAsync(string roomCode);
        Task SaveRoomAsync(string roomCode, RoomData roomData);
    }
}