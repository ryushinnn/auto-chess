using System;
using RExt.Extension;
using UnityEngine;

public class AttackProcessor_Teemo : AttackProcessor {
    public const string DOT_KEY = "teemo_hell_flame";
    public const int MAX_STACKS = 5;
    public const int TOTAL_TIME = 3000; //ms
    public const int INTERVAL = 500; //ms
    public const float MAX_HP_DMG = 0.001f;
    public const float DMG_MUL_LIMIT = 0.01f;

    public AttackProcessor_Teemo(Hero hero) : base(hero) {
        AnimationLength = 0.933f;
        Timers = new[] { 0.17f };
        Description = $"Đòn đánh sẽ thêm 1 cộng dồn (tối đa {MAX_STACKS} cộng dồn) " +
                      $"và làm mới thời gian duy trì ({TOTAL_TIME / 1000f}s) của hiệu ứng <color=red>HOẢ NGỤC</color>.\n" +
                      $"<color=red>HOẢ NGỤC</color>: Mỗi {INTERVAL / 1000f}s gây sát thương chuẩn bằng {MAX_HP_DMG * 100}% " +
                      $"<sprite name=hp> tối đa của mục tiêu, tối đa bằng {DMG_MUL_LIMIT * 100}% <sprite name=mdmg>.";
    }

    public override void Process(float timer) {
        base.Process(timer);

        if (trueTimer >= Timers[0] && atkExecuted == 0) {
            if (hero.Target != null) {
                var currentStacks = hero.Target.GetAbility<HeroMark>().GetMark(DOT_KEY, hero)?.stacks ?? 0;
                var nextStacks = Mathf.Min(currentStacks + 1, MAX_STACKS);

                var igniteDmg = Damage.Create(
                    nextStacks * Mathf.Min(attributes.MagicalDamage * DMG_MUL_LIMIT, hero.Target.GetAbility<HeroAttributes>().MaxHp * MAX_HP_DMG),
                    DamageType.True,
                    0
                );

                var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
                    new[] {
                        attributes.GetDamage(DamageType.Magical),
                        igniteDmg,
                    }) - igniteDmg.value;
                // why subtract ignite dmg???
                // add it to TakeDamage just for nicer visual (2 HpText arranges vertically),
                // it doesn't actually count as this attack's dmg

                var heal = outputDmg * attributes.LifeSteal;
                if (heal > 0) {
                    attributes.Heal(heal);
                }

                attributes.RegenEnergy(hero.Trait.energyRegenPerAttack);

                hero.Target.GetAbility<HeroAttributes>().AddDamageOverTime(
                    DamageOverTime.Create(
                        DOT_KEY,
                        hero,
                        igniteDmg,
                        (TOTAL_TIME / INTERVAL) - 1,
                        INTERVAL.ToSeconds(),
                        nextStacks,
                        false,
                        true
                    ));
                // why subtract 1 from stack???
                // 1 stack is already applied in TakeDamage
            }

            atkExecuted++;
        }
    }
}