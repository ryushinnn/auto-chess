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
        currentAttackCooldown = attackCooldown;
        return true;
    }
}