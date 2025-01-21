using System;
using UnityEngine;

public class Skill_Aatrox : Skill {
    Hero hero;

    const float DMG_MUL_0 = 0.7f;
    const float DMG_MUL_1 = 1.5f;
    const float DMG_MUL_2 = 2f;
    const float VAMP_0 = 0.25f;
    const float VAMP_1 = 0.25f;
    const float VAMP_2 = 1f;
    const float AIRBORNE_TIME_0 = 0.2f;
    const float AIRBORNE_TIME_1 = 0.2f;
    const float AIRBORNE_TIME_2 = 0.25f;

    public Skill_Aatrox(Hero hero) {
        this.hero = hero;

        events = new Action[]{LightSlash, MediumSlash, HeavySlash};
        unstoppable = true;
    }

    void LightSlash() {
        var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            hero.GetAbility<HeroAttributes>().PhysicalDamage * DMG_MUL_0, 
            DamageType.Physical, 
            hero.GetAbility<HeroAttributes>().PhysicalPenetration);
        hero.GetAbility<HeroAttributes>().Heal(outputDmg * VAMP_0);
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_0);
    }
    
    void MediumSlash() {
        var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            hero.GetAbility<HeroAttributes>().PhysicalDamage * DMG_MUL_1,
            DamageType.Physical, 
            hero.GetAbility<HeroAttributes>().PhysicalPenetration);
        hero.GetAbility<HeroAttributes>().Heal(outputDmg * VAMP_1);
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_1);
    }
    
    void HeavySlash() {
        var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            hero.GetAbility<HeroAttributes>().PhysicalDamage * DMG_MUL_2, 
            DamageType.Physical, 
            hero.GetAbility<HeroAttributes>().PhysicalPenetration);
        hero.GetAbility<HeroAttributes>().Heal(outputDmg * VAMP_2);
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_2);
    }
}