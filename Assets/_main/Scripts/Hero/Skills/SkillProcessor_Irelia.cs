using System;

/// <summary>
/// trieu hoi rong tan cong ke dich, gay st phep = 250% st vat ly
/// hat tung ke dich trong 1s
/// </summary>
public class SkillProcessor_Irelia : SkillProcessor {
    const float DMG_MUL = 2.5f;
    const float AIRBRONE_TIME = 1; //ms
    
    public SkillProcessor_Irelia(Hero hero) : base(hero) {
        events = new Action[] { SummonDragon };
        unstoppable = true;
    }

    void SummonDragon() {
        if (hero.Target == null) return;

        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(attributes.PhysicalDamage * DMG_MUL, DamageType.Magical, attributes.MagicalPenetration));
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBRONE_TIME);
    }
}