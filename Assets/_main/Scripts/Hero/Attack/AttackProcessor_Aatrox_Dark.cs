using System;
using UnityEngine;

/// <summary>
/// don danh gay st 2 lan, moi lan gay 30-50% st vat ly
/// dua theo mau da mat cua muc tieu
/// don danh dau tien ko the chi mang
/// don danh thu 2 chac chan chi mang
/// </summary>
public class AttackProcessor_Aatrox_Dark : AttackProcessor {
    const float MIN_DMG_MUL = 0.5f;
    const float MAX_DMG_MUL = 1;

    public AttackProcessor_Aatrox_Dark(Hero hero) : base(hero) {
        AnimationLength = 2.167f;
        Timers = new[] { 0.9f, 1.9f };
        Description = "Chém 2 lần, mỗi lần gây sát thương vật lý " +
                      $"bằng ({MIN_DMG_MUL * 100}%~{MAX_DMG_MUL * 100}% " +
                      $"sát thương vật lý) dựa trên máu đã mất của mục " +
                      $"tiêu. Nhát chém đầu tiên không thể chí mạng, " +
                      $"nhát chém thứ 2 chắc chắn chí mạng.";
    }

    public override void Process(float timer) {
        base.Process(timer);

        if (trueTimer >= Timers[0] && atkExecuted == 0) {
            if (hero.Target != null) {
                var targetAtb = hero.Target.GetAbility<HeroAttributes>();
                var perc = Mathf.Lerp(MIN_DMG_MUL, MAX_DMG_MUL, targetAtb.HpLostPercentage);
                var damage = attributes.GetDamage(DamageType.Physical, false, scaledValues: new[] {
                    (perc, DamageType.Physical)
                });
                var outputDmg = targetAtb.TakeDamage(damage);
                var heal = outputDmg * attributes.LifeSteal;
                if (heal > 0) {
                    attributes.Heal(heal);
                }
            }
            atkExecuted++;
        }
        else if (trueTimer >= Timers[1] && atkExecuted == 1) {
            if (hero.Target != null) {
                var targetAtb = hero.Target.GetAbility<HeroAttributes>();
                var perc = Mathf.Lerp(MIN_DMG_MUL, MAX_DMG_MUL, targetAtb.HpLostPercentage);
                var damage = attributes.GetDamage(DamageType.Physical, true, scaledValues: new[] {
                    (perc, DamageType.Physical)
                });
                var outputDmg = targetAtb.TakeDamage(damage);
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