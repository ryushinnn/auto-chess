using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Mecanim_Yasuo : Mecanim {
    Coroutine useSkillCoroutine;
    
    public override void DoNone() {
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
        base.DoNone();
    }
    
    public override float UseSkill(System.Action[] events) {
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
        useSkillCoroutine = StartCoroutine(DoUseSkill(events));
        return 4.6f + 0.9f;
    }

    IEnumerator DoUseSkill(System.Action[] events) {
        bodyParts.SetBodyParts(0,("dragon",true));
        DoAction(Action.Skill, (paramSkill, 1));
        yield return new WaitForSeconds(4.6f);
        DoAction(Action.Skill, (paramSkill, 2));
        yield return new WaitForSeconds(0.9f);
        bodyParts.SetBodyParts(0, ("dragon",false));
    }
}