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
    
    protected int skillExecuted;

    protected SkillProcessor(Hero hero) {
        this.hero = hero;
        attributes = this.hero.GetAbility<HeroAttributes>();
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
}