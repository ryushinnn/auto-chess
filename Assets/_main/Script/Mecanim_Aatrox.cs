using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Mecanim_Aatrox : Mecanim {
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

    [Button]
    void Test__DiveIn() {
        DoAction(Action.Dive, (paramDiveIn, true));
    }
    
    [Button]
    void Test__DiveOut() {
        DoAction(Action.Dive, (paramDiveIn, false));
    }

    [Button]
    void Test__Attack() {
        DoAction(Action.Skill, (paramSkill, 0));
    }

    [Button]
    void Test__Ultimate() {
        StartCoroutine(DoUltimate());
    }

    IEnumerator DoUltimate() {
        DoAction(Action.Skill, (paramSkill, 1));
        yield return new WaitForSeconds(1.2f);
        DoAction(Action.Skill, (paramSkill, 2));
        yield return new WaitForSeconds(1.8f);
        DoAction(Action.Skill, (paramSkill, 3));
    }
}