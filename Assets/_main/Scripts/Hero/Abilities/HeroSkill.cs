using DG.Tweening;
using UnityEngine;

public class HeroSkill : HeroAbility {
    public bool IsUsingSkill => isUsingSkill;

    HeroAttributes attributes;
    HeroRotation rotation;
    HeroStatusEffects effects;
    HeroAttack attack;
    
    SkillProcessor processor;
    bool isUsingSkill;
    Tween resetUsingSkillTween;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        processor = hero.Trait.id switch {
            "Aatrox" => new SkillProcessor_Aatrox(hero),
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
        resetUsingSkillTween?.Kill();
    }

    public override void Process() {
        processor.Process();
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
        rotation.Rotate(hero.Target.transform.position - hero.transform.position);
        processor.Execute(out var duration);
        resetUsingSkillTween?.Kill();
        resetUsingSkillTween = DOVirtual.DelayedCall(duration, () => {
            isUsingSkill = false;
            attack.RefreshAttackCooldown();
        });
        return true;
    }

    public void Interrupt() {
        isUsingSkill = false;
        processor.Cancel();
        resetUsingSkillTween?.Kill();
    }
    
    bool BlockedByOtherActions() {
        return false;
    }
    
    bool BlockedByStatusEffects() {
        return effects.IsAirborne || effects.IsStun || effects.IsSilent;
    }
}