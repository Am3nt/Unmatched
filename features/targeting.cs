public class Space { public int Id; public HashSet<Space> Adjacent; public int Zone; }

public bool InRange(Fighter attacker, Fighter target) =>
    attacker.IsRanged
        ? attacker.Space.Zone == target.Space.Zone  // plus any line-of-sight rules
        : attacker.Space.Adjacent.Contains(target.Space);