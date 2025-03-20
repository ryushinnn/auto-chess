using System;

/// <summary>
/// don danh se gay st = 60% st vat ly + 60% st phep
/// </summary>
public class AttackProcessor_MissFortune : AttackProcessor {
    const float PHYSICAL_DMG_MUL = 0.6f;
    const float MAGICAL_DMG_MUL = 0.6f;
    
    public AttackProcessor_MissFortune(Hero hero) : base(hero) { }

    public override void Execute() {
        CalculateDamage(out var damage);
        hero.Mecanim.Attack(new Action[]{
            () => {
                if (hero.Target == null) return;
                var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
                    new[] {
                        Damage.Create(damage.value * PHYSICAL_DMG_MUL, DamageType.Physical, attributes.PhysicalPenetration, damage.crit),
                        Damage.Create(damage.value * MAGICAL_DMG_MUL, DamageType.Magical, attributes.MagicalPenetration, damage.crit),
                    });
                var heal = outputDamage * attributes.LifeSteal;
                if (heal > 0) {
                    attributes.Heal(heal);
                }
                attributes.RegenEnergy(hero.Trait.energyRegenPerAttack);
            }
        });
    }
}