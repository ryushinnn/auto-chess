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
        processor = hero.Trait.id switch {
            HeroId.Aatrox_Dark => new SkillProcessor_Aatrox_Dark(hero),
            HeroId.Aatrox_Light => new SkillProcessor_Aatrox_Light(hero),
            HeroId.Akali => new SkillProcessor_Akali(hero),
            HeroId.Ashe => new SkillProcessor_Ashe(hero),
            HeroId.Caitlyn => new SkillProcessor_Caitlyn(hero),
            HeroId.Irelia => new SkillProcessor_Irelia(hero),
            HeroId.Jinx => new SkillProcessor_Jinx(hero),
            HeroId.Katarina => new SkillProcessor_Katarina(hero),
            HeroId.Malphite => new SkillProcessor_Malphite(hero),
            HeroId.MissFortune => new SkillProcessor_MissFortune(hero),
            HeroId.Morgana => new SkillProcessor_Morgana(hero),
            HeroId.Teemo => new SkillProcessor_Teemo(hero),
            HeroId.Tristana => new SkillProcessor_Tristana(hero),
            HeroId.Yasuo => new SkillProcessor_Yasuo(hero),
            HeroId.Yone => new SkillProcessor_Yone(hero),
            HeroId.Zed => new SkillProcessor_Zed(hero),
            _ => new SkillProcessor(hero),
        };
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