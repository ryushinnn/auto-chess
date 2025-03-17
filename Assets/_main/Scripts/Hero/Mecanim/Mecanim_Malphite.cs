using System.Collections;
using UnityEngine;

public class Mecanim_Malphite : Mecanim {
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
        yield return new WaitForSeconds(0.17f);
        atkEvent.Invoke();
    }
    
    public override float UseSkill(System.Action[] events) {
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
        useSkillCoroutine = StartCoroutine(DoUseSkill(events));
        return 4.7f;
    }
    
    public override void InterruptSkill() {
        DoNone();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
            bodyParts.SetBodyParts(0,("punch",false));
        }
    }

    IEnumerator DoUseSkill(System.Action[] events) {
        DoAction(Action.Skill, (paramSkill, 1));
        yield return new WaitForSeconds(0.46f);
        bodyParts.SetBodyParts(0,("punch",true));
        events[0]();
        yield return new WaitForSeconds(1.84f);
        events[1]();
        yield return new WaitForSeconds(1.84f);
        bodyParts.SetBodyParts(0,("punch",false));
    }
}