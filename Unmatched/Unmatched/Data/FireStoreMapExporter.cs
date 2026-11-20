using System.Text.Json;
using Google.Cloud.Firestore;

namespace Unmatched.Data;

/// <summary>
/// Egyszeri eszköz: a maps/{mapId} dokumentumot teljes egészében JSON fájlba menti,
/// hogy a pálya a repóban legyen, és a játék adatbázis nélkül betölthesse.
/// </summary>
public static class FirestoreMapExporter
{
    public static async Task ExportAsync(string mapId, string outputPath, string projectId = "szoftfejleszt")
    {
        var db = FirestoreDb.Create(projectId);
        var snap = await db.Collection("maps").Document(mapId).GetSnapshotAsync();
        if (!snap.Exists)
            throw new InvalidOperationException($"A(z) maps/{mapId} dokumentum nem található.");

        var json = JsonSerializer.Serialize(
            ToJsonValue(snap.ToDictionary()),
            new JsonSerializerOptions { WriteIndented = true });

        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);
        await File.WriteAllTextAsync(outputPath, json);
    }

    // Firestore-specifikus típusokból sima JSON-barát érték (map, lista, szám, szöveg).
    private static object? ToJsonValue(object? v) => v switch
    {
        Dictionary<string, object> d => d.ToDictionary(kv => kv.Key, kv => ToJsonValue(kv.Value)),
        List<object> l => l.Select(ToJsonValue).ToList(),
        Timestamp t => t.ToDateTime().ToString("o"),
        GeoPoint g => new { lat = g.Latitude, lng = g.Longitude },
        _ => v
    };
}