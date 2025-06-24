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
        AnimationLength = 1.367f;
        Timers = new[] { 0.66f };
        Description = "Thay đổi qua lại giữa Thần Kiếm và Quỷ Kiếm sau mỗi đòn đánh.\n" +
                      $"Thần Kiếm: Thêm 1 cộng dồn (tối đa {DIVINE_WEAKENING_MAX_STACK} cộng dồn " +
                      $"và làm mới thời gian duy trì ({DIVINE_WEAKENING_DURATION}s) hiệu ứng SUY YẾU " +
                      $"trên mục tiêu\n" +
                      $"Quỷ Kiếm: Cho thêm tối đa {DEVIL_VAMP_MAX*100}% hút máu, dựa vào " +
                      $"máu đã mất\n" +
                      $"Thanh Kiếm cuối cùng được sử dụng sẽ quyết định kỹ năng sẽ kích hoạt\n" +
                      $"SUY YẾU: Giảm {DIVINE_WEAKENING_PER_STACK * -100}% sát thương";
        
        customInt = new CustomData<int>();
        customInt["sword"] = (int)YoneSword.Divine;
    }

    public override void Process(float timer) {
        base.Process(timer);

        if (trueTimer >= Timers[0] && atkExecuted == 0) {
            if (hero.Target != null) {
                var sword = (YoneSword)customInt["sword"];
                var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical));
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