using System;
using System.Collections;
using RExt.Utils;
using UnityEngine;

public class Mecanim_MissFortune : Mecanim {
    protected override IEnumerator DoUseSkill(Action[] events) {
        Interact(Interaction.Skill, (paramSkill, 0));
        yield return BetterWaitForSeconds.Wait(1f);
        events[0]();
        yield return BetterWaitForSeconds.Wait(0.3f);
        events[1]();
    }
}