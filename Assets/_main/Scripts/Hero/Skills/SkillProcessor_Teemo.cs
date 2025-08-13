using System;
using RExt.Extensions;
using UnityEngine;

public class SkillProcessor_Teemo : SkillProcessor {
    const float DMG_MUL = 1f;
    const float DMG_MUL_PER_IGNITE = 0.1f;
    
    public SkillProcessor_Teemo(BattleHero hero) : base(hero) {
        animationLength = 4;
        timers = new[] { 1.56f, 2.73f, 3.29f };
        Name = "Hoả Ngục Tàn Hồn: Tam Thức";
        Description = $"Ném 3 quả bom, mỗi quả bom gây ({DMG_MUL * 100}% <sprite name=mdmg>) sát thương phép" +
                      $", tăng {DMG_MUL_PER_IGNITE * 100}% " +
                      $"sát thương với mỗi cộng dồn <color=red>HOẢ NGỤC</color> trên mục tiêu. Mỗi quả bom " +
                      $"đều thêm 1 cộng dồn và làm mới thời gian duy trì của hiệu ứng " +
                      $"<color=red>HOẢ NGỤC</color>. Quả bom thứ 3 gây sát thương chí mạng.";
    }

    public override void Process(float timer) {
        if (timer >= timers[0] && skillExecuted == 0) {
            ThrowBomb();
            skillExecuted++;
        }
        else if (timer >= timers[1] && skillExecuted == 1) {
            ThrowBomb();
            skillExecuted++;
        }
        else if (timer >= timers[2] && skillExecuted == 2) {
            ThrowBigBomb();
            skillExecuted++;
        }
    }

    void ThrowBomb() {
        if (((BattleHero)hero).Target == null) return;

        var currentStacks = ((BattleHero)hero).Target.GetAbility<HeroMark>().GetMark(AttackProcessor_Teemo.DOT_KEY, hero)?.stacks ?? 0;
        var nextStacks = Mathf.Min(currentStacks + 1, AttackProcessor_Teemo.MAX_STACKS);
        
        var igniteDmg = Damage.Create(
            nextStacks * Mathf.Min(attributes.MagicalDamage * AttackProcessor_Teemo.DMG_MUL_LIMIT, ((BattleHero)hero).Target.GetAbility<HeroAttributes>().MaxHp * AttackProcessor_Teemo.MAX_HP_DMG),
            DamageType.True,
            0
        );

        var mainDmg = attributes.GetDamage(DamageType.Magical, false,
            scaledValues: new[] { (DMG_MUL + DMG_MUL_PER_IGNITE * currentStacks, DamageType.Magical) });

        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(new[] { mainDmg, igniteDmg });
        
        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().AddDamageOverTime(
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
        if (((BattleHero)hero).Target == null) return;
        
        var currentStacks = ((BattleHero)hero).Target.GetAbility<HeroMark>().GetMark(AttackProcessor_Teemo.DOT_KEY, hero)?.stacks ?? 0;
        var nextStacks = Mathf.Min(currentStacks + 1, AttackProcessor_Teemo.MAX_STACKS);

        var igniteDmg = Damage.Create(
            nextStacks * Mathf.Min(attributes.MagicalDamage * AttackProcessor_Teemo.DMG_MUL_LIMIT, ((BattleHero)hero).Target.GetAbility<HeroAttributes>().MaxHp * AttackProcessor_Teemo.MAX_HP_DMG),
            DamageType.True,
            0
        );

        var mainDmg = attributes.GetDamage(DamageType.Magical, true,
            scaledValues: new[] { (DMG_MUL + DMG_MUL_PER_IGNITE * currentStacks, DamageType.Magical) });

        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(new[] { mainDmg, igniteDmg });
        
        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().AddDamageOverTime(
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