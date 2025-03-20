using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Mecanim : MonoBehaviour {
    protected enum State {
        None = 0,
        Idle = 1,
        Run = 2,
        Death = 99
    }

    protected enum Interaction {
        None = 0,
        Dive = 1,
        Attack = 2,
        Skill = 3
    }
    
    protected readonly int paramChangeState = Animator.StringToHash("change_state");
    protected readonly int paramState = Animator.StringToHash("state");
    protected readonly int paramLastState = Animator.StringToHash("last_state");
    protected readonly int paramIdleIn = Animator.StringToHash("idle_in");
    protected readonly int paramRunIn = Animator.StringToHash("run_in");
    protected readonly int paramInteract = Animator.StringToHash("interact");
    protected readonly int paramInteraction = Animator.StringToHash("interaction");
    protected readonly int paramDiveIn = Animator.StringToHash("dive_in");
    protected readonly int paramSkill= Animator.StringToHash("skill");
    protected readonly int paramAttackIn = Animator.StringToHash("attack_in");
    protected readonly int paramAttackOut = Animator.StringToHash("attack_out");
    protected readonly int paramAttackMultiplier = Animator.StringToHash("attack_multiplier");

    [SerializeField] float defaultAttackTime;
    [SerializeField, ReadOnly] protected State currentState;
    [SerializeField, ReadOnly] protected State lastState;
    [SerializeField, ReadOnly] protected Interaction lastInteraction;
    
    protected Animator animator;
    protected BodyParts bodyParts;
    protected float attackTimeMultiplier;

    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        bodyParts = GetComponent<BodyParts>();
        attackTimeMultiplier = 1;
        SetUp();
    }

    [Button]
    public virtual void Idle() {
        ChangeState(State.Idle);
    }

    [Button]
    public virtual void Run() {
        ChangeState(State.Run);
    }

    [Button]
    public virtual void Death() {
        ChangeState(State.Death);
    }

    public virtual void DoNone() {
        Interact(Interaction.None);
    }

    public virtual void Attack(Action atkEvent) {
        Interact(Interaction.Attack);
    }
    
    public virtual void InterruptAttack() { }

    public virtual float UseSkill(Action[] events) {
        return 0;
    }

    public virtual void InterruptSkill() { }

    public void ModifyAttackTime(float atkSpd) {
        var time = 1 / atkSpd;
        attackTimeMultiplier = Mathf.Max(1, defaultAttackTime / time);
        animator.SetFloat(paramAttackMultiplier, attackTimeMultiplier);
    }

    protected void ChangeState(State state) {
        if (state == currentState) return;

        lastState = currentState;
        currentState = state;
        animator.SetInteger(paramState, (int)currentState);
        animator.SetInteger(paramLastState, (int)lastState);
        animator.SetTrigger(paramChangeState);

        ModifyBodyParts();
    }

    protected void Interact(Interaction interaction, params (int, object)[] metaDatas) {
        foreach (var data in metaDatas) {
            switch (data.Item2) {
                case int i:
                    animator.SetInteger(data.Item1, i);
                    break;
                case float f:
                    animator.SetFloat(data.Item1, f);
                    break;
                case bool b:
                    animator.SetBool(data.Item1, b);
                    break;
            }
        }
        
        lastInteraction = interaction;
        animator.SetInteger(paramInteraction, (int)lastInteraction);
        animator.SetTrigger(paramInteract);
    }
    
    protected virtual void SetUp() {
        
    }

    protected virtual void ModifyBodyParts() {
        
    }
    
    [Button]
    void dev_DiveIn() {
        Interact(Interaction.Dive, (paramDiveIn, true));
    }
    
    [Button]
    void dev_DiveOut() {
        Interact(Interaction.Dive, (paramDiveIn, false));
    }
    
    [Button]
    void dev_Attack() {
        Attack(()=>{});
    }

    [Button]
    void dev_Skill() {
        UseSkill(new Action[] { () => { } });
    }
}