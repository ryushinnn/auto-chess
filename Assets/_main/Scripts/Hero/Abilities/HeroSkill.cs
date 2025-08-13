using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroSkill : HeroAbility {
    public bool IsUsingSkill => isUsingSkill;
    public SkillProcessor Processor => processor;

    HeroAttributes attributes;
    HeroRotation rotation;
    HeroStatusEffects effects;
    HeroAttack attack;
    
    SkillProcessor processor;
    [SerializeField, ReadOnly] float timer;
    [SerializeField, ReadOnly] bool isUsingSkill;
    [SerializeField, ReadOnly] float duration;

    public override void ResetAll() {
        processor = SkillProcessorFactory.Create(hero);
        isUsingSkill = false;
    }

    public override void Process() {
        HandleSkill();
    }

    protected override void FindReferences() {
        attributes = hero.GetAbility<HeroAttributes>();
        rotation = hero.GetAbility<HeroRotation>();
        effects = hero.GetAbility<HeroStatusEffects>();
        attack = hero.GetAbility<HeroAttack>();
    }

    public bool UseSkill() {
        if (attributes.Energy < HeroTrait.MAX_ENERGY 
            || isUsingSkill
            || BlockedByOtherActions()
            || BlockedByStatusEffects()) return false;
        
        isUsingSkill = true;
        timer = 0;
        rotation.Rotate(((BattleHero)hero).Target.transform.position - hero.transform.position);
        processor.Begin(out duration);
        hero.Mecanim.UseSkill();
        return true;
    }

    public void Interrupt() {
        isUsingSkill = false;
        hero.Mecanim.InterruptSkill();
        processor.End(false);
    }
    
    bool BlockedByOtherActions() {
        return false;
    }
    
    bool BlockedByStatusEffects() {
        return effects.IsAirborne || effects.IsStun || effects.IsSilent;
    }

    void HandleSkill() {
        if (!isUsingSkill) return;

        timer += Time.deltaTime;
        processor.Process(timer);

        if (timer > duration) {
            isUsingSkill = false;
            processor.End(true);
        }
    }
}