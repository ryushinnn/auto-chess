using System;
using UnityEngine;

public class AttackProcessor_Aatrox_Dark : AttackProcessor {
    const float MIN_DMG_MUL = 0.5f;
    const float MAX_DMG_MUL = 1;

    public AttackProcessor_Aatrox_Dark(BattleHero hero) : base(hero) {
        AnimationLength = 2.167f;
        Timers = new[] { 0.9f, 1.9f };
        Description = "Đòn đánh sẽ chém liên tiếp 2 nhát, mỗi nhát gây " +
                      $"({MIN_DMG_MUL * 100}%~{MAX_DMG_MUL * 100}% <sprite name=pdmg>) sát thương vật lý " +
                      $"dựa trên máu đã mất của mục tiêu. Nhát chém đầu tiên không thể chí mạng, " +
                      $"nhát chém thứ 2 chắc chắn chí mạng.";
    }

    public override void Process(float timer) {
        base.Process(timer);

        if (trueTimer >= Timers[0] && atkExecuted == 0) {
            if (((BattleHero)hero).Target != null) {
                var targetAtb = ((BattleHero)hero).Target.GetAbility<HeroAttributes>();
                var perc = Mathf.Lerp(MIN_DMG_MUL, MAX_DMG_MUL, targetAtb.HpLostPercentage);
                var baseDmg = attributes.GetDamage(DamageType.Physical, false, scaledValues: new[] {
                    (perc, DamageType.Physical)
                });
                var outputDmg = targetAtb.TakeDamage(baseDmg);
                var heal = outputDmg * attributes.LifeSteal;
                if (heal > 0) {
                    attributes.Heal(heal);
                }
            }
            atkExecuted++;
        }
        else if (trueTimer >= Timers[1] && atkExecuted == 1) {
            if (((BattleHero)hero).Target != null) {
                var targetAtb = ((BattleHero)hero).Target.GetAbility<HeroAttributes>();
                var perc = Mathf.Lerp(MIN_DMG_MUL, MAX_DMG_MUL, targetAtb.HpLostPercentage);
                var baseDmg = attributes.GetDamage(DamageType.Physical, true, scaledValues: new[] {
                    (perc, DamageType.Physical)
                });
                var outputDmg = targetAtb.TakeDamage(baseDmg);
                var heal = outputDmg * attributes.LifeSteal;
                if (heal > 0) {
                    attributes.Heal(heal);
                }
            }
            atkExecuted++;
            attributes.RegenEnergy(hero.Trait.energyRegenPerAttack);
        }
    }
}