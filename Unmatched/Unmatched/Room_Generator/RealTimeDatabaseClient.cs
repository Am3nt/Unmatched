using System.Text;
using System.Text.Json;

namespace Unmatched.Room_Generator;

/// <summary>
/// Firebase Realtime Database kliens REST-en keresztül (nincs szükség extra csomagra és service account kulcsra).
/// A szobák a /rooms/{roomCode} útvonalra kerülnek.
/// Ha a Realtime Database szabályai nem engedik a névtelen írást, add meg az authToken-t (Firebase Auth ID token).
/// </summary>
public class RealtimeDatabaseClient : IDatabaseClient
{
    public const string DefaultBaseUrl = "https://szoftfejleszt-rooms.europe-west1.firebasedatabase.app";
    private const string RoomsPath = "rooms";

    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private readonly string? _authToken;

    public RealtimeDatabaseClient(string baseUrl = DefaultBaseUrl, string? authToken = null, HttpClient? http = null)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _authToken = authToken;
        _http = http ?? new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
    }

    private string Url(string path, bool shallow = false)
    {
        var query = new List<string>();
        if (shallow) query.Add("shallow=true");
        if (_authToken != null) query.Add($"auth={Uri.EscapeDataString(_authToken)}");
        return $"{_baseUrl}/{path}.json" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
    }

    public async Task<bool> RoomExistsAsync(string roomCode)
    {
        // shallow=true: csak a kulcsokat kéri le, nem tölti le az egész szobát
        var body = await GetAsync(Url($"{RoomsPath}/{roomCode}", shallow: true));
        return body != "null";
    }

    public async Task SaveRoomAsync(string roomCode, RoomData roomData) =>
        await PutAsync(Url($"{RoomsPath}/{roomCode}"), roomData);

    /// <summary>Szoba beolvasása. Null, ha nem létezik.</summary>
    public async Task<RoomData?> GetRoomAsync(string roomCode)
    {
        var body = await GetAsync(Url($"{RoomsPath}/{roomCode}"));
        return body == "null" ? null : JsonSerializer.Deserialize<RoomData>(body);
    }

    /// <summary>Egy felhasználó hozzáadása/felülírása egy szobában (csatlakozáskor).</summary>
    public async Task SaveUserAsync(string roomCode, string userId, UserData user) =>
        await PutAsync(Url($"{RoomsPath}/{roomCode}/users/{userId}"), user);

    // ---------- HTTP ----------

    private async Task<string> GetAsync(string url)
    {
        using var response = await _http.GetAsync(url);
        return await ReadOrThrowAsync(response);
    }

    private async Task PutAsync<T>(string url, T value)
    {
        var json = JsonSerializer.Serialize(value);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _http.PutAsync(url, content);
        await ReadOrThrowAsync(response);
    }

    private static async Task<string> ReadOrThrowAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode) return body.Trim();

        string hint = (int)response.StatusCode == 401
            ? " (a Realtime Database szabályai nem engedik a hozzáférést)"
            : "";
        throw new InvalidOperationException($"Firebase hiba: {(int)response.StatusCode}{hint} {body}");
    }
}