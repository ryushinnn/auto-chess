using System.Collections;
using UnityEngine;

public class Mecanim_Yone : Mecanim {
    Coroutine attackCoroutine;
    Coroutine useSkillCoroutine;
    Hero hero;
    
    protected override void SetUp() {
        hero = GetComponentInParent<Hero>();
        animator.SetBool(paramHasSkill0In, true);
        animator.SetBool(paramHasSkill0Out, true);
    }

    public override void Attack(System.Action atkEvent) {
        if (attackCoroutine != null) {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(DoAttack(atkEvent));
    }

    public override void InterruptAttack() {
        DoNone();
        if (attackCoroutine != null) {
            StopCoroutine(attackCoroutine);
        }
    }

    IEnumerator DoAttack(System.Action atkEvent) {
        if (hero.GetAbility<HeroAttack>().Processor is not AttackProcessor_Yone atkProcessor) yield break;
        
        if (atkProcessor.CustomInt["sword"] == (int)YoneSword.Divine) {
            DoAction(Action.Skill, (paramSkill, 0));
            yield return new WaitForSeconds(0.26f);
            atkEvent.Invoke();
        }
        else if (atkProcessor.CustomInt["sword"] == (int)YoneSword.Devil) {
            DoAction(Action.Skill, (paramSkill, 0));
            yield return new WaitForSeconds(0.26f);
            atkEvent.Invoke();
        }
    }
    
    public override float UseSkill(System.Action[] events) {
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }

        if (hero.GetAbility<HeroAttack>().Processor is not AttackProcessor_Yone atkProcessor) return 0;
        
        if (atkProcessor.CustomInt["sword"] == (int)YoneSword.Divine) {
            useSkillCoroutine = StartCoroutine(DoUseSkill(YoneSword.Divine, events));
            return 4f;
        }
        
        if (atkProcessor.CustomInt["sword"] == (int)YoneSword.Devil) {
            useSkillCoroutine = StartCoroutine(DoUseSkill(YoneSword.Devil, events));
            return 4.25f;
        }

        return 0;
    }
    
    public override void InterruptSkill() {
        DoNone();
        if (useSkillCoroutine != null) {
            StopCoroutine(useSkillCoroutine);
        }
    }

    IEnumerator DoUseSkill(YoneSword sword, System.Action[] events) {
        if (sword == YoneSword.Divine) {
            DoAction(Action.Skill, (paramSkill, 2));
            yield return new WaitForSeconds(1.66f);
            events[0]();
        }
        else if (sword == YoneSword.Devil) {
            DoAction(Action.Skill, (paramSkill, 3));
            yield return new WaitForSeconds(0.12f);
            events[1]();
            yield return new WaitForSeconds(0.6f);
            events[1]();
            yield return new WaitForSeconds(0.36f);
            events[1]();
            yield return new WaitForSeconds(1.16f);
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