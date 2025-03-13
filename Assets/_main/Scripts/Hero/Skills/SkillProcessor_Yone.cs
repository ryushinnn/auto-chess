using System;
using Random = UnityEngine.Random;

/// <summary>
/// than kiem: lao toi chem 1 nhat gay 225%d st vat ly, hat tung muc tieu 1s
/// neu tieu diet ke dich, hoi phuc 50 nang luong
/// quy kiem: chem nhanh 4 nhat, 3 nhat dau gay 50% st vat ly, phat cuoi gay 150% st vat ly
/// cac nhat chem nay co the chi mang
/// </summary>
public class SkillProcessor_Yone : SkillProcessor {
    const float DIVINE_DMG_MUL = 2.25f;
    const float DIVINE_AIRBORNE_TIME = 1f;
    const float DIVINE_REGEN_ENERGY = 50f;
    const float DEVIL_DMG_MUL_0 = 0.5f;
    const float DEVIL_DMG_MUL_1 = 1.5f;
    
    public SkillProcessor_Yone(Hero hero) : base(hero) {
        this.hero = hero;
        events = new Action[] { Judge, LightSmite, HeavySmite };
        unstoppable = false;
    }

    void Judge() {
        if (hero.Target == null) return;
        
        var attribute = hero.GetAbility<HeroAttributes>();
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            attribute.PhysicalDamage * DIVINE_DMG_MUL,
            DamageType.Physical, 
            attribute.PhysicalPenetration);
        
        if (!hero.Target.GetAbility<HeroAttributes>().IsAlive){
            attribute.RegenEnergy(DIVINE_REGEN_ENERGY);
        }
    }

    void LightSmite() {
        if (hero.Target == null) return;
        
        var attribute = hero.GetAbility<HeroAttributes>();
        var dmg = attribute.PhysicalDamage * DEVIL_DMG_MUL_0;
        if (Random.value < attribute.CriticalChance) {
            dmg *= attribute.CriticalDamage;
        }
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            dmg,
            DamageType.Physical, 
            attribute.PhysicalPenetration);
    }

    void HeavySmite() {
        if (hero.Target == null) return;
        
        var attribute = hero.GetAbility<HeroAttributes>();
        var dmg = attribute.PhysicalDamage * DEVIL_DMG_MUL_1;
        if (Random.value < attribute.CriticalChance) {
            dmg *= attribute.CriticalDamage;
        }
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            dmg,
            DamageType.Physical, 
            attribute.PhysicalPenetration);
    }
}