using System;
using UnityEngine;

/// <summary>
/// chem xoay pham vi 1 xung quanh, gay 150% st vat ly
/// neu chem trung it nhat 1 muc tieu, tang 20% toc danh, duy tri 5s (ko stack)
/// </summary>
public class Skill_Yasuo : Skill {
    Hero hero;
    string lastAttackSpeedModifierId;

    const float DMG_MUL = 1.5f;
    const int RANGE = 1;
    const float ATK_SPD_MUL = 0.2f;
    const float ATK_SPD_DURATION = 10f;
    
    public Skill_Yasuo(Hero hero) {
        this.hero = hero;

        events = new Action[]{SpinSlash};
        unstoppable = false;
    }

    void SpinSlash() {
        if (hero.Target == null) return;
        
        var success = false;
        var affectedNodes = Map.Instance.GetCircle(hero.MapNode.X, hero.MapNode.Y, RANGE);
        foreach (var node in affectedNodes) {
            if (!node.HasNone()) {
                node.Process(x => {
                    if (x is Hero h) {
                        h.GetAbility<HeroAttributes>().TakeDamage(
                            hero.GetAbility<HeroAttributes>().PhysicalDamage * DMG_MUL, 
                            DamageType.Physical, 
                            hero.GetAbility<HeroAttributes>().PhysicalPenetration);
                        success = true;
                    }
                });
            }
        }

        if (success) {
            if (lastAttackSpeedModifierId != null) {
                hero.GetAbility<HeroAttributes>().RemoveAttributeModifier(lastAttackSpeedModifierId);
            }

            var modifier = AttributeModifier.Create(AttributeModifierKey.AttackSpeed, ATK_SPD_MUL, ModifierType.Percentage, ATK_SPD_DURATION);
            hero.GetAbility<HeroAttributes>().AddAttributeModifier(modifier);
            lastAttackSpeedModifierId = modifier.id;
        }
    }
}