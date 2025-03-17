using Sirenix.OdinInspector;
using UnityEngine;

public enum DamageType {
    Physical,
    Magical,
    True
}

public class HeroAttack : HeroAbility {
    public AttackProcessor Processor => processor;
    
    HeroAttributes attributes;
    HeroRotation rotation;
    HeroSkill skill;
    HeroStatusEffects effects;
    
    AttackProcessor processor;
    [SerializeField, ReadOnly] float currentAttackCooldown;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        processor = hero.Trait.id switch {
            "Yasuo" => new AttackProcessor_Yasuo(hero),
            "Yone" => new AttackProcessor_Yone(hero),
            "MissFortune" => new AttackProcessor_MissFortune(hero),
            "Teemo" => new AttackProcessor_Teemo(hero),
            _ => new AttackProcessor(hero)
        };
    }

    public override void ResetAll() {
        currentAttackCooldown = 0;
    }

    public override void Process() {
        if (currentAttackCooldown > 0) {
            currentAttackCooldown -= Time.deltaTime;
        }
        processor.Process();
    }

    protected override void FindReferences() {
        attributes = hero.GetAbility<HeroAttributes>();
        rotation = hero.GetAbility<HeroRotation>();
        skill = hero.GetAbility<HeroSkill>();
        effects = hero.GetAbility<HeroStatusEffects>();
    }

    public bool Attack() {
        if (currentAttackCooldown > 0 
            || BlockedByOtherActions()
            || BlockedByStatusEffects()) return false;
        
        rotation.Rotate(hero.Target.transform.position - hero.transform.position);
        processor.Execute();
        currentAttackCooldown = 1 / attributes.AttackSpeed;
        return true;
    }

    public void Interrupt() {
        hero.Mecanim.InterruptAttack();
        currentAttackCooldown = 1 / attributes.AttackSpeed;
    }

    public void RefreshAttackCooldown() {
        currentAttackCooldown = Mathf.Min(currentAttackCooldown, 1 / attributes.AttackSpeed);
    }

    bool BlockedByOtherActions() {
        return skill.IsUsingSkill;
    }

    bool BlockedByStatusEffects() {
        return effects.IsAirborne || effects.IsStun;
    }
}