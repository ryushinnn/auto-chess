using Sirenix.OdinInspector;
using UnityEngine;

public class Mecanim : MonoBehaviour {
    public enum State {
        None = 0,
        Idle = 1,
        Run = 2,
        Death = 99
    }
    
    public enum Action {
        None = 0,
        Dive = 1,
        Skill = 2
    }
    
    protected readonly int paramStateOn = Animator.StringToHash("state_on");
    protected readonly int paramState = Animator.StringToHash("state");
    protected readonly int paramLastState = Animator.StringToHash("last_state");
    protected readonly int paramHasIdleIn = Animator.StringToHash("has_idle_in");
    protected readonly int paramHasRunIn = Animator.StringToHash("has_run_in");
    protected readonly int paramActionOn = Animator.StringToHash("action_on");
    protected readonly int paramAction = Animator.StringToHash("action");
    protected readonly int paramDiveIn = Animator.StringToHash("dive_in");
    protected readonly int paramSkill= Animator.StringToHash("skill");
    protected readonly int paramHasSkill0In = Animator.StringToHash("has_skill_0_in");
    protected readonly int paramHasSkill0Out = Animator.StringToHash("has_skill_0_out");
    protected readonly int paramSkill0Multiplier = Animator.StringToHash("skill_0_multiplier");

    [SerializeField, ReadOnly] protected State currentState;
    [SerializeField, ReadOnly] protected State lastState;
    [SerializeField, ReadOnly] protected Action lastAction;
    
    protected Animator animator;
    protected BodyParts bodyParts;

    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        bodyParts = GetComponent<BodyParts>();
        SetUp();
    }

    protected virtual void Start() {
        Idle();
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
        DoAction(Action.None);
    }

    public virtual void Attack() {
        DoAction(Action.Skill, (paramSkill, 0));
    }

    public virtual float UseSkill(System.Action[] events) {
        return 0;
    }

    public virtual void ChangeState(State state) {
        if (state == currentState) return;

        lastState = currentState;
        currentState = state;
        animator.SetInteger(paramState, (int)currentState);
        animator.SetInteger(paramLastState, (int)lastState);
        animator.SetTrigger(paramStateOn);

        ModifyBodyParts();
    }

    public virtual void DoAction(Action action, params (int, object)[] metaDatas) {
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
        
        lastAction = action;
        animator.SetInteger(paramAction, (int)lastAction);
        animator.SetTrigger(paramActionOn);
    }
    
    protected virtual void SetUp() {
        
    }

    protected virtual void ModifyBodyParts() {
        
    }
}