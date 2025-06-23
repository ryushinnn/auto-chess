using System;
using System.Linq;
using RExt.Extension;
using UnityEngine;

[Serializable]
public class AttackProcessor {
    public float AnimationLength { get; protected set; }
    public float[] Timers { get; protected set; }
    public string Description { get; protected set; }

    protected float atkTimeMul;
    protected int atkExecuted;
    protected float trueTimer;
    
    public CustomData<int> CustomInt => customInt;
    
    protected readonly Hero hero;
    protected readonly HeroAttributes attributes;
    protected CustomData<int> customInt;
    
    public AttackProcessor(Hero hero) {
        this.hero = hero;
        attributes = this.hero.GetAbility<HeroAttributes>();
    }

    public virtual void Begin(out float actualAnimLength) {
        var expectedAnimLength = 1 / attributes.AttackSpeed;
        atkTimeMul = Mathf.Max(1, AnimationLength / expectedAnimLength);
        actualAnimLength = AnimationLength / atkTimeMul;
        hero.Mecanim.ModifyAttackTime_New(atkTimeMul);
        atkExecuted = 0;
    }

    public virtual void Process(float timer) {
        trueTimer = timer * atkTimeMul;
    }

    public virtual void End(bool complete) { }
}