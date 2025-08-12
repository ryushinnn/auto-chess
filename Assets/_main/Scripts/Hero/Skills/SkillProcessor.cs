using System;
using DG.Tweening;

public class SkillProcessor {
    public float AnimationLength { get; protected set; }
    public float[] Timers { get; protected set; }
    public bool Unstoppable { get; protected set; }
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    
    protected readonly Hero hero;
    protected readonly HeroAttributes attributes;

    protected bool drainEnergy;
    protected float drainEnergyDelay;
    protected float drainEnergyDuration;
    
    protected int skillExecuted;

    public SkillProcessor(Hero hero) {
        this.hero = hero;
        attributes = this.hero.GetAbility<HeroAttributes>();
    }

    public virtual void Begin(out float animLength) {
        if (Unstoppable) {
            hero.GetAbility<HeroStatusEffects>().Unstoppable(true);
        }

        if (drainEnergy) {
            attributes.DrainEnergy(drainEnergyDelay, drainEnergyDuration);
        }
        else {
            attributes.UseAllEnergy();
        }
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