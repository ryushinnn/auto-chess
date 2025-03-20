using System;
using System.Collections;
using RExt.Utils;
using UnityEngine;

public class Mecanim_Yone : Mecanim {
    Hero hero;
    YoneSword sword;
    
    protected override void SetUp() {
        hero = GetComponentInParent<Hero>();
        animator.SetBool(paramAttackIn, true);
        animator.SetBool(paramAttackOut, true);
    }
    
    public override float UseSkill(Action[] events) {
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }

        if (hero.GetAbility<HeroAttack>().Processor is not AttackProcessor_Yone atkProcessor) {
            Debug.LogError("Yone: AttackProcessor is missing");
            return 0;
        }
        
        if (atkProcessor.CustomInt["sword"] == (int)YoneSword.Divine) {
            sword = YoneSword.Divine;
            useSkillCoroutine = StartCoroutine(DoUseSkill(events));
            return skillFullTimes[0];
        }
        
        if (atkProcessor.CustomInt["sword"] == (int)YoneSword.Devil) {
            sword = YoneSword.Devil;
            useSkillCoroutine = StartCoroutine(DoUseSkill(events));
            return skillFullTimes[1];
        }

        Debug.LogError("Yone: No sword found");
        return 0;
    }

    protected override IEnumerator DoUseSkill(Action[] events) {
        if (sword == YoneSword.Divine) {
            Interact(Interaction.Skill, (paramSkill, 0));
            yield return BetterWaitForSeconds.Wait(1.66f);
            events[0]();
        }
        else if (sword == YoneSword.Devil) {
            Interact(Interaction.Skill, (paramSkill, 1));
            yield return BetterWaitForSeconds.Wait(0.12f);
            events[1]();
            yield return BetterWaitForSeconds.Wait(0.6f);
            events[1]();
            yield return BetterWaitForSeconds.Wait(0.36f);
            events[1]();
            yield return BetterWaitForSeconds.Wait(1.16f);
            events[2]();
        }
    }

    protected override void ModifyBodyParts() {
        switch (currentState) {
            case State.Death:
                bodyParts.SetBodyParts(0,
                    ("mouth", false),
                    ("r_hand",false),
                    ("eyes",false),
                    ("decor",false),
                    ("body",false),
                    ("horns",false),
                    ("hair",false));
                break;
        }
    }
}