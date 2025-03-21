using System;
using System.Collections.Generic;

/// <summary>
/// tang 200% giap va khoang phep, toi da khang hieu ung,
/// nhung bi giam 50% toc do danh, duy tri 5s
/// nhay len roi giam xuong gay st = 100% st vay ly va hat tung ke dich 2s
/// </summary>
public class SkillProcessor_Malphite : SkillProcessor {
    const string EFFECT_KEY = "malphite_unstoppable";
    const float DEFENSE_MUL = 2;
    const float ATK_SPEED_REDUCE_MUL = -0.5f;
    const float EFFECT_DURATION = 5;
    const float DMG_MUL = 1f;
    const float AIRBORNE_DURATION = 2f;
    
    public SkillProcessor_Malphite(Hero hero) : base(hero) {
        events = new Action[] { Strengthen, Slam };
    }

    void Strengthen() {
        if (!attributes.IsAlive) return;
        
        attributes.AddAttributeModifier(
            AttributeModifierSet.Create(
                hero,
                EFFECT_KEY,
                EFFECT_DURATION,
                new [] {
                    (AttributeModifier.Key.Armor, DEFENSE_MUL, AttributeModifier.Type.Percentage),
                    (AttributeModifier.Key.Resistance, DEFENSE_MUL, AttributeModifier.Type.Percentage),
                    (AttributeModifier.Key.AttackSpeed, ATK_SPEED_REDUCE_MUL, AttributeModifier.Type.Percentage),
                    (AttributeModifier.Key.Tenacity, HeroTrait.MAX_TENACITY, AttributeModifier.Type.FixedValue),
                }
            ));
    }

    void Slam() {
        if (hero.Target == null) return;
        
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                attributes.PhysicalDamage * DMG_MUL,
                DamageType.Physical,
                attributes.PhysicalPenetration
            ));
        
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_DURATION);
    }
}