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

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        attackRange = this.hero.Trait.attackRange;
        attackCooldown = 1 / this.hero.Trait.attackSpeed;
        energyRegenPerAttack = this.hero.Trait.energyRegenPerAttack;
        physicalDamage = this.hero.Trait.physicalDamage;
        magicalPower = this.hero.Trait.magicalPower;
        physicalPenetration = this.hero.Trait.physicalPenetration;
        magicalPenetration = this.hero.Trait.magicalPenetration;
    }

    public override void Process() {
        if (hero.GetAbility<HeroSkill>().IsUsingSkill) return;
        
        if (currentAttackCooldown > 0) {
            currentAttackCooldown -= Time.deltaTime;
        }
    }

    public bool Attack() {
        if (currentAttackCooldown > 0) return false;
        
        Debug.Log("attack");
        hero.Mecanim.DoAction(Mecanim.Action.Skill, (Animator.StringToHash("skill"), 0));
        hero.GetAbility<HeroRotation>().Rotate(hero.Target.transform.position - hero.transform.position);
        hero.GetAbility<HeroSkill>().RegenEnergy(energyRegenPerAttack);
        GetDamage(out var dmg, out var type, out var pen);
        hero.Target.GetAbility<HeroHealth>().TakeDamage(dmg, type, pen);
        currentAttackCooldown = attackCooldown;
        return true;
    }

    void GetDamage(out float damage, out DamageType type, out float penetration) {
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
    }
}

public enum DamageType {
    Physical,
    Magical,
    Pure
}