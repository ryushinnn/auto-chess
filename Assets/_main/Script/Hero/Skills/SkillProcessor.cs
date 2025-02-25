using System;
using DG.Tweening;

public abstract class SkillProcessor {
    protected Hero hero;
    
    protected Action[] events;
    protected bool unstoppable;
    
    protected Tween resetUnstoppableTween;

    protected SkillProcessor(Hero hero) {
        this.hero = hero;
    }
    
    public virtual void Process() {
        
    }
    
    public virtual void Execute(out float duration) {
        duration = hero.Mecanim.UseSkill(events);
        if (unstoppable) {
            hero.GetAbility<HeroStatusEffects>().Unstoppable(true);
        }
        hero.GetAbility<HeroAttributes>().UseAllEnergy();
        resetUnstoppableTween?.Kill();  
        resetUnstoppableTween = DOVirtual.DelayedCall(duration, () => {
            if (unstoppable) {
                hero.GetAbility<HeroStatusEffects>().Unstoppable(false);
            }
        });
    }
    
    public virtual void Cancel() {
        hero.Mecanim.InterruptSkill();
        if (unstoppable) {
            hero.GetAbility<HeroStatusEffects>().Unstoppable(false);
        }   
    }
}