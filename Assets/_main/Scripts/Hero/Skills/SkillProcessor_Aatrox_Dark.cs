using System;
using Random = UnityEngine.Random;

/// <summary>
/// chem 3 lan, lan luot gay 70%/150%/200% st vat ly,
/// hat tung muc tieu 0.2s/0.2s/0.5s
/// neu muc tieu duoi 50% mau, cac don chem se gay them hieu ung
/// don 1: giam 25% giap trong 5s 
/// don 2: gay them st chuan = 50% st vat ly
/// don 3: co the chi mang
/// khi dang dung skill, khong the can pha
/// </summary>
public class SkillProcessor_Aatrox_Dark : SkillProcessor {
    const float DMG_MUL_0 = 0.7f;
    const float DMG_MUL_1 = 1.5f;
    const float DMG_MUL_2 = 2f;
    const float AIRBORNE_TIME_0 = 0.2f;
    const float AIRBORNE_TIME_1 = 0.2f;
    const float AIRBORNE_TIME_2 = 0.5f;
    const float ARMOR_REDUCE_MUL = -0.25f;
    const float ARMOR_REDUCE_DURATION = 5f;
    const string EFFECT_KEY = "aatrox_curse";
    const float BONUS_TRUE_DMG_MUL = 0.5f;
    const float HP_THRESHOLD = 0.5f;

    public SkillProcessor_Aatrox_Dark(Hero hero) : base(hero) {
        events = new Action[]{LightSlash, MediumSlash, HeavySlash};
        unstoppable = true;
    }

    void LightSlash() {
        if (hero.Target == null) return;
        
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                attributes.PhysicalDamage * DMG_MUL_0,
                DamageType.Physical,
                attributes.PhysicalPenetration
            ));
        
        if (hero.Target.GetAbility<HeroAttributes>().Hp / hero.Target.GetAbility<HeroAttributes>().MaxHp < HP_THRESHOLD) {
            hero.Target.GetAbility<HeroAttributes>().AddAttributeModifier(
                AttributeModifier.Create(
                    AttributeModifierKey.Armor,
                    ARMOR_REDUCE_MUL,
                    ModifierType.Percentage,
                    ARMOR_REDUCE_DURATION
                ));
            
            hero.Target.GetAbility<HeroMark>().AddMark(
                Mark.Create(
                    EFFECT_KEY,
                    hero,
                    1,
                    ARMOR_REDUCE_DURATION
                ));
        }
        
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_0);
        
    }

    void MediumSlash() {
        if (hero.Target == null) return;
        
        if (hero.Target.GetAbility<HeroAttributes>().Hp / hero.Target.GetAbility<HeroAttributes>().MaxHp >= HP_THRESHOLD) {
            hero.Target.GetAbility<HeroAttributes>().TakeDamage(
                Damage.Create(
                    attributes.PhysicalDamage * DMG_MUL_1,
                    DamageType.Physical,
                    attributes.PhysicalPenetration
                ));
        }
        else {
            hero.Target.GetAbility<HeroAttributes>().TakeDamage(
                new[] {
                    Damage.Create(
                        attributes.PhysicalDamage * DMG_MUL_1,
                        DamageType.Physical,
                        attributes.PhysicalPenetration
                    ),
                    Damage.Create(
                        attributes.PhysicalDamage * BONUS_TRUE_DMG_MUL,
                        DamageType.True,
                        0
                    )
                });
        }
        
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_1);
    }
    
    void HeavySlash() {
        if (hero.Target == null) return;
        
        var dmg = attributes.PhysicalDamage * DMG_MUL_2;
        var crit = Random.value < attributes.CriticalChance;
        if (crit) {
            dmg *= attributes.CriticalDamage;
        }
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                dmg,
                DamageType.Physical,
                attributes.PhysicalPenetration
            ));
    }
}