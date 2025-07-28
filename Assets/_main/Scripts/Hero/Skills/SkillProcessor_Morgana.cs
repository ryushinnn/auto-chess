using System;
using UnityEngine;

public class SkillProcessor_Morgana : SkillProcessor {
    const float MAX_HP_TO_HEAL_MIN = 0.1f;
    const float MAX_HP_TO_HEAL_MAX = 0.3f;
    const float DMG_TO_HEAL_MUL = 2f;

    public SkillProcessor_Morgana(Hero hero) : base(hero) {
        AnimationLength = 1;
        Timers = new[] { 0.41f };
        Name = "Thần Quang Phổ Chiếu";
        Description = $"Hồi máu bằng ({MAX_HP_TO_HEAL_MIN * 100}%~{MAX_HP_TO_HEAL_MAX * 100}% " +
                      $"<sprite name=hp> tối đa + {DMG_TO_HEAL_MUL * 100}% <sprite name=mdmg>) " +
                      $"dựa trên máu đã mất.";
    }

    public override void Process(float timer) {
        if (timer >= Timers[0] && skillExecuted == 0) {
            OpenWings();
            skillExecuted++;
        }
    }

    void OpenWings() {
        if (!attributes.IsAlive) return;
        
        attributes.Heal(attributes.MagicalDamage * DMG_TO_HEAL_MUL 
                        + attributes.MaxHp * Mathf.Lerp(MAX_HP_TO_HEAL_MIN, MAX_HP_TO_HEAL_MAX, attributes.HpLostPercentage));
    }
}