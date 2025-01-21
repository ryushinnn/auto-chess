using UnityEngine;

public class HeroAttack : HeroAbility {
    public int AttackRange => attackRange;
    
    int attackRange;
    float attackCooldown;
    float currentAttackCooldown;
    float energyRegenPerAttack;
    float physicalDamage;
    float magicalPower;
    float physicalPenetration;
    float magicalPenetration;
    float lifeSteal;
    float criticalChance;
    float criticalDamage;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        attackRange = this.hero.Trait.attackRange;
        attackCooldown = 1 / this.hero.Trait.attackSpeed;
        energyRegenPerAttack = this.hero.Trait.energyRegenPerAttack;
        physicalDamage = this.hero.Trait.physicalDamage;
        magicalPower = this.hero.Trait.magicalPower;
        physicalPenetration = this.hero.Trait.physicalPenetration;
        magicalPenetration = this.hero.Trait.magicalPenetration;
        lifeSteal = this.hero.Trait.lifeSteal;
        criticalChance = this.hero.Trait.criticalChance;
        criticalDamage = this.hero.Trait.criticalDamage;
    }

    public override void Process() {
        if (currentAttackCooldown > 0) {
            currentAttackCooldown -= Time.deltaTime;
        }
    }

    public bool Attack() {
        if (currentAttackCooldown > 0 
            || hero.GetAbility<HeroSkill>().IsUsingSkill
            || hero.GetAbility<HeroStatusEffects>().IsAirborne
            || hero.GetAbility<HeroStatusEffects>().IsStun) return false;
        
        hero.GetAbility<HeroRotation>().Rotate(hero.Target.transform.position - hero.transform.position);
        CalculateDamage(out var dmg, out var type, out var pen);
        hero.Mecanim.Attack(() => {
            var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg, type, pen);
            hero.GetAbility<HeroAttributes>().Heal(outputDamage * lifeSteal);
            hero.GetAbility<HeroSkill>().RegenEnergy(energyRegenPerAttack);
        });
        currentAttackCooldown = attackCooldown;
        return true;
    }

    public void Interrupt() {
        hero.Mecanim.InterruptAttack();
        currentAttackCooldown = attackCooldown;
    }

    void CalculateDamage(out float damage, out DamageType type, out float penetration) {
        if (physicalDamage > magicalPower) {
            damage = physicalDamage;
            type = DamageType.Physical;
            penetration = physicalPenetration;
        }
        else if (physicalDamage < magicalPower) {
            damage = magicalPower;
            type = DamageType.Magical;
            penetration = magicalPenetration;
        }
        else {
            if (physicalPenetration > magicalPenetration) {
                damage = physicalDamage;
                type = DamageType.Physical;
                penetration = physicalPenetration;
            }
            else {
                damage = magicalPower;
                type = DamageType.Magical;
                penetration = magicalPenetration;
            }
        }

        if (Random.value < criticalChance) {
            damage *= criticalDamage;
        }
    }
}

public enum DamageType {
    Physical,
    Magical,
    Pure
}