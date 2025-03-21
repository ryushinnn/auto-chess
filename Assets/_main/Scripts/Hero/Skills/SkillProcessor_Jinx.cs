using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

/// <summary>
/// ban 5 qua ten lua, moi qua gay st = 70% st vat ly, co the chi mang
/// sat thuong dau ra ngau nhien la st vat ly, st phep, hoac st chuan
/// net la st vat ly hoac st phep thi co 50% xuyen giap
/// </summary>
public class SkillProcessor_Jinx : SkillProcessor {
    public const int ROCKETS = 5;
    public const int INTERVAL = 200;
    const float DMG_MUL_PER_ROCKET = 0.5f;
    const float PENETRATION = 0.5f;

    List<Hero> affectedTargets = new();
    
    public SkillProcessor_Jinx(Hero hero) : base(hero) {
        events = new Action[] { ShotRockets };
        unstoppable = true;
    }

    async void ShotRockets() {
        affectedTargets.Clear();
        ShotRocket();
        for (int i = 1; i < ROCKETS; i++) {
            await Task.Delay(INTERVAL);
            ShotRocket();
        }
    }

    void ShotRocket() {
        if (hero.Target == null) return;

        var type = Random.Range(0, 3);
        var dmg = attributes.PhysicalDamage * DMG_MUL_PER_ROCKET;
        var crit = attributes.Crit();
        if (crit) {
            dmg *= attributes.CriticalDamage;
        }
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(type switch {
            0 => Damage.Create(dmg * DMG_MUL_PER_ROCKET, DamageType.Physical, PENETRATION, crit),
            1 => Damage.Create(dmg * DMG_MUL_PER_ROCKET, DamageType.Magical, PENETRATION, crit),
            _ => Damage.Create(dmg * DMG_MUL_PER_ROCKET, DamageType.True, 0,crit),
        }
        , !affectedTargets.Contains(hero.Target));
        
        affectedTargets.Add(hero.Target);
    }
}