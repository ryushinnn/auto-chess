using DG.Tweening;
using UnityEngine;

public class HeroSkill : HeroAbility {
    public bool IsUsingSkill => isUsingSkill;
    
    SkillProcessor processor;
    bool isUsingSkill;
    Tween resetUsingSkillTween;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        processor = hero.Trait.id switch {
            "Aatrox" => new SkillProcessor_Aatrox(hero),
            "Yasuo" => new SkillProcessor_Yasuo(hero),
        };
    }

    public override void Process() {
        processor.Process();
    }

    public bool UseSkill() {
        if (hero.GetAbility<HeroAttributes>().Energy < HeroTrait.MAX_ENERGY 
            || isUsingSkill
            || BlockedByOtherActions()
            || BlockedByStatusEffects()) return false;
        
        isUsingSkill = true;
        hero.GetAbility<HeroRotation>().Rotate(hero.Target.transform.position - hero.transform.position);
        processor.Execute(out var duration);
        resetUsingSkillTween?.Kill();
        resetUsingSkillTween = DOVirtual.DelayedCall(duration, () => {
            isUsingSkill = false;
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
        return hero.GetAbility<HeroStatusEffects>().IsAirborne
               || hero.GetAbility<HeroStatusEffects>().IsStun
               || hero.GetAbility<HeroStatusEffects>().IsSilent;
    }
}