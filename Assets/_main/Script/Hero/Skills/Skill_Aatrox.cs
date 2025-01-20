using System;
using UnityEngine;

public class Skill_Aatrox : Skill {
    Hero hero;

    public Skill_Aatrox(Hero hero) {
        this.hero = hero;

        events = new Action[]{LightSlash, MediumSlash, HeavySlash};
    }

    void LightSlash() {
        hero.Target.GetAbility<HeroHealth>().TakeDamage(99, DamageType.Physical, 0);
    }
    
    void MediumSlash() {
        hero.Target.GetAbility<HeroHealth>().TakeDamage(199, DamageType.Physical, 0);
    }
    
    void HeavySlash() {
        hero.Target.GetAbility<HeroHealth>().TakeDamage(299, DamageType.Physical, 0);
    }
}