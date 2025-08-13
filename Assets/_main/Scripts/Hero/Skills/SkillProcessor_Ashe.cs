using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RExt.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillProcessor_Ashe : SkillProcessor {
    readonly int arrowSetMin;
    readonly int arrowSetMax;
    readonly float dmgBase;
    readonly float pdmgMul;
    readonly float mdmgMul;
    readonly float totalTime;
    readonly float radius;
    
    public SkillProcessor_Ashe(BattleHero hero) : base(hero) {
        animationLength = 3;
        timers = new[] { 0.75f };

        var skillParams = hero.Trait.skillParams;
        arrowSetMin = (int)skillParams[0].value;
        arrowSetMax = (int)skillParams[1].value;
        dmgBase = skillParams[2].value;
        pdmgMul = skillParams[3].value;
        mdmgMul = skillParams[4].value;
        totalTime = skillParams[5].value;
        radius = skillParams[6].value;
    }

    public override void Process(float timer) {
        if (timer >= timers[0] && skillExecuted == 0) {
            ShotArrows();
            skillExecuted++;
        }
    }

    void ShotArrows() {
        var dotArea = GameObject.Instantiate(PrefabDB.Instance.DotArea);
        dotArea.transform.position = ((BattleHero)hero).Target.WorldPosition;
        var dmg = attributes.GetDamage(DamageType.Physical, false,
            scaledValues: new[] {
                (pdmgMul, DamageType.Physical),
                (mdmgMul, DamageType.Magical)
            }, fixedValues: new[] { dmgBase });
        var set = Random.Range(arrowSetMin, arrowSetMax + 1);
        var vfx = VfxPool.Instance.GetVfx("ashe_skill");
        dotArea.SetData((BattleHero)hero, dmg, set, totalTime / set, true, radius, vfx);
    }
}