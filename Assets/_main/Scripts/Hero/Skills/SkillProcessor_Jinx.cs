using System;
using Random = UnityEngine.Random;

/// <summary>
/// ban 5 qua ten lua, moi qua gay st = 70% st vat ly, co the chi mang
/// sat thuong dau ra ngau nhien la st vat ly, st phep, hoac st chuan
/// net la st vat ly hoac st phep thi co 50% xuyen giap
/// </summary>
public class SkillProcessor_Jinx : SkillProcessor {
    const float DMG_MUL_PER_ROCKET = 0.5f;
    const float PENETRATION = 0.5f;

    public SkillProcessor_Jinx(Hero hero) : base(hero) {
        events = new Action[] { ShotRocket };
    }

    void ShotRocket() {
        if (hero.Target == null) return;

        var type = Random.Range(0, 3);
        var crit = Random.value < attributes.CriticalChance;
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(type switch {
            0 => Damage.Create(attributes.PhysicalDamage * DMG_MUL_PER_ROCKET, DamageType.Physical, PENETRATION, crit),
            1 => Damage.Create(attributes.PhysicalDamage * DMG_MUL_PER_ROCKET, DamageType.Magical, PENETRATION, crit),
            _ => Damage.Create(attributes.PhysicalDamage * DMG_MUL_PER_ROCKET, DamageType.True, 0,crit),
        });
    }
}