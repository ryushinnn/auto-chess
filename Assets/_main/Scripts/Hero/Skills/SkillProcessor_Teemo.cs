using System;
using RExt.Extension;
using UnityEngine;

public class SkillProcessor_Teemo : SkillProcessor {
    const float DMG_MUL = 1f;
    const float DMG_MUL_PER_IGNITE = 0.1f;
    
    public SkillProcessor_Teemo(Hero hero) : base(hero) {
        AnimationLength = 4;
        Timers = new[] { 1.56f, 2.73f, 3.29f };
        Description = "Ném 3 quả bom, mỗi quả bom gây sát thương phép bằng " +
                      $"({DMG_MUL * 100}% sát thương phép), tăng {DMG_MUL_PER_IGNITE * 100}% " +
                      $"sát thương với mỗi cộng dồn HOẢ NGỤC trên mục tiêu. Mỗi quả bom " +
                      $"đều thêm 1 cộng dồn và làm mới thời gian duy trì của hiệu ứng " +
                      $"HOẢ NGỤC. Quả bom thứ 3 gây sát thương chí mạng.";
    }

    public override void Process(float timer) {
        if (timer >= Timers[0] && skillExecuted == 0) {
            ThrowBomb();
            skillExecuted++;
        }
        else if (timer >= Timers[1] && skillExecuted == 1) {
            ThrowBomb();
            skillExecuted++;
        }
        else if (timer >= Timers[2] && skillExecuted == 2) {
            ThrowBigBomb();
            skillExecuted++;
        }
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

        var mainDmg = attributes.GetDamage(DamageType.Magical, false,
            scaledValues: new[] { (DMG_MUL + DMG_MUL_PER_IGNITE * currentStacks, DamageType.Magical) });

        hero.Target.GetAbility<HeroAttributes>().TakeDamage(new[] { mainDmg, igniteDmg });
        
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

        var mainDmg = attributes.GetDamage(DamageType.Magical, true,
            scaledValues: new[] { (DMG_MUL + DMG_MUL_PER_IGNITE * currentStacks, DamageType.Magical) });

        hero.Target.GetAbility<HeroAttributes>().TakeDamage(new[] { mainDmg, igniteDmg });
        
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