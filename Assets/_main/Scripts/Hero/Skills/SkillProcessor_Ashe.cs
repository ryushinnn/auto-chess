using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillProcessor_Ashe : SkillProcessor {
    const int MIN_ARROW_SET = 10;
    const int MAX_ARROW_SET = 20;
    const float DMG_MUL = 0.2f;
    const float PENETRATION = 0.8f;
    const int TOTAL_TIME = 2000; //ms
    
    List<Hero> affectedTargets = new();
    
    public SkillProcessor_Ashe(Hero hero) : base(hero) {
        AnimationLength = 3;
        Timers = new[] { 0.75f };
        Description = $"Bắn ngẫu nhiên {MIN_ARROW_SET}-{MAX_ARROW_SET} loạt mưa tên vào mục tiêu " +
                      "và phạm vi 1 xung quanh. Mỗi loạt mưa tên gây sát thương vật lý bằng " +
                      $"({DMG_MUL * 100}% sát thương vật lý) kèm {PENETRATION*100}% " +
                      $"xuyên giáp và có thể chí mạng.";
    }

    public override void Process(float timer) {
        if (timer >= Timers[0] && skillExecuted == 0) {
            ShotArrows();
            skillExecuted++;
        }
    }

    async void ShotArrows() {
        affectedTargets.Clear();
        var set = Random.Range(MIN_ARROW_SET, MAX_ARROW_SET + 1);
        var timePerSet = TOTAL_TIME / set;
        ShotArrow();
        for (int i=1; i<set; i++) {
            await Task.Delay(timePerSet);
            ShotArrow();
        }
    }

    void ShotArrow() {
        if (hero.Target == null) return;

        var dmg = attributes.GetDamage(DamageType.Physical, attributes.Crit(),
            scaledValues: new[] { (DMG_MUL, DamageType.Physical) });
        dmg.penetration = PENETRATION;
        var isNewTarget = !affectedTargets.Contains(hero.Target);
        
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg, isNewTarget);

        if (isNewTarget) {
            affectedTargets.Add(hero.Target);
        }
    }
}