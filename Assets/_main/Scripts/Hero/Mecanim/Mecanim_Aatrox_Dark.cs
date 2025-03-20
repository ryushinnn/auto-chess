using System;
using System.Collections;
using RExt.Utils;
using UnityEngine;

public class Mecanim_Aatrox_Dark : Mecanim {
    protected override void ModifyBodyParts() {
        switch (currentState) {
            case State.Idle:
                bodyParts.SetBodyParts(0, ("spikes", false), ("wings_0", true), ("wings_1", true));
                break;
            
            case State.Run:
                bodyParts.SetBodyParts(0, ("spikes", false), ("wings_0", true), ("wings_1", true));
                break;
            
            case State.Death:
                bodyParts.SetBodyParts(0, ("spikes", true), ("wings_0", false), ("wings_1", false));
                break;
        }
    }

    protected override IEnumerator DoAttack(Action[] events) {
        Interact(Interaction.Attack);
        for (int i = 0; i < 2; i++) {
            yield return BetterWaitForSeconds.Wait(defaultAttackTime[i] / attackTimeMultiplier);
            events[i]();
        }
    }

    protected override IEnumerator DoUseSkill(Action[] events) {
        Interact(Interaction.Skill, (paramSkill, 0));
        yield return BetterWaitForSeconds.Wait(0.6f);
        events[0]();
        yield return BetterWaitForSeconds.Wait(0.6f);
        Interact(Interaction.Skill, (paramSkill, 1));
        yield return BetterWaitForSeconds.Wait(1.1f);
        events[1]();
        yield return BetterWaitForSeconds.Wait(0.8f);
        Interact(Interaction.Skill, (paramSkill, 2));
        yield return BetterWaitForSeconds.Wait(1f);
        events[2]();
    }
}