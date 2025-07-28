using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackProcessor_Yone : AttackProcessor {
    const string EFFECT_KEY = "yone_weakening";
    const float DIVINE_WEAKENING_PER_STACK = -0.05f;
    const int DIVINE_WEAKENING_MAX_STACK = 3;
    const int DIVINE_WEAKENING_DURATION = 5;
    const float DEVIL_VAMP_MIN = 0f;
    const float DEVIL_VAMP_MAX = 0.2f;

    public AttackProcessor_Yone(Hero hero) : base(hero) {
        AnimationLength = 1.367f;
        Timers = new[] { 0.5f };
        Description = "Thay đổi qua lại giữa <color=yellow>Thần Kiếm</color> và <color=purple>Quỷ Kiếm</color> sau mỗi đòn đánh.\n" +
                      $"- <color=yellow>Thần Kiếm</color>: Gây (100% <sprite name=pdmg>) sát thương vật lý. " +
                      $"Thêm 1 cộng dồn (tối đa {DIVINE_WEAKENING_MAX_STACK} cộng dồn) " +
                      $"và làm mới thời gian duy trì ({DIVINE_WEAKENING_DURATION}s) hiệu ứng <color=orange>SUY YẾU</color> " +
                      $"trên mục tiêu\n" +
                      $"<color=orange>SUY YẾU</color>: Giảm {DIVINE_WEAKENING_PER_STACK * -100}% <sprite name=pdmg> " +
                      $"và <sprite name=mdmg> của mục tiêu.\n" +
                      $"- <color=purple>Quỷ Kiếm</color>: Gây (100% <sprite name=pdmg>) sát thương phép. " +
                      $"Nhận thêm tối đa {DEVIL_VAMP_MAX * 100}% <sprite name=ls>, dựa trên " +
                      $"máu đã mất";
        
        customInt = new CustomData<int>();
        customInt["sword"] = (int)YoneSword.Divine;
    }

    public override void Process(float timer) {
        base.Process(timer);

        if (trueTimer >= Timers[0] && atkExecuted == 0) {
            if (hero.Target != null) {
                var sword = (YoneSword)customInt["sword"];
                Damage dmg = default;
                if (sword == YoneSword.Divine) {
                    dmg = attributes.GetDamage(DamageType.Physical);
                }
                else {
                    dmg = attributes.GetDamage(DamageType.Magical, attributes.Crit(),
                        scaledValues: new[] { (1f, DamageType.Physical) });
                }
                
                var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg);
                var heal = outputDmg * attributes.LifeSteal;
                if (sword == YoneSword.Devil) {
                    heal += outputDmg * Mathf.Lerp(DEVIL_VAMP_MIN, DEVIL_VAMP_MAX, attributes.HpLostPercentage);
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
                                (AttributeModifierKey.PhysicalDamage, DIVINE_WEAKENING_PER_STACK, AttributeModifier.Type.Percentage),
                                (AttributeModifierKey.MagicalDamage, DIVINE_WEAKENING_PER_STACK, AttributeModifier.Type.Percentage),
                            },
                            nextStacks
                        ));
                }
                
                attributes.RegenEnergy(hero.Trait.energyRegenPerAttack);
            }
            customInt["sword"] = (customInt["sword"] + 1) % 2;
            atkExecuted++;
        }
    }
}

public enum YoneSword {
    Divine,
    Devil
}