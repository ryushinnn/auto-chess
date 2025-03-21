using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// thay doi qua lai giua than kiem va quy kiem sau moi don danh
/// don danh than kiem +1 stack suy yeu: giam 5% st cua muc tieu trong 5s, toi da 3 stack
/// don danh quy kiem cho toi da 20% hut mau, dua vao mau da mat
/// thanh kiem cuoi cung duoc su dung cung quyet dinh skill la gi
/// </summary>
public class AttackProcessor_Yone : AttackProcessor {
    const string EFFECT_KEY = "yone_weakening";
    const float DIVINE_WEAKENING_PER_STACK = -0.05f;
    const int DIVINE_WEAKENING_MAX_STACK = 3;
    const int DIVINE_WEAKENING_DURATION = 5;
    const float DEVIL_VAMP_MIN = 0f;
    const float DEVIL_VAMP_MAX = 0.2f;

    public AttackProcessor_Yone(Hero hero) : base(hero) {
        customInt = new CustomData<int>();
        customInt["sword"] = (int)YoneSword.Divine;
    }

    public override void Execute() {
        CalculateDamage(out var damage);
        var sword = (YoneSword)customInt["sword"];
        hero.Mecanim.Attack(new Action[]{
            () => {
                if (hero.Target == null) return;
                
                var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(damage);
                var heal = outputDamage * attributes.LifeSteal;
                if (sword == YoneSword.Devil) {
                    heal += outputDamage * Mathf.Lerp(DEVIL_VAMP_MIN, DEVIL_VAMP_MAX, attributes.HpLostPercentage);
                }

                if (heal > 0) {
                    attributes.Heal(heal);
                }

                if (sword == YoneSword.Divine) {
                    var currentStack = hero.Target.GetAbility<HeroMark>().GetMark(EFFECT_KEY, hero)?.stacks ?? 0;
                    var nextStacks = Mathf.Min(currentStack + 1, DIVINE_WEAKENING_MAX_STACK);
                    
                    hero.Target.GetAbility<HeroAttributes>().AddAttributeModifier(
                        AttributeModifierSet.Create(
                            hero,
                            EFFECT_KEY,
                            DIVINE_WEAKENING_DURATION,
                            new[] {
                                (AttributeModifier.Key.PhysicalDamage, DIVINE_WEAKENING_PER_STACK, AttributeModifier.Type.Percentage),
                                (AttributeModifier.Key.MagicalDamage, DIVINE_WEAKENING_PER_STACK, AttributeModifier.Type.Percentage),
                            },
                            nextStacks
                        ));
                }
            }
        });
        customInt["sword"] = (customInt["sword"] + 1) % 2;
    }
}

public enum YoneSword {
    Divine,
    Devil
}