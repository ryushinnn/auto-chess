using System;
using UnityEngine;

/// <summary>
/// chem 3 lan, lan luot gay 70%/150%/200% st vat ly,
/// hat tung muc tieu 0.2s/0.2s/0.25s
/// va hoi mau bang 25%/50%/100% st gay ra
/// neu mau hien tai thap hon 50%, luong mau hoi phuc x2
/// </summary>
public class SkillProcessor_Aatrox : SkillProcessor {
    Hero hero;

    const float DMG_MUL_0 = 0.7f;
    const float DMG_MUL_1 = 1.5f;
    const float DMG_MUL_2 = 2f;
    const float VAMP_0 = 0.25f;
    const float VAMP_1 = 0.5f;
    const float VAMP_2 = 1f;
    const float VAMP_MUL_WHEN_LOW_HP = 2f;
    const float AIRBORNE_TIME_0 = 0.2f;
    const float AIRBORNE_TIME_1 = 0.2f;
    const float AIRBORNE_TIME_2 = 0.25f;

    public SkillProcessor_Aatrox(Hero hero) : base(hero) {
        this.hero = hero;
        events = new Action[]{LightSlash, MediumSlash, HeavySlash};
        unstoppable = true;
    }

    void LightSlash() {
        if (hero.Target == null) return;

        var attribute = hero.GetAbility<HeroAttributes>();
        var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            attribute.PhysicalDamage * DMG_MUL_0, 
            DamageType.Physical, 
            attribute.PhysicalPenetration);
        
        var vamp = attribute.Hp / attribute.MaxHp < 0.5f ? VAMP_0 * VAMP_MUL_WHEN_LOW_HP : VAMP_0;
        attribute.Heal(outputDmg * vamp);
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_0);
    }
    
    void MediumSlash() {
        if (hero.Target == null) return;
        
        var attribute = hero.GetAbility<HeroAttributes>();
        var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            attribute.PhysicalDamage * DMG_MUL_1,
            DamageType.Physical, 
            attribute.PhysicalPenetration);
        
        var vamp = attribute.Hp / attribute.MaxHp < 0.5f ? VAMP_1 * VAMP_MUL_WHEN_LOW_HP : VAMP_1;
        attribute.Heal(outputDmg * vamp);
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_1);
    }
    
    void HeavySlash() {
        if (hero.Target == null) return;
        
        var attribute = hero.GetAbility<HeroAttributes>();
        var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            attribute.PhysicalDamage * DMG_MUL_2, 
            DamageType.Physical, 
            attribute.PhysicalPenetration);
        
        var vamp = attribute.Hp / attribute.MaxHp < 0.5f ? VAMP_2 * VAMP_MUL_WHEN_LOW_HP : VAMP_2;
        attribute.Heal(outputDmg * vamp);
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_2);
    }
}