using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Mecanim_Yasuo : Mecanim {
    [Button]
    void Test__Attack() {
        DoAction(Action.Skill, (paramSkill, 0));
    }

    [Button]
    void Test__Ultimate() {
        StartCoroutine(DoUltimate());
    }

    IEnumerator DoUltimate() {
        bodyParts.SetBodyParts(0,("dragon",true));
        DoAction(Action.Skill, (paramSkill, 1));
        yield return new WaitForSeconds(4.6f);
        DoAction(Action.Skill, (paramSkill, 2));
        yield return new WaitForSeconds(0.9f);
        bodyParts.SetBodyParts(0, ("dragon",false));
    }
}