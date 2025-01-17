using UnityEngine;

public class HeroAttack : HeroAbility {
    public int AttackRange => attackRange;
    
    int attackRange;
    float attackCooldown;
    float currentAttackCooldown;
    float energyRegenPerAttack;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        attackRange = this.hero.Trait.attackRange;
        attackCooldown = 1 / this.hero.Trait.attackSpeed;
        energyRegenPerAttack = this.hero.Trait.energyRegenPerAttack;
    }

    public override void Process() {
        if (currentAttackCooldown > 0) {
            currentAttackCooldown -= Time.deltaTime;
        }
    }

    public bool Attack() {
        if (currentAttackCooldown > 0) return false;
        
        Debug.Log("attack");
        hero.GetAbility<HeroSkill>().RegenEnergy(energyRegenPerAttack);
        currentAttackCooldown = attackCooldown;
        return true;
    }
}