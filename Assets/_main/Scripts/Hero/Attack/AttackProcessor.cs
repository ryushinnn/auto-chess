using System;
using Random = UnityEngine.Random;

[Serializable]
public class AttackProcessor {
    public CustomData<int> CustomInt => customInt;
    
    protected Hero hero;
    protected CustomData<int> customInt;
    protected HeroAttributes attributes;
    
    public AttackProcessor(Hero hero) {
        this.hero = hero;
        attributes = this.hero.GetAbility<HeroAttributes>();
    }

    public virtual void Process() {
        
    }

    public virtual void Execute() {
        CalculateDamage(out var dmg, out var type, out var pen, out var crit);
        hero.Mecanim.Attack(() => {
            if (hero.Target == null) return;
            var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg, type, pen, crit);
            var heal = outputDamage * attributes.LifeSteal;
            if (heal > 0) {
                attributes.Heal(heal);
            }
            attributes.RegenEnergy(hero.Trait.energyRegenPerAttack);
        });
    }
    
    protected virtual void CalculateDamage(out float damage, out DamageType type, out float penetration, out bool crit) {
        if (attributes.PhysicalDamage > attributes.MagicalDamage) {
            damage = attributes.PhysicalDamage;
            type = DamageType.Physical;
            penetration = attributes.PhysicalPenetration;
        }
        else if (attributes.PhysicalDamage < attributes.MagicalDamage) {
            damage = attributes.MagicalDamage;
            type = DamageType.Magical;
            penetration = attributes.MagicalPenetration;
        }
        else {
            if (attributes.PhysicalPenetration > attributes.MagicalPenetration) {
                damage = attributes.PhysicalDamage;
                type = DamageType.Physical;
                penetration = attributes.PhysicalPenetration;
            }
            else {
                damage = attributes.MagicalDamage;
                type = DamageType.Magical;
                penetration = attributes.MagicalPenetration;
            }
        }

        crit = Random.value < attributes.CriticalChance;
        if (crit) {
            damage *= attributes.CriticalDamage;
        }
    }
}