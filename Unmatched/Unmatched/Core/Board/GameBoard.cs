using Unmatched.Core.Fighters;

namespace Unmatched.Core.Board;

// FIGYELEM: a típus neve GameBoard, mert a "Board" névtér (Unmatched.Core.Board) és egy "Board" osztály ütközne.
public class GameBoard
{
    private readonly List<Space> _spaces = new();
    public IReadOnlyList<Space> Spaces => _spaces;

    public Space AddSpace(int id, params int[] zones)
    {
        var s = new Space(id, zones);
        _spaces.Add(s);
        return s;
    }

    public Space Get(int id) => _spaces.First(s => s.Id == id);

    public void Connect(int a, int b) => Space.Connect(Get(a), Get(b));

    public void Place(Fighter f, Space target)
    {
        if (target.Occupant != null && target.Occupant != f)
            throw new InvalidOperationException($"A(z) {target} mező foglalt.");
        if (f.Space != null) f.Space.Occupant = null;
        target.Occupant = f;
        f.PlaceOn(target);
    }

    public void Remove(Fighter f)
    {
        if (f.Space != null) f.Space.Occupant = null;
        f.RemoveFromBoard();
    }

    /// <summary>Közelharc: szomszédos mező. Távolsági: ugyanabban a zónában (a mező több zónát is érinthet).</summary>
    public bool InRange(Fighter attacker, Fighter target)
    {
        if (attacker.Space == null || target.Space == null) return false;
        if (attacker.Owner == target.Owner) return false;   // csak ellenség

        return attacker.IsRanged
            ? attacker.Space.Zones.Overlaps(target.Space.Zones)   // + esetleges látóvonal-szabályok
            : attacker.Space.Adjacent.Contains(target.Space);
    }

    public IReadOnlyList<Fighter> TargetsFor(Fighter attacker) =>
        _spaces.Select(s => s.Occupant)
               .OfType<Fighter>()
               .Where(f => !f.IsDefeated && InRange(attacker, f))
               .ToList();

    /// <summary>
    /// Elérhető üres mezők. Saját harcoson át lehet menni, ellenségen nem.
    /// (SZABÁLY-ELLENŐRZÉS: a mozgás pontos szabályait vesd össze a szabálykönyvvel.)
    /// </summary>
    public IReadOnlyList<Space> ReachableSpaces(Fighter mover, int maxSpaces)
    {
        var result = new List<Space>();
        if (mover.Space == null) return result;

        var dist = new Dictionary<Space, int> { [mover.Space] = 0 };
        var queue = new Queue<Space>();
        queue.Enqueue(mover.Space);

        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            int d = dist[cur];
            if (d >= maxSpaces) continue;

            foreach (var n in cur.Adjacent)
            {
                if (dist.ContainsKey(n)) continue;
                if (n.Occupant != null && n.Occupant.Owner != mover.Owner) continue; // ellenségen nem lehet átmenni

                dist[n] = d + 1;
                queue.Enqueue(n);
                if (n.Occupant == null) result.Add(n);
            }
        }
        return result;
    }
}
