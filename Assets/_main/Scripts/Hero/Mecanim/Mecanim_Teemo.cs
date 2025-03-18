using System;
using System.Collections;
using UnityEngine;

public class Mecanim_Teemo : Mecanim {
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
        yield return new WaitForSeconds(0.17f / attackTimeMultiplier);
        atkEvent.Invoke();
    }
    
    public override float UseSkill(Action[] events) {
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
        useSkillCoroutine = StartCoroutine(DoUseSkill(events));
        return 4f;
    }
    
    public override void InterruptSkill() {
        DoNone();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
            bodyParts.SetBodyParts(0, ("bomb_0",false),("bomb_1",false),("bomb_2",false));
        }
    }

    IEnumerator DoUseSkill(Action[] events) {
        Interact(Interaction.Skill, (paramSkill, 0));
        bodyParts.SetBodyParts(0, ("bomb_0",true),("bomb_1",true),("bomb_2",true));
        yield return new WaitForSeconds(1.56f);
        bodyParts.SetBodyParts(0,("bomb_2",false));
        events[0]();
        yield return new WaitForSeconds(1.17f);
        bodyParts.SetBodyParts(0,("bomb_0",false));
        events[0]();
        yield return new WaitForSeconds(0.56f);
        events[1]();
        bodyParts.SetBodyParts(0, ("bomb_1",false));
    }
}