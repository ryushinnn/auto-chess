using System;

public class AttackProcessor_MissFortune : AttackProcessor {
    const float PHYSICAL_DMG_MUL_0 = 0.7f;
    const float MAGICAL_DMG_MUL_0 = 0.3f;
    const float PHYSICAL_DMG_MUL_1 = 0.3f;
    const float MAGICAL_DMG_MUL_1 = 0.7f;
    

    public AttackProcessor_MissFortune(Hero hero) : base(hero) {
        AnimationLength = 1.167f;
        Timers = new[] { 0.35f };
        Description = "Bắn cùng lúc 2 viên đạn. 1 viên gây sát thương " +
                      $"vật lý bằng ({PHYSICAL_DMG_MUL_0 * 100}% sát thương vật lý + " +
                      $"{MAGICAL_DMG_MUL_0*100}% sát thương phép), viên còn lại " +
                      $"gây sát thương phép bằng ({PHYSICAL_DMG_MUL_1*100}% sát thương " +
                      $"vật lý + {MAGICAL_DMG_MUL_1 * 100}% sát thương phép)";
    }

    public override void Process(float timer) {
        base.Process(timer);

        if (trueTimer >= Timers[0] && atkExecuted == 0) {
            if (hero.Target != null) {
                var crit = attributes.Crit();
                var phyDmg = attributes.GetDamage(DamageType.Physical, crit, scaledValues: new[] {
                    (PHYSICAL_DMG_MUL_0, DamageType.Physical),
                    (MAGICAL_DMG_MUL_0, DamageType.Magical)
                });
                var magDmg = attributes.GetDamage(DamageType.Magical, crit, scaledValues: new[] {
                    (PHYSICAL_DMG_MUL_1, DamageType.Physical),
                    (MAGICAL_DMG_MUL_1, DamageType.Magical)
                });
                var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(new[] { phyDmg, magDmg });
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