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

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        processor = hero.Trait.id switch {
            "Aatrox_Dark" => new SkillProcessor_Aatrox_Dark(hero),
            "Aatrox_Light" => new SkillProcessor_Aatrox_Light(hero),
            "Yasuo" => new SkillProcessor_Yasuo(hero),
            "Zed" => new SkillProcessor_Zed(hero),
            "Yone" => new SkillProcessor_Yone(hero),
            "Akali" => new SkillProcessor_Akali(hero),
            "Ashe" => new SkillProcessor_Ashe(hero),
            "Caitlyn" => new SkillProcessor_Caitlyn(hero),
            "Jinx" => new SkillProcessor_Jinx(hero),
            "Irelia" => new SkillProcessor_Irelia(hero),
            "Katarina" => new SkillProcessor_Katarina(hero),
            "Malphite" => new SkillProcessor_Malphite(hero),
            "MissFortune" => new SkillProcessor_MissFortune(hero),
            "Morgana" => new SkillProcessor_Morgana(hero),
            "Teemo" => new SkillProcessor_Teemo(hero),
            "Tristana" => new SkillProcessor_Tristana(hero),
        };
    }

    public override void ResetAll() {
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
        rotation.Rotate(hero.Target.transform.position - hero.transform.position);
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