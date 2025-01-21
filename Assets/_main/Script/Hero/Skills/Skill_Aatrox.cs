using System;
using UnityEngine;

public class Skill_Aatrox : Skill {
    Hero hero;

    public Skill_Aatrox(Hero hero) {
        this.hero = hero;

        events = new Action[]{LightSlash, MediumSlash, HeavySlash};
        unstoppable = true;
    }

    void LightSlash() {
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(40, DamageType.Physical, 0);
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(0.2f);
    }
    
    void MediumSlash() {
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(60, DamageType.Physical, 0);
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(0.2f);
    }
    
    void HeavySlash() {
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(200, DamageType.Physical, 0);
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(0.25f);
    }
}