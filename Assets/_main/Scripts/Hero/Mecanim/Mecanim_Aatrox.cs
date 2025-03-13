using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Mecanim_Aatrox : Mecanim {
    Coroutine attackCoroutine;
    Coroutine useSkillCoroutine;
    
    protected override void ModifyBodyParts() {
        switch (currentState) {
            case State.Idle:
                bodyParts.SetBodyParts(0, ("spikes", false), ("wings_0", true), ("wings_1", true));
                break;
            
            case State.Run:
                bodyParts.SetBodyParts(0, ("spikes", false), ("wings_0", true), ("wings_1", true));
                break;
            
            case State.Death:
                bodyParts.SetBodyParts(0, ("spikes", true), ("wings_0", false), ("wings_1", false));
                break;
        }
    }

    [Button]
    void Test__DiveIn() {
        DoAction(Action.Dive, (paramDiveIn, true));
    }
    
    [Button]
    void Test__DiveOut() {
        DoAction(Action.Dive, (paramDiveIn, false));
    }

    public override void Attack(System.Action atkEvent) {
        if (attackCoroutine != null) {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(DoAttack(atkEvent));
    }
    
    public override void InterruptAttack() {
        DoNone();
        if (attackCoroutine != null) {
            StopCoroutine(attackCoroutine);
        }
    }

    IEnumerator DoAttack(System.Action atkEvent) {
        DoAction(Action.Skill, (paramSkill, 0));
        yield return new WaitForSeconds(0.2f);
        atkEvent.Invoke();
    }

    public override float UseSkill(System.Action[] events) {
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
        useSkillCoroutine = StartCoroutine(DoUseSkill(events));
        return 5f;
    }

    public override void InterruptSkill() {
        DoNone();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
    }

    IEnumerator DoUseSkill(System.Action[] events) {
        DoAction(Action.Skill, (paramSkill, 1));
        yield return new WaitForSeconds(0.6f);
        events[0]();
        yield return new WaitForSeconds(0.6f);
        DoAction(Action.Skill, (paramSkill, 2));
        yield return new WaitForSeconds(1.1f);
        events[1]();
        yield return new WaitForSeconds(0.8f);
        DoAction(Action.Skill, (paramSkill, 3));
        yield return new WaitForSeconds(1f);
        events[2]();
    }
}