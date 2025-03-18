using System;
using System.Collections;
using UnityEngine;

public class Mecanim_Irelia : Mecanim {
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
        yield return new WaitForSeconds(0.2f / attackTimeMultiplier);
        atkEvent.Invoke();
    }
    
    public override float UseSkill(Action[] events) {
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
        useSkillCoroutine = StartCoroutine(DoUseSkill(events));
        return 4.8f;
    }
    
    public override void InterruptSkill() {
        DoNone();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
            bodyParts.SetBodyParts(0,("dragon_0",false),("dragon_1",false));
        }
    }

    IEnumerator DoUseSkill(Action[] events) {
        Interact(Interaction.Skill, (paramSkill, 0));
        yield return new WaitForSeconds(2.35f);
        bodyParts.SetBodyParts(0,("dragon_0",true),("dragon_1",true));
        yield return new WaitForSeconds(1.88f);
        events[0]();
        yield return new WaitForSeconds(0.25f);
        bodyParts.SetBodyParts(0,("dragon_0",false),("dragon_1",false));
    }
}