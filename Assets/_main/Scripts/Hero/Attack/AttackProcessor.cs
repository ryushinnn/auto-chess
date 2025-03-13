using UnityEngine;

public class AttackProcessor {
    protected Hero hero;
    
    public AttackProcessor(Hero hero) {
        this.hero = hero;
    }

    public virtual void Process() {
        
    }

    public virtual void Execute() {
        CalculateDamage(out var dmg, out var type, out var pen);
        var attributes = hero.GetAbility<HeroAttributes>();
        hero.Mecanim.Attack(() => {
            if (hero.Target == null) return;
            var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg, type, pen);
            attributes.Heal(outputDamage * attributes.LifeSteal);
            attributes.RegenEnergy(hero.Trait.energyRegenPerAttack);
        });
    }
    
    protected virtual void CalculateDamage(out float damage, out DamageType type, out float penetration) {
        var attributes = hero.GetAbility<HeroAttributes>();
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

        if (Random.value < attributes.CriticalChance) {
            damage *= attributes.CriticalDamage;
        }
    }
}