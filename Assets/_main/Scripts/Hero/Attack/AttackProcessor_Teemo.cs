using RExt.Extension;
using UnityEngine;

/// <summary>
/// don danh se kem theo 1 cong don hoa nguc trong 3s
/// moi 0.5s gay st chuan = 0.1% maxhp cua ke dich, toi da = 1% st phep
/// cong don toi da 5 lan
/// </summary>
public class AttackProcessor_Teemo : AttackProcessor {
    public const string DOT_KEY = "teemo_hell_flame";
    public const int MAX_STACKS = 5;
    public const int TOTAL_TIME = 3000; //ms
    public const int INTERVAL = 500; //ms
    public const float MAX_HP_DMG = 0.001f;
    public const float DMG_MUL_LIMIT = 0.01f;
    
    public AttackProcessor_Teemo(Hero hero) : base(hero) { }

    public override void Execute() {
        CalculateDamage(out var dmg, out var type, out var pen, out var crit);
        hero.Mecanim.Attack(() => {
            if (hero.Target == null) return;
            
            var currentStacks = hero.Target.GetAbility<HeroMark>().GetMark(DOT_KEY, hero)?.stacks ?? 0;
            var nextStacks = Mathf.Min(currentStacks + 1, MAX_STACKS);
            
            var igniteDmg = Damage.Create(
                nextStacks * Mathf.Min(attributes.MagicalDamage * DMG_MUL_LIMIT, hero.Target.GetAbility<HeroAttributes>().MaxHp * MAX_HP_DMG),
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
        });
    }
}