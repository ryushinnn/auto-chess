using UnityEngine;

public class HeroAttack : HeroAbility {
    AttackProcessor processor;
    float currentAttackCooldown;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        processor = hero.Trait.id switch {
            "Yasuo" => new AttackProcessor_Yasuo(hero),
            _ => new AttackProcessor(hero)
        };
    }

    public override void Process() {
        if (currentAttackCooldown > 0) {
            currentAttackCooldown -= Time.deltaTime;
        }
        processor.Process();
    }

    public bool Attack() {
        if (currentAttackCooldown > 0 
            || BlockedByOtherActions()
            || BlockedByStatusEffects()) return false;
        
        hero.GetAbility<HeroRotation>().Rotate(hero.Target.transform.position - hero.transform.position);
        processor.Execute();
        currentAttackCooldown = 1 / hero.GetAbility<HeroAttributes>().AttackSpeed;
        return true;
    }

    public void Interrupt() {
        hero.Mecanim.InterruptAttack();
        currentAttackCooldown = 1 / hero.GetAbility<HeroAttributes>().AttackSpeed;
    }

    public void RefreshAttackCooldown() {
        currentAttackCooldown = Mathf.Min(currentAttackCooldown, 1 / hero.GetAbility<HeroAttributes>().AttackSpeed);
    }

    bool BlockedByOtherActions() {
        return hero.GetAbility<HeroSkill>().IsUsingSkill;
    }

    bool BlockedByStatusEffects() {
        return hero.GetAbility<HeroStatusEffects>().IsAirborne 
               || hero.GetAbility<HeroStatusEffects>().IsStun;
    }
}

public enum DamageType {
    Physical,
    Magical,
    True
}