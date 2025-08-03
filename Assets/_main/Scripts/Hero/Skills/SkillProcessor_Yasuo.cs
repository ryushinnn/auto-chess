using System;
using RExt.Extensions;
using UnityEngine;

public class SkillProcessor_Yasuo : SkillProcessor {
    const float AIRBORNE_DURATION = 1f;
    const float DMG_MUL_0 = 1f;
    const float DMG_MUL_1 = 1.5f;
    const string DOT_KEY = "yasuo_wind_claws";
    const float DOT_DMG_MUL = 0.1f;
    const int DOT_TOTAL_TIME = 4000; //ms
    const int DOT_INTERVAL = 333; //ms
    
    public SkillProcessor_Yasuo(Hero hero) : base(hero) {
        AnimationLength = 6.3f;
        Timers = new[] { 0.7f, 1.7f, 3.5f };
        Name = "Tuỷ Tán Tâm Tan";
        Description = $"Hất tung mục tiêu trong {AIRBORNE_DURATION}s sau đó chém 2 lần gây " +
                      $"lần lượt ({DMG_MUL_0 * 100}% <sprite name=pdmg>) sát thương vật lý " +
                      $"và ({DMG_MUL_1 * 100}% <sprite name=pdmg>) sát thương vật lý. Nhát chém thứ 2 kèm theo " +
                      $"hiệu ứng <color=red>XUẤT HUYẾT</color> duy trì {DOT_TOTAL_TIME / 1000f}s\n" +
                      $"<color=red>XUẤT HUYẾT</color>: mỗi {DOT_INTERVAL / 1000f}s gây ({DOT_DMG_MUL * 100}% <sprite name=pdmg>) sát thương vật lý.";
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

        var bleedDmg = attributes.GetDamage(DamageType.Physical, false,
            scaledValues: new[] { (DOT_DMG_MUL, DamageType.Physical) });
        hero.Target.GetAbility<HeroAttributes>().AddDamageOverTime(
            DamageOverTime.Create(
                DOT_KEY,
                hero,
                bleedDmg,
                DOT_TOTAL_TIME / DOT_INTERVAL,
                DOT_INTERVAL.ToSeconds(),
                1,
                false,
                true
            ));
    }
}