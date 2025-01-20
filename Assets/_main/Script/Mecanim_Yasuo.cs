using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Mecanim_Yasuo : Mecanim {
    public override float UseSkill() {
        StartCoroutine(DoUseSkill());
        return 4.6f + 0.9f;
    }

    IEnumerator DoUseSkill() {
        bodyParts.SetBodyParts(0,("dragon",true));
        DoAction(Action.Skill, (paramSkill, 1));
        yield return new WaitForSeconds(4.6f);
        DoAction(Action.Skill, (paramSkill, 2));
        yield return new WaitForSeconds(0.9f);
        bodyParts.SetBodyParts(0, ("dragon",false));
    }
}