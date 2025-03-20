using System;
using System.Collections;
using UnityEngine;

public class Mecanim_Yasuo : Mecanim {
    Coroutine attackCoroutine;
    Coroutine useSkillCoroutine;

    public override void Attack(Action atkEvent) {
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

    IEnumerator DoAttack(Action atkEvent) {
        Interact(Interaction.Attack);
        yield return new WaitForSeconds(0.33f / attackTimeMultiplier);
        atkEvent.Invoke();
    }

    public override float UseSkill(Action[] events) {
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
        useSkillCoroutine = StartCoroutine(DoUseSkill(events));
        return 6.3f;
    }

    public override void InterruptSkill() {
        DoNone();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
    }

    IEnumerator DoUseSkill(Action[] events) {
        Interact(Interaction.Skill, (paramSkill, 0));
        yield return new WaitForSeconds(0.7f);
        events[0]();
        yield return new WaitForSeconds(1f);
        events[1]();
        yield return new WaitForSeconds(1.8f);
        events[2]();
    }
}