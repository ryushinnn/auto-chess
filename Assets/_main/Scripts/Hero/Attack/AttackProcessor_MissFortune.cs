using System;

public class AttackProcessor_MissFortune : AttackProcessor {
    const float PHYSICAL_DMG_MUL_0 = 0.7f;
    const float MAGICAL_DMG_MUL_0 = 0.3f;
    const float PHYSICAL_DMG_MUL_1 = 0.3f;
    const float MAGICAL_DMG_MUL_1 = 0.7f;
    

    public AttackProcessor_MissFortune(BattleHero hero) : base(hero) {
        AnimationLength = 1.167f;
        Timers = new[] { 0.35f };
        Description = $"Đòn đánh bắn cùng lúc 2 viên đạn. 1 viên gây ({PHYSICAL_DMG_MUL_0 * 100}% <sprite name=pdmg> + " +
                      $"{MAGICAL_DMG_MUL_0*100}% <sprite name=mdmg>) sát thương vật lý, viên còn lại " +
                      $"gây ({PHYSICAL_DMG_MUL_1*100}% <sprite name=pdmg> + {MAGICAL_DMG_MUL_1 * 100}% <sprite name=mdmg>) " +
                      $"sát thương phép.";
    }

    public override void Process(float timer) {
        base.Process(timer);

        if (trueTimer >= Timers[0] && atkExecuted == 0) {
            if (((BattleHero)hero).Target != null) {
                var crit = attributes.Crit();
                var phyDmg = attributes.GetDamage(DamageType.Physical, crit, scaledValues: new[] {
                    (PHYSICAL_DMG_MUL_0, DamageType.Physical),
                    (MAGICAL_DMG_MUL_0, DamageType.Magical)
                });
                var magDmg = attributes.GetDamage(DamageType.Magical, crit, scaledValues: new[] {
                    (PHYSICAL_DMG_MUL_1, DamageType.Physical),
                    (MAGICAL_DMG_MUL_1, DamageType.Magical)
                });
                var outputDmg = ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(new[] { phyDmg, magDmg });
                var heal = outputDmg * attributes.LifeSteal;
                if (heal > 0) {
                    attributes.Heal(heal);
                }
                attributes.RegenEnergy(hero.Trait.energyRegenPerAttack);
            }

            atkExecuted++;
        }
    }
}