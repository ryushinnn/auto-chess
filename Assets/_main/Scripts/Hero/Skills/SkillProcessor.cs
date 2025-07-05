using System;
using DG.Tweening;

public abstract class SkillProcessor {
    public float AnimationLength { get; protected set; }
    public float[] Timers { get; protected set; }
    public bool Unstoppable { get; protected set; }
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    
    protected readonly Hero hero;
    protected readonly HeroAttributes attributes;
    protected readonly HeroMark mark;
    protected Action[] events;
    
    protected Tween resetUnstoppableTween;
    
    protected int skillExecuted;

    protected SkillProcessor(Hero hero) {
        this.hero = hero;
        attributes = this.hero.GetAbility<HeroAttributes>();
        mark = this.hero.GetAbility<HeroMark>();
    }
    
    public virtual void Process() {
        
    }

    public virtual void Begin(out float animLength) {
        if (Unstoppable) {
            hero.GetAbility<HeroStatusEffects>().Unstoppable(true);
        }
        hero.GetAbility<HeroAttributes>().UseAllEnergy();
        animLength = AnimationLength;
        skillExecuted = 0;
    }

    public virtual void Process(float timer) {
        
    }

    public virtual void End(bool complete) {
        if (Unstoppable) {
            hero.GetAbility<HeroStatusEffects>().Unstoppable(false);
        }
    }
    
    public virtual void Execute(out float duration) {
        duration = hero.Mecanim.UseSkill(events);
        if (Unstoppable) {
            hero.GetAbility<HeroStatusEffects>().Unstoppable(true);
        }
        hero.GetAbility<HeroAttributes>().UseAllEnergy();
        resetUnstoppableTween?.Kill();  
        resetUnstoppableTween = DOVirtual.DelayedCall(duration, () => {
            if (Unstoppable) {
                hero.GetAbility<HeroStatusEffects>().Unstoppable(false);
            }
        });
    }
    
    public virtual void Cancel() {
        hero.Mecanim.InterruptSkill();
        if (Unstoppable) {
            hero.GetAbility<HeroStatusEffects>().Unstoppable(false);
        }   
    }
}