using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RExt.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillProcessor_Ashe : SkillProcessor {
    const int MIN_ARROW_SET = 10;
    const int MAX_ARROW_SET = 20;
    const float DMG_BASE = 5;
    const float DMG_MUL = 0.1f;
    const float TOTAL_TIME = 2;
    const float RADIUS = 2;
    
    public SkillProcessor_Ashe(Hero hero) : base(hero) {
        AnimationLength = 3;
        Timers = new[] { 0.75f };
        Name = "Thiên Giáng Thần Vũ Tiễn";
        Description = $"Bắn ngẫu nhiên <color=grey>{MIN_ARROW_SET} ~ {MAX_ARROW_SET}</color> loạt mưa tên. Mỗi loạt mưa tên gây" +
                      $" <color=grey>({DMG_BASE} + {DMG_MUL * 100}%<sprite name=pdmg>)</color> sát thương vật lý (có thể chí mạng)" +
                      $"cho tất cả mục tiêu trong vùng ảnh hưởng.";
    }

    public override void Process(float timer) {
        if (timer >= Timers[0] && skillExecuted == 0) {
            ShotArrows();
            skillExecuted++;
        }
    }

    async void ShotArrows() {
        var dotArea = GameObject.Instantiate(PrefabDB.Instance.DotArea);
        dotArea.transform.position = ((BattleHero)hero).Target.WorldPosition;
        var dmg = attributes.GetDamage(DamageType.Physical, false,
            scaledValues: new[] { (DMG_MUL, DamageType.Physical) }, fixedValues: new[] { DMG_BASE });
        var set = Random.Range(MIN_ARROW_SET, MAX_ARROW_SET + 1);
        var vfx = VfxPool.Instance.GetVfx("ashe_skill");
        dotArea.SetData((BattleHero)hero, dmg, set, TOTAL_TIME / set, true, RADIUS, vfx);
    }
}