using System;
using UnityEngine;

/// <summary>
/// chem 3 lan, lan luot gay 70%/150%/200% st vat ly,
/// hat tung muc tieu 0.2s/0.2s/0.25s
/// va hoi mau bang 25%/50%/100% st gay ra
/// neu mau hien tai thap hon 50%, luong mau hoi phuc x2
/// khi dang dung skill, khong the can pha
/// </summary>
public class SkillProcessor_Aatrox_Dark : SkillProcessor {
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

    public SkillProcessor_Aatrox_Dark(Hero hero) : base(hero) {
        events = new Action[]{LightSlash, MediumSlash, HeavySlash};
        unstoppable = true;
    }

    void LightSlash() {
        if (hero.Target == null) return;

        var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                attributes.PhysicalDamage * DMG_MUL_0,
                DamageType.Physical,
                attributes.PhysicalPenetration
            ));
        
        var vamp = attributes.Hp / attributes.MaxHp < 0.5f ? VAMP_0 * VAMP_MUL_WHEN_LOW_HP : VAMP_0;
        attributes.Heal(outputDmg * vamp);
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_0);
    }
    
    void MediumSlash() {
        if (hero.Target == null) return;
        
        var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                attributes.PhysicalDamage * DMG_MUL_1,
                DamageType.Physical, 
                attributes.PhysicalPenetration
            ));
        
        var vamp = attributes.Hp / attributes.MaxHp < 0.5f ? VAMP_1 * VAMP_MUL_WHEN_LOW_HP : VAMP_1;
        attributes.Heal(outputDmg * vamp);
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_1);
    }
    
    void HeavySlash() {
        if (hero.Target == null) return;
        
        var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                attributes.PhysicalDamage * DMG_MUL_2, 
                DamageType.Physical, 
                attributes.PhysicalPenetration
            ));
        
        var vamp = attributes.Hp / attributes.MaxHp < 0.5f ? VAMP_2 * VAMP_MUL_WHEN_LOW_HP : VAMP_2;
        attributes.Heal(outputDmg * vamp);
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_2);
    }
}