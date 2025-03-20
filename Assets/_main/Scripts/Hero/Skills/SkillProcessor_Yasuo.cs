using System;
using RExt.Extension;
using UnityEngine;

/// <summary>
/// hat tung ke dich 1s sau do gay lan luot 100%/150% st vat ly
/// don chem thu 2 kem theo hieu ung chay mau, gay st 10% st vat ly, 90% xuyen giap moi 0.33s
/// duy tri 4s 
/// </summary>
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
        events = new Action[]{BlowUp,Cut,BonusCut};
    }

    void BlowUp() {
        if (hero.Target == null) return;

        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_DURATION);
    }

    void Cut() {
        if (hero.Target == null) return;

        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                attributes.PhysicalDamage * DMG_MUL_0,
                DamageType.Physical,
                attributes.PhysicalPenetration
            ));
    }

    void BonusCut() {
        if (hero.Target == null) return;
        
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                attributes.PhysicalDamage * DMG_MUL_1,
                DamageType.Physical,
                attributes.PhysicalPenetration
            ));

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