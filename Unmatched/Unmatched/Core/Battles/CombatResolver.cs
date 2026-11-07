using Unmatched.Core.Cards;
using Unmatched.Core.Fighters;
using Unmatched.Core.Game;

namespace Unmatched.Core.Battles;

public class CombatResolver
{
    public IReadOnlyList<Card> LegalAttackCards(Fighter attacker) =>
        attacker.Owner.Hand.Where(c => c.CanAttackWith).ToList();

    public IReadOnlyList<Card> LegalDefenseCards(Fighter defender) =>
        defender.Owner.Hand.Where(c => c.CanDefendWith).ToList();

    public async Task ResolveAsync(Combat c, GameState state, IPlayerInput input)
    {
        var legalAttack = LegalAttackCards(c.Attacker);
        var legalDefense = LegalDefenseCards(c.Defender);

        if (legalAttack.Count == 0)
            throw new InvalidOperationException("A támadónak nincs támadó lapja.");

        // 1) Mindkét fél egyszerre, titokban választ – a védő nem látja a támadó lapját.
        c.Phase = CombatPhase.AwaitingCards;
        var attackTask = input.ChooseAttackCardAsync(c.Attacker.Owner, c.Attacker, c.Defender, legalAttack);
        Task<Card?> defenseTask = legalDefense.Count > 0
            ? input.ChooseDefenseCardAsync(c.Defender.Owner, c.Defender, c.Attacker, legalDefense)
            : Task.FromResult<Card?>(null);
        await Task.WhenAll((Task)attackTask, defenseTask);

        var attackCard = await attackTask;
        var defenseCard = await defenseTask;

        // 2) Validálás
        if (!legalAttack.Contains(attackCard))
            throw new InvalidOperationException("Érvénytelen támadó lap.");
        if (defenseCard != null && !legalDefense.Contains(defenseCard))
            throw new InvalidOperationException("Érvénytelen védő lap.");

        // 3) Felfordítás
        c.AttackCard = attackCard;
        c.DefenseCard = defenseCard;
        c.Phase = CombatPhase.Revealed;

        state.Hand(c.Attacker).Remove(attackCard);
        if (defenseCard != null) state.Hand(c.Defender).Remove(defenseCard);

        c.AttackValue = attackCard.Value;
        c.DefenseValue = defenseCard?.Value ?? 0;

        state.Log($"{c.Attacker.Name} támad: {attackCard.Name} ({attackCard.Value}) → {c.Defender.Name}.");
        state.Log(defenseCard != null
            ? $"{c.Defender.Name} véd: {defenseCard.Name} ({defenseCard.Value})."
            : $"{c.Defender.Name} nem véd.");

        // 4) Effektek és sebzés
        c.Phase = CombatPhase.Immediately;
        await RunEffectsAsync(c, state, input, EffectTiming.Immediately);
        if (c.Cancelled) { Cleanup(c); return; }

        c.Phase = CombatPhase.DuringCombat;
        await RunEffectsAsync(c, state, input, EffectTiming.DuringCombat);

        c.Phase = CombatPhase.Damage;
        c.DamageDealt = Math.Max(0, c.AttackValue - c.DefenseValue);
        state.DealDamage(c.Defender, c.DamageDealt);

        // SZABÁLY-ELLENŐRZÉS: az After Combat effektek feltételei (pl. elesett tulajdonos)
        c.Phase = CombatPhase.AfterCombat;
        await RunEffectsAsync(c, state, input, EffectTiming.AfterCombat);

        Cleanup(c);
    }

    private async Task RunEffectsAsync(Combat c, GameState state, IPlayerInput input, EffectTiming timing)
    {
        // Először a támadó effektjei, utána a védőé (szélsőséges esetek: szabálykönyv/FAQ).
        await RunCardEffectsAsync(c.AttackCard, c.Attacker, c, state, input, timing);
        if (c.Cancelled && timing == EffectTiming.Immediately) return;
        await RunCardEffectsAsync(c.DefenseCard, c.Defender, c, state, input, timing);
    }

    private static async Task RunCardEffectsAsync(Card? card, Fighter owner, Combat c,
        GameState state, IPlayerInput input, EffectTiming timing)
    {
        if (card == null) return;
        if (timing == EffectTiming.AfterCombat && (owner.IsDefeated || state.IsGameOver)) return;

        foreach (var effect in card.Effects.Where(e => e.Timing == timing).ToList())
            await effect.ApplyAsync(c, state, owner, input);
    }

    private static void Cleanup(Combat c)
    {
        // A kijátszott lapok a dobópakliba kerülnek.
        if (c.AttackCard != null) c.Attacker.Owner.DiscardPile.Add(c.AttackCard);
        if (c.DefenseCard != null) c.Defender.Owner.DiscardPile.Add(c.DefenseCard);
        c.Phase = CombatPhase.Done;
    }
}
