using Unmatched.Core.Fighters;

namespace Unmatched.Core.Board;

// Ha nálatok már van Space osztály, csak a jelölt tagokat egészítsd ki.
public class Space
{
    public int Id { get; }

    // --- szükséges tagok ---
    /// <summary>Egy mező több zónához is tartozhat a pályán.</summary>
    public HashSet<int> Zones { get; }
    public HashSet<Space> Adjacent { get; } = new();
    public Fighter? Occupant { get; internal set; }
    // -----------------------

    public Space(int id, params int[] zones)
    {
        Id = id;
        Zones = new HashSet<int>(zones);
    }

    public static void Connect(Space a, Space b)
    {
        a.Adjacent.Add(b);
        b.Adjacent.Add(a);
    }

    public override string ToString() => $"#{Id}";
}
