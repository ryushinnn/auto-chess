using System;
using System.Collections;
using RExt.Utils;
using UnityEngine;

public class Mecanim_Yasuo : Mecanim {
    protected override IEnumerator DoUseSkill(Action[] events) {
        Interact(Interaction.Skill, (paramSkill, 0));
        yield return BetterWaitForSeconds.Wait(0.7f);
        events[0]();
        yield return BetterWaitForSeconds.Wait(1f);
        events[1]();
        yield return BetterWaitForSeconds.Wait(1.8f);
        events[2]();
    }
}