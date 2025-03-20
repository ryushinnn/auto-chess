using System;
using System.Collections;
using RExt.Utils;
using UnityEngine;

public class Mecanim_Jinx : Mecanim {
    public override void InterruptSkill() {
        DoNothing();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
            bodyParts.SetBodyParts(0, ("rocket",false));
        }
    }

    protected override IEnumerator DoUseSkill(Action[] events) {
        bodyParts.SetBodyParts(0, ("rocket",true));
        Interact(Interaction.Skill, (paramSkill, 0));
        yield return BetterWaitForSeconds.Wait(2f);
        events[0]();
        yield return BetterWaitForSeconds.Wait(2f);
        bodyParts.SetBodyParts(0, ("rocket",false));
    }     
}