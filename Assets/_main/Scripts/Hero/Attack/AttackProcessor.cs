using System;
using Random = UnityEngine.Random;

[Serializable]
public class AttackProcessor {
    public CustomData<int> CustomInt => customInt;
    
    protected readonly Hero hero;
    protected readonly HeroAttributes attributes;
    protected CustomData<int> customInt;
    
    public AttackProcessor(Hero hero) {
        this.hero = hero;
        attributes = this.hero.GetAbility<HeroAttributes>();
    }

    public virtual void Process() {
        
    }

    public virtual void Execute() {
        CalculateDamage(out var damage);
        hero.Mecanim.Attack(new Action[]{
            () => {
                if (hero.Target == null) return;
                var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(damage);
                var heal = outputDamage * attributes.LifeSteal;
                if (heal > 0) {
                    attributes.Heal(heal);
                }
                attributes.RegenEnergy(hero.Trait.energyRegenPerAttack);
            }
        });
    }
    
    protected virtual void CalculateDamage(out Damage damage) {
        damage = new Damage();
        damage.value = attributes.PhysicalDamage + attributes.MagicalDamage;
        if (attributes.PhysicalDamage > attributes.MagicalDamage) {
            damage.type = DamageType.Physical;
            damage.penetration = attributes.PhysicalPenetration;
        }
        else if (attributes.PhysicalDamage < attributes.MagicalDamage) {
            damage.type = DamageType.Magical;
            damage.penetration = attributes.MagicalPenetration;
        }
        else {
            if (attributes.PhysicalPenetration > attributes.MagicalPenetration) {
                damage.type = DamageType.Physical;
                damage.penetration = attributes.PhysicalPenetration;
            }
            else {
                damage.type = DamageType.Magical;
                damage.penetration = attributes.MagicalPenetration;
            }
        }

        damage.crit = attributes.Crit();
        if (damage.crit) {
            damage.value *= attributes.CriticalDamage;
        }
    }
}