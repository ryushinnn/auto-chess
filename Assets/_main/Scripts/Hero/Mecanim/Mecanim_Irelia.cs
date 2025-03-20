using System;
using System.Collections;
using RExt.Utils;
using UnityEngine;

public class Mecanim_Irelia : Mecanim {
    public override void InterruptSkill() {
        DoNothing();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
            bodyParts.SetBodyParts(0,("dragon_0",false),("dragon_1",false));
        }
    }

    protected override IEnumerator DoUseSkill(Action[] events) {
        Interact(Interaction.Skill, (paramSkill, 0));
        yield return BetterWaitForSeconds.Wait(2.35f);
        bodyParts.SetBodyParts(0,("dragon_0",true),("dragon_1",true));
        yield return BetterWaitForSeconds.Wait(1.88f);
        events[0]();
        yield return BetterWaitForSeconds.Wait(0.25f);
        bodyParts.SetBodyParts(0,("dragon_0",false),("dragon_1",false));
    }
}