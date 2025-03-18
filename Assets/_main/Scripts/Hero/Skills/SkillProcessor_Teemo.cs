using System;
using RExt.Extension;
using UnityEngine;

/// <summary>
/// nem 3 qua bom, gay 100% st phep, +10% st phep voi moi cong don thieu dot
/// moi qua bom deu gay them hieu ung thieu dot
/// qua thu 3 gay st chi mang
/// </summary>
public class SkillProcessor_Teemo : SkillProcessor {
    const float DMG_MUL = 1f;
    const float DMG_MUL_PER_IGNITE = 0.1f;
    
    public SkillProcessor_Teemo(Hero hero) : base(hero) {
        events = new Action[]{ThrowBomb, ThrowBigBomb};
    }
    
    void ThrowBomb() {
        if (hero.Target == null) return;

        var currentStacks = hero.Target.GetAbility<HeroMark>().GetMark(AttackProcessor_Teemo.DOT_KEY, hero)?.stacks ?? 0;
        var nextStacks = Mathf.Min(currentStacks + 1, AttackProcessor_Teemo.MAX_STACKS);
        
        var igniteDmg = Damage.Create(
            nextStacks * Mathf.Min(attributes.MagicalDamage * AttackProcessor_Teemo.DMG_MUL_LIMIT, hero.Target.GetAbility<HeroAttributes>().MaxHp * AttackProcessor_Teemo.MAX_HP_DMG),
            DamageType.True,
            0
        );

        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            new[] {
                Damage.Create(
                    attributes.MagicalDamage * (DMG_MUL + DMG_MUL_PER_IGNITE * currentStacks),
                    DamageType.Magical,
                    attributes.MagicalPenetration),
                igniteDmg
            });
        
        hero.Target.GetAbility<HeroAttributes>().AddDamageOverTime(
            DamageOverTime.Create(
                    AttackProcessor_Teemo.DOT_KEY,
                    hero,
                    igniteDmg, 
                    (AttackProcessor_Teemo.TOTAL_TIME / AttackProcessor_Teemo.INTERVAL) - 1,
                    AttackProcessor_Teemo.INTERVAL.ToSeconds(),
                    nextStacks,
                    false,
                    true
                ));
    }

    void ThrowBigBomb() {
        if (hero.Target == null) return;
        
        var currentStacks = hero.Target.GetAbility<HeroMark>().GetMark(AttackProcessor_Teemo.DOT_KEY, hero)?.stacks ?? 0;
        var nextStacks = Mathf.Min(currentStacks + 1, AttackProcessor_Teemo.MAX_STACKS);

        var igniteDmg = Damage.Create(
            nextStacks * Mathf.Min(attributes.MagicalDamage * AttackProcessor_Teemo.DMG_MUL_LIMIT, hero.Target.GetAbility<HeroAttributes>().MaxHp * AttackProcessor_Teemo.MAX_HP_DMG),
            DamageType.True,
            0
        );

        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            new[] {
                Damage.Create(
                    attributes.MagicalDamage * (DMG_MUL + DMG_MUL_PER_IGNITE * currentStacks) * attributes.CriticalDamage,
                    DamageType.Magical,
                    attributes.MagicalPenetration,
                    true),
                igniteDmg
            });
        
        hero.Target.GetAbility<HeroAttributes>().AddDamageOverTime(
            DamageOverTime.Create(
                AttackProcessor_Teemo.DOT_KEY,
                hero,
                igniteDmg,
                (AttackProcessor_Teemo.TOTAL_TIME / AttackProcessor_Teemo.INTERVAL) - 1,
                AttackProcessor_Teemo.INTERVAL.ToSeconds(),
                nextStacks,
                false,
                true
            ));
    }
}