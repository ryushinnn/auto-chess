using System;
using UnityEngine;

/// <summary>
/// hoi mau = 10% - 30% maxhp ban than + 200% st phep dua vao mau da mat
/// </summary>
public class SkillProcessor_Morgana : SkillProcessor {
    const float MAX_HP_TO_HEAL_MIN = 0.1f;
    const float MAX_HP_TO_HEAL_MAX = 0.3f;
    const float DMG_TO_HEAL_MUL = 2f;

    public SkillProcessor_Morgana(Hero hero) : base(hero) {
        events = new Action[] { OpenWings };
    }

    void OpenWings() {
        if (!attributes.IsAlive) return;
        
        attributes.Heal(attributes.MagicalDamage * DMG_TO_HEAL_MUL 
                        + Mathf.Lerp(attributes.MaxHp * MAX_HP_TO_HEAL_MIN, attributes.MaxHp * MAX_HP_TO_HEAL_MAX, 1 - attributes.Hp / attributes.MaxHp));
    }
}