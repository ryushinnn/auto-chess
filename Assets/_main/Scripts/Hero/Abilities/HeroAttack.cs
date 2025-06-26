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
    [SerializeField, ReadOnly] float timer;
    [SerializeField, ReadOnly] bool isAttacking;
    [SerializeField, ReadOnly] float duration;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        processor = hero.Trait.id switch {
            "Aatrox_Dark" => new AttackProcessor_Aatrox_Dark(hero),
            "Aatrox_Light" => new AttackProcessor_Aatrox_Light(hero),
            "Yasuo" => new AttackProcessor_Yasuo(hero),
            "Yone" => new AttackProcessor_Yone(hero),
            "MissFortune" => new AttackProcessor_MissFortune(hero),
            "Teemo" => new AttackProcessor_Teemo(hero),
            _ => new AttackProcessor(hero)
        };
        
        Debug.Log(processor.Description);
    }

    public override void ResetAll() {
        currentAttackCooldown = 0;
    }

    public override void Process() {
        HandleCooldown();
        HandleAttack();
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

        isAttacking = true;
        timer = 0;
        rotation.Rotate(hero.Target.transform.position - hero.transform.position);
        currentAttackCooldown = 1 / attributes.AttackSpeed;
        processor.Begin(out duration);
        hero.Mecanim.Attack();
        return true;
    }

    public void Interrupt() {
        isAttacking = false;
        hero.Mecanim.InterruptAttack();
        currentAttackCooldown = 1 / attributes.AttackSpeed;
        processor.End(false);
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

    void HandleAttack() {
        if (!isAttacking) return;

        timer += Time.deltaTime;
        processor.Process(timer);
        
        if (timer > duration) {
            isAttacking = false;
            processor.End(true);
        }
    }

    void HandleCooldown() {
        if (currentAttackCooldown > 0) {
            currentAttackCooldown -= Time.deltaTime;
        }
    }
}