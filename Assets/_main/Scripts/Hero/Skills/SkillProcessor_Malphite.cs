using System;
using System.Collections.Generic;

/// <summary>
/// tang 200% giap va khoang phep, toi da khang hieu ung,
/// nhung bi giam 50% toc do danh, duy tri 5s
/// nhay len roi giam xuong gay st = 100% st vay ly va hat tung ke dich 2s
/// </summary>
public class SkillProcessor_Malphite : SkillProcessor {
    List<string> lastModifierIds = new();
    
    const float DEFENSE_MUL = 2;
    const float ATK_SPEED_REDUCE_MUL = -0.5f;
    const float EFFECT_DURATION = 5;
    const float DMG_MUL = 1f;
    const float AIRBORNE_DURATION = 2f;
    
    public SkillProcessor_Malphite(Hero hero) : base(hero) {
        events = new Action[] { Strengthen, Slam };
    }

    void Strengthen() {
        foreach (var id in lastModifierIds) {
            attributes.RemoveAttributeModifier(id);
        }
        lastModifierIds.Clear();

        var armorModifier = AttributeModifier.Create(AttributeModifierKey.Armor, DEFENSE_MUL, ModifierType.Percentage, EFFECT_DURATION);
        attributes.AddAttributeModifier(armorModifier);
        lastModifierIds.Add(armorModifier.id);

        var resistanceModifier = AttributeModifier.Create(AttributeModifierKey.Resistance, DEFENSE_MUL, ModifierType.Percentage, EFFECT_DURATION);
        attributes.AddAttributeModifier(resistanceModifier);
        lastModifierIds.Add(resistanceModifier.id);

        var atkSpeedModifier = AttributeModifier.Create(AttributeModifierKey.AttackSpeed, ATK_SPEED_REDUCE_MUL, ModifierType.Percentage, EFFECT_DURATION);
        attributes.AddAttributeModifier(atkSpeedModifier);
        lastModifierIds.Add(atkSpeedModifier.id);
        
        var tenacityModifier = AttributeModifier.Create(AttributeModifierKey.Tenacity, HeroTrait.MAX_TENACITY, ModifierType.FixedValue, EFFECT_DURATION);
        attributes.AddAttributeModifier(tenacityModifier);
        lastModifierIds.Add(tenacityModifier.id);
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