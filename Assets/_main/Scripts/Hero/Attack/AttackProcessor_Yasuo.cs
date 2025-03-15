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
        CalculateDamage(out var dmg, out var type, out var pen, out var crit);
        hero.Mecanim.Attack(() => {
            if (hero.Target == null) return;
            
            var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg, type, pen, crit);

            if (count == COUNT_LIMIT) {
                outputDamage += hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg * DMG_MUL, DamageType.True, 0, crit);
                count = 0;
            }
            else {
                count++;
            }

            var heal = outputDamage * attributes.LifeSteal;
            if (heal > 0) {
                hero.GetAbility<HeroAttributes>().Heal(heal);
            }
            hero.GetAbility<HeroAttributes>().RegenEnergy(hero.Trait.energyRegenPerAttack);
        });
    }
}