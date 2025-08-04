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

    public override void ResetAll() {
        processor = hero.Trait.id switch {
            HeroId.Aatrox_Dark => new AttackProcessor_Aatrox_Dark(hero),
            HeroId.Aatrox_Light => new AttackProcessor_Aatrox_Light(hero),
            HeroId.Akali => new AttackProcessor_Akali(hero),
            HeroId.Ashe => new AttackProcessor_Ashe(hero),
            HeroId.Caitlyn => new AttackProcessor_Caitlyn(hero),
            HeroId.Irelia => new AttackProcessor_Irelia(hero),
            HeroId.Jinx => new AttackProcessor_Jinx(hero),
            HeroId.Katarina => new AttackProcessor_Katarina(hero),
            HeroId.Malphite => new AttackProcessor_Malphite(hero),
            HeroId.MissFortune => new AttackProcessor_MissFortune(hero),
            HeroId.Morgana => new AttackProcessor_Morgana(hero),
            HeroId.Teemo => new AttackProcessor_Teemo(hero),
            HeroId.Tristana => new AttackProcessor_Tristana(hero),
            HeroId.Yasuo => new AttackProcessor_Yasuo(hero),
            HeroId.Yone => new AttackProcessor_Yone(hero),
            HeroId.Zed => new AttackProcessor_Zed(hero),
            _ => new AttackProcessor(hero)
        };
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