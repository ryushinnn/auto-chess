using System;

/// <summary>
/// moi don danh thu 3 se gay them 50% st chuan
/// </summary>
public class AttackProcessor_Yasuo : AttackProcessor {
    int count = 0;
    
    const float DMG_MUL = 0.5f;
    const int COUNT_LIMIT = 2;
    
    public AttackProcessor_Yasuo(Hero hero) : base(hero) { }

    public override void Execute() {
        CalculateDamage(out var damage);
        hero.Mecanim.Attack(new Action[]{
            () => {
                if (hero.Target == null) return;

                float outputDamage;
                if (count == COUNT_LIMIT) {
                    outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(new[] {
                        damage,
                        Damage.Create(damage.value * DMG_MUL, DamageType.True, 0, damage.crit),
                    });
                    count = 0;
                }
                else {
                    outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(damage);
                    count++;
                }

                var heal = outputDamage * attributes.LifeSteal;
                if (heal > 0) {
                    hero.GetAbility<HeroAttributes>().Heal(heal);
                }

                hero.GetAbility<HeroAttributes>().RegenEnergy(hero.Trait.energyRegenPerAttack);
            }
        });
    }
}