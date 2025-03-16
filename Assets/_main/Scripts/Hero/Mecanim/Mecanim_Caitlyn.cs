using System.Collections;
using UnityEngine;

public class Mecanim_Caitlyn : Mecanim {
    Coroutine attackCoroutine;
    Coroutine useSkillCoroutine;

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
        return 5.633f;
    }
    
    public override void InterruptSkill() {
        DoNone();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
            bodyParts.SetBodyParts(0, ("cup",false));
        }
    }

    IEnumerator DoUseSkill(System.Action[] events) {
        bodyParts.SetBodyParts(0,("cup",true));
        DoAction(Action.Skill, (paramSkill, 1));
        yield return new WaitForSeconds(1.4f);
        events[0]();
        yield return new WaitForSeconds(3.1f);
        bodyParts.SetBodyParts(0,("cup",false));
    }
}