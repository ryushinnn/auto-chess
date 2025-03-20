using System;
using System.Collections;
using RExt.Utils;
using UnityEngine;

public class Mecanim_Katarina : Mecanim {
    protected override IEnumerator DoUseSkill(Action[] events) {
        Interact(Interaction.Skill, (paramSkill, 0));
        events[0]();
        yield break;
    }
}