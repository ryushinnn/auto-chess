using System;
using UnityEngine;

public class SkillProcessor_Aatrox_Light : SkillProcessor {
    const float DMG_MUL_0 = 0.7f;
    const float DMG_MUL_1 = 1.5f;
    const float DMG_MUL_2 = 2f;
    const float AIRBORNE_TIME_0 = 0.2f;
    const float AIRBORNE_TIME_1 = 0.2f;
    const float AIRBORNE_TIME_2 = 0.5f;
    const float VAMP_0 = 0.25f;
    const float VAMP_1 = 0.5f;
    const float VAMP_2 = 1f;
    const float VAMP_MUL_WHEN_LOW_HP = 2f;
    const float HP_THRESHOLD = 0.5f;

    public SkillProcessor_Aatrox_Light(BattleHero hero) : base(hero) {
        animationLength = 5f;
        timers = new[] { 0.6f, 2.3f, 4.1f };
        unstoppable = true;
        Name = "Nguyệt Thực: Bạch Vân Liên Trảm";
        Description = $"Chém 3 nhát, lần lượt gây ({DMG_MUL_0 * 100}%/{DMG_MUL_1 * 100}%/{DMG_MUL_2 * 100}% <sprite name=mdmg>) sát thương phép " +
                      $"và hất tung mục tiêu trong {AIRBORNE_TIME_0}s/{AIRBORNE_TIME_1}s/{AIRBORNE_TIME_2}s " +
                      $"đồng thời hồi máu bằng {VAMP_0 * 100}%/{VAMP_1 * 100}%/{VAMP_2 * 100}% " +
                      $"sát thương gây ra. Nếu bản thân có thấp hơn {HP_THRESHOLD * 100}% <sprite name=hp>, " +
                      $"lượng máu hồi phục gấp {VAMP_MUL_WHEN_LOW_HP} lần.";
    }
    
    public override void Process(float timer) {
        if (timer >= timers[0] && skillExecuted == 0) {
            LightSlash();
            skillExecuted++;
        }
        else if (timer >= timers[1] && skillExecuted == 1) {
            MediumSlash();
            skillExecuted++;
        }
        else if (timer >= timers[2] && skillExecuted == 2) {
            HeavySlash();
            skillExecuted++;
        }
    }

    void LightSlash() {
        if (((BattleHero)hero).Target == null) return;

        var outputDmg = ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Magical, false,
            scaledValues: new []{(DMG_MUL_0, DamageType.Magical)}));
        
        var vamp = attributes.HpPercentage < HP_THRESHOLD ? VAMP_0 * VAMP_MUL_WHEN_LOW_HP : VAMP_0;
        attributes.Heal(outputDmg * vamp);
        ((BattleHero)hero).Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_0);
    }
    
    void MediumSlash() {
        if (((BattleHero)hero).Target == null) return;
        
        var outputDmg = ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Magical, false,
            scaledValues: new []{(DMG_MUL_1, DamageType.Magical)}));
        
        var vamp = attributes.HpPercentage < HP_THRESHOLD ? VAMP_1 * VAMP_MUL_WHEN_LOW_HP : VAMP_1;
        attributes.Heal(outputDmg * vamp);
        ((BattleHero)hero).Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_1);
    }
    
    void HeavySlash() {
        if (((BattleHero)hero).Target == null) return;
        
        var outputDmg = ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Magical, false,
            scaledValues: new []{(DMG_MUL_0, DamageType.Magical)}));
        
        var vamp = attributes.HpPercentage < HP_THRESHOLD ? VAMP_2 * VAMP_MUL_WHEN_LOW_HP : VAMP_2;
        attributes.Heal(outputDmg * vamp);
        ((BattleHero)hero).Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_2);
    }
}