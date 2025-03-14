using System.Collections;
using UnityEngine;

public class Mecanim_Zed : Mecanim {
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
        yield return new WaitForSeconds(0.25f);
        atkEvent.Invoke();
    }
    
    public override float UseSkill(System.Action[] events) {
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
        useSkillCoroutine = StartCoroutine(DoUseSkill(events));
        return 3f;
    }
    
    public override void InterruptSkill() {
        DoNone();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
    }

    IEnumerator DoUseSkill(System.Action[] events) {
        DoAction(Action.Skill, (paramSkill, 1));
        yield return new WaitForSeconds(0.83f);
        events[0]();
        yield return new WaitForSeconds(0.83f);
        events[1]();
    }
}