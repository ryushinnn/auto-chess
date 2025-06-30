using System;
using RExt.Extension;
using UnityEngine;

public class SkillProcessor_Yasuo : SkillProcessor {
    const float AIRBORNE_DURATION = 1f;
    const float DMG_MUL_0 = 1f;
    const float DMG_MUL_1 = 1.5f;
    const string DOT_KEY = "yasuo_wind_claws";
    const float DOT_DMG_MUL = 0.1f;
    const float DOT_PEN = 0.9f;
    const int DOT_TOTAL_TIME = 4000; //ms
    const int DOT_INTERVAL = 333; //ms
    
    public SkillProcessor_Yasuo(Hero hero) : base(hero) {
        AnimationLength = 6.3f;
        Timers = new[] { 0.7f, 1.7f, 3.5f };
        Description = $"Hất tung mục tiêu trong {AIRBORNE_DURATION}s sau đó chém 2 lần gây " +
                      $"sát thương vật lý lần lượt bằng ({DMG_MUL_0 * 100}% sát thương vật lý) " +
                      $"và ({DMG_MUL_1 * 100}% sát thương vật lý). Nhát chém thứ 2 kèm theo " +
                      $"hiệu ứng XUẤT HUYẾT duy trì {DOT_TOTAL_TIME / 1000f}s\n" +
                      $"XUẤT HUYẾT: mỗi {DOT_INTERVAL / 1000f}s gây sát thương vật lý bằng " +
                      $"({DOT_DMG_MUL * 100}% sát thương vật lý) kèm {DOT_PEN * 100}% xuyên giáp.";
    }

    public override void Process(float timer) {
        if (timer >= Timers[0] && skillExecuted == 0) {
            BlowUp();
            skillExecuted++;
        }
        else if (timer >= Timers[1] && skillExecuted == 1) {
            Cut();
            skillExecuted++;
        }
        else if (timer >= Timers[2] && skillExecuted == 2) {
            BonusCut();
            skillExecuted++;
        }
    }

    void BlowUp() {
        if (hero.Target == null) return;

        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_DURATION);
    }

    void Cut() {
        if (hero.Target == null) return;

        hero.Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical, false,
            scaledValues:new[]{(DMG_MUL_0, DamageType.Physical)}));
    }

    void BonusCut() {
        if (hero.Target == null) return;
        
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical, false,
            scaledValues:new[]{(DMG_MUL_1, DamageType.Physical)}));

        hero.Target.GetAbility<HeroAttributes>().AddDamageOverTime(
            DamageOverTime.Create(
                DOT_KEY,
                hero,
                Damage.Create(
                    attributes.PhysicalDamage * DOT_DMG_MUL,
                    DamageType.Physical,
                    DOT_PEN
                ),
                DOT_TOTAL_TIME / DOT_INTERVAL,
                DOT_INTERVAL.ToSeconds(),
                1,
                false,
                true
            ));
    }
}