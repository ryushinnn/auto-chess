using System;
using System.Collections;
using RExt.Utils;
using UnityEngine;

public class Mecanim_Akali : Mecanim {
    protected override IEnumerator DoUseSkill(Action[] events) {
        Interact(Interaction.Skill, (paramSkill, 0));
        yield return BetterWaitForSeconds.Wait(0.2f);
        events[0]();
    }
}