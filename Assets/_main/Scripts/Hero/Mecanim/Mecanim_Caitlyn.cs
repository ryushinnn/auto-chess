using System;
using System.Collections;
using RExt.Utils;
using UnityEngine;

public class Mecanim_Caitlyn : Mecanim {
    protected override IEnumerator DoUseSkill(Action[] events) {
        Interact(Interaction.Skill, (paramSkill, 0));
        yield return BetterWaitForSeconds.Wait(1f);
        bodyParts.SetBodyParts(0,("cake",true));
        yield return BetterWaitForSeconds.Wait(1.2f);
        events[0]();
        yield return BetterWaitForSeconds.Wait(1.9f);
        bodyParts.SetBodyParts(0,("cake",false));
        yield return BetterWaitForSeconds.Wait(1.5f);
        events[1]();
    }
}