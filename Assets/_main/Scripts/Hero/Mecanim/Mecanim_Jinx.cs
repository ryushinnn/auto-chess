using System;
using System.Collections;
using UnityEngine;

public class Mecanim_Jinx : Mecanim {
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
        yield return new WaitForSeconds(0.37f / attackTimeMultiplier);
        atkEvent.Invoke();
    }
    
    public override float UseSkill(Action[] events) {
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
        useSkillCoroutine = StartCoroutine(DoUseSkill(events));
        return 4.2f;
    }
    
    public override void InterruptSkill() {
        DoNone();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
            bodyParts.SetBodyParts(0, ("rocket",false));
        }
    }

    IEnumerator DoUseSkill(Action[] events) {
        bodyParts.SetBodyParts(0, ("rocket",true));
        Interact(Interaction.Skill, (paramSkill, 0));
        yield return new WaitForSeconds(2f);
        events[0]();
        yield return new WaitForSeconds(2f);
        bodyParts.SetBodyParts(0, ("rocket",false));
    }     
}