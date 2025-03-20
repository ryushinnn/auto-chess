using System;
using System.Collections;
using RExt.Utils;
using UnityEngine;

public class Mecanim_Malphite : Mecanim {
    public override void InterruptSkill() {
        DoNothing();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
            bodyParts.SetBodyParts(0, ("punch", false));
        }
    }

    protected override IEnumerator DoUseSkill(Action[] events) {
        Interact(Interaction.Skill, (paramSkill, 0));
        yield return BetterWaitForSeconds.Wait(0.46f);
        bodyParts.SetBodyParts(0,("punch",true));
        events[0]();
        yield return BetterWaitForSeconds.Wait(1.84f);
        events[1]();
        yield return BetterWaitForSeconds.Wait(1.84f);
        bodyParts.SetBodyParts(0,("punch",false));
    }
}