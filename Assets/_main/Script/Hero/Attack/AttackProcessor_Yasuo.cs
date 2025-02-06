/// <summary>
/// moi don danh thu 3 se gay them 50% st chuan
/// </summary>
public class AttackProcessor_Yasuo : AttackProcessor {
    int count = 0;
    
    const float DMG_MUL = 0.5f;
    const int COUNT_LIMIT = 2;
    
    public AttackProcessor_Yasuo(Hero hero) : base(hero) {
        this.hero = hero;
    }

    public override void Execute() {
        CalculateDamage(out var dmg, out var type, out var pen);
        hero.Mecanim.Attack(() => {
            if (hero.Target == null) return;
            
            var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg, type, pen);

            if (count == COUNT_LIMIT) {
                outputDamage += hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg * DMG_MUL, DamageType.True, 0);
                count = 0;
            }
            else {
                count++;
            }
            hero.GetAbility<HeroAttributes>().Heal(outputDamage * hero.GetAbility<HeroAttributes>().LifeSteal);
            hero.GetAbility<HeroAttributes>().RegenEnergy(hero.Trait.energyRegenPerAttack);
        });
    }
}