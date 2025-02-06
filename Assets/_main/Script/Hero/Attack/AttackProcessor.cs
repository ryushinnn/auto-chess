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
        hero.Mecanim.Attack(() => {
            if (hero.Target == null) return;
            var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg, type, pen);
            hero.GetAbility<HeroAttributes>().Heal(outputDamage * hero.GetAbility<HeroAttributes>().LifeSteal);
            hero.GetAbility<HeroAttributes>().RegenEnergy(hero.Trait.energyRegenPerAttack);
        });
    }
    
    protected virtual void CalculateDamage(out float damage, out DamageType type, out float penetration) {
        if (hero.GetAbility<HeroAttributes>().PhysicalDamage > hero.GetAbility<HeroAttributes>().MagicalDamage) {
            damage = hero.GetAbility<HeroAttributes>().PhysicalDamage;
            type = DamageType.Physical;
            penetration = hero.GetAbility<HeroAttributes>().PhysicalPenetration;
        }
        else if (hero.GetAbility<HeroAttributes>().PhysicalDamage < hero.GetAbility<HeroAttributes>().MagicalDamage) {
            damage = hero.GetAbility<HeroAttributes>().MagicalDamage;
            type = DamageType.Magical;
            penetration = hero.GetAbility<HeroAttributes>().MagicalPenetration;
        }
        else {
            if (hero.GetAbility<HeroAttributes>().PhysicalPenetration > hero.GetAbility<HeroAttributes>().MagicalPenetration) {
                damage = hero.GetAbility<HeroAttributes>().PhysicalDamage;
                type = DamageType.Physical;
                penetration = hero.GetAbility<HeroAttributes>().PhysicalPenetration;
            }
            else {
                damage = hero.GetAbility<HeroAttributes>().MagicalDamage;
                type = DamageType.Magical;
                penetration = hero.GetAbility<HeroAttributes>().MagicalPenetration;
            }
        }

        if (Random.value < hero.GetAbility<HeroAttributes>().CriticalChance) {
            damage *= hero.GetAbility<HeroAttributes>().CriticalDamage;
        }
    }
}