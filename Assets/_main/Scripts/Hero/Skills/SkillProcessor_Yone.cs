using System;
using Random = UnityEngine.Random;

/// <summary>
/// than kiem: lao toi chem 1 nhat gay 250% st vat ly, hat tung muc tieu 1.5s
/// neu tieu diet ke dich, hoi phuc 50 nang luong
/// quy kiem: chem nhanh 4 nhat, 3 nhat dau gay 50% st vat ly, phat cuoi gay 250% st vat ly
/// cac nhat chem nay co the chi mang
/// </summary>
public class SkillProcessor_Yone : SkillProcessor {
    const float DIVINE_DMG_MUL = 2.5f;
    const float DIVINE_AIRBORNE_TIME = 1.5f;
    const float DIVINE_REGEN_ENERGY = 50f;
    const float DEVIL_DMG_MUL_0 = 0.5f;
    const float DEVIL_DMG_MUL_1 = 2.5f;
    
    public SkillProcessor_Yone(Hero hero) : base(hero) {
        events = new Action[] { Judge, LightSmite, HeavySmite };
    }

    void Judge() {
        if (hero.Target == null) return;
        
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                attributes.PhysicalDamage * DIVINE_DMG_MUL,
                DamageType.Physical, 
                attributes.PhysicalPenetration
            ));
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(DIVINE_AIRBORNE_TIME);
        
        if (!hero.Target.GetAbility<HeroAttributes>().IsAlive){
            attributes.RegenEnergy(DIVINE_REGEN_ENERGY);
        }
    }

    void LightSmite() {
        if (hero.Target == null) return;
        
        var dmg = attributes.PhysicalDamage * DEVIL_DMG_MUL_0;
        if (Random.value < attributes.CriticalChance) {
            dmg *= attributes.CriticalDamage;
        }
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                dmg,
                DamageType.Physical, 
                attributes.PhysicalPenetration
            ));
    }

    void HeavySmite() {
        if (hero.Target == null) return;
        
        var dmg = attributes.PhysicalDamage * DEVIL_DMG_MUL_1;
        if (Random.value < attributes.CriticalChance) {
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