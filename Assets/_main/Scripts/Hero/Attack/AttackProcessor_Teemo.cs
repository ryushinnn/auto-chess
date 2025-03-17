using RExt.Extension;
using UnityEngine;

/// <summary>
/// don danh se gay them st thieu dot trong 5s
/// moi 1s gay st chuan = 0.4% maxhp cua ke dich, toi da = 4% st phep
/// ke dich co the nhan hieu ung thieu dot nhieu lan
/// </summary>
public class AttackProcessor_Teemo : AttackProcessor {
    public const string KEY = "teemo_ignite";
    public const int TOTAL_TIME = 5000; //ms
    public const int INTERVAL = 1000; //ms
    public const float MAX_HP_DMG = 0.004f;
    public const float DMG_MUL_LIMIT = 0.04f;
    
    public AttackProcessor_Teemo(Hero hero) : base(hero) { }

    public override void Execute() {
        CalculateDamage(out var dmg, out var type, out var pen, out var crit);
        hero.Mecanim.Attack(() => {
            if (hero.Target == null) return;
            
            var igniteDmg = Damage.Create(
                Mathf.Min(attributes.MagicalDamage * DMG_MUL_LIMIT, hero.Target.GetAbility<HeroAttributes>().MaxHp * MAX_HP_DMG),
                DamageType.True,
                0
            );

            var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
                new[] {
                    Damage.Create(dmg, type, pen, crit),
                    igniteDmg,
                }) - igniteDmg.value;
            // why subtract ignite dmg???
            // add it to TakeDamage just for nicer visual (2 HpText arranges vertically),
            // it doesn't actually count as this attack's dmg
            
            var heal = outputDamage * attributes.LifeSteal;
            if (heal > 0) {
                attributes.Heal(heal);
            }
            attributes.RegenEnergy(hero.Trait.energyRegenPerAttack);
            
            hero.Target.GetAbility<HeroAttributes>().AddDamageOverTime(
                DamageOverTime.Create(
                        KEY,
                        igniteDmg, 
                        (TOTAL_TIME / INTERVAL) - 1,
                        INTERVAL.ToSeconds(),
                        false,
                        false,
                        true
                    ));
            // why subtract 1 from stack???
            // 1 stack is already applied in TakeDamage
        });
    }
}