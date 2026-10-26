public async Task ResolveAsync(Combat c, GameState state, IPlayerInput input)
{
    // Attacker commits, defender responds
    c.AttackCard  = await input.ChooseAttackCardAsync(c.Attacker.Owner, c);
    c.DefenseCard = await input.ChooseDefenseCardAsync(c.Defender.Owner, c); // may be null

    state.Hand(c.Attacker).Remove(c.AttackCard);
    if (c.DefenseCard != null) state.Hand(c.Defender).Remove(c.DefenseCard);

    c.AttackValue  = c.AttackCard.Value;
    c.DefenseValue = c.DefenseCard?.Value ?? 0;

    await RunEffectsAsync(c, state, EffectTiming.Immediately);
    if (c.Cancelled) { Cleanup(c, state); return; }

    await RunEffectsAsync(c, state, EffectTiming.DuringCombat);

    int damage = Math.Max(0, c.AttackValue - c.DefenseValue);
    state.DealDamage(c.Defender, damage);

    if (!c.Defender.IsDefeated)
        await RunEffectsAsync(c, state, EffectTiming.AfterCombat);

    Cleanup(c, state);
}

private async Task RunEffectsAsync(Combat c, GameState s, EffectTiming timing)
{
    // Attacker's effects first, then defender's (check the rulebook/FAQ for edge cases)
    foreach (var e in c.AttackCard.Effects.Where(e => e.Timing == timing))
        await e.Apply(c, s, c.Attacker);
    if (c.DefenseCard != null)
        foreach (var e in c.DefenseCard.Effects.Where(e => e.Timing == timing))
            await e.Apply(c, s, c.Defender);
}