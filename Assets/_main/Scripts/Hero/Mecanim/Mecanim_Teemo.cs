using System;
using System.Collections;
using RExt.Utils;
using UnityEngine;

public class Mecanim_Teemo : Mecanim {
    public override void InterruptSkill() {
        DoNothing();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
            bodyParts.SetBodyParts(0, ("bomb_0",false),("bomb_1",false),("bomb_2",false));
        }
    }

    protected override IEnumerator DoUseSkill(Action[] events) {
        Interact(Interaction.Skill, (paramSkill, 0));
        bodyParts.SetBodyParts(0, ("bomb_0",true),("bomb_1",true),("bomb_2",true));
        yield return BetterWaitForSeconds.Wait(1.56f);
        bodyParts.SetBodyParts(0,("bomb_2",false));
        events[0]();
        yield return BetterWaitForSeconds.Wait(1.17f);
        bodyParts.SetBodyParts(0,("bomb_0",false));
        events[0]();
        yield return BetterWaitForSeconds.Wait(0.56f);
        events[1]();
        bodyParts.SetBodyParts(0, ("bomb_1",false));
    }
}