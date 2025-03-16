using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// ban ngau nhien 5-10 loat mua ten vao muc tieu va pham vi 1 xung quanh
/// moi loat mua ten gay 25% st vat ly kem 50% xuyen giap va co the chi mang
/// </summary>
public class SkillProcessor_Ashe : SkillProcessor {
    const int MIN_ARROW_SET = 10;
    const int MAX_ARROW_SET = 20;
    const float DMG_MUL = 0.2f;
    const float PENETRATION = 0.5f;
    
    List<Hero> affectedTargets = new();
    
    public SkillProcessor_Ashe(Hero hero) : base(hero) {
        events = new Action[] { ShotArrows };
    }

    async void ShotArrows() {
        affectedTargets.Clear();
        var set = Random.Range(MIN_ARROW_SET, MAX_ARROW_SET + 1);
        var totalTime = 2000; //ms
        var timePerSet = totalTime / set;
        ShotArrow();
        for (int i=1; i<set; i++) {
            await Task.Delay(timePerSet);
            ShotArrow();
        }
    }

    void ShotArrow() {
        if (hero.Target == null) return;
        
        var dmg = attributes.PhysicalDamage * DMG_MUL;
        var crit = Random.value < attributes.CriticalChance;
        if (crit) {
            dmg *= attributes.CriticalDamage;
        }
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                dmg,
                DamageType.Physical, 
                PENETRATION,
                crit
            ), !affectedTargets.Contains(hero.Target));
        
        affectedTargets.Add(hero.Target);
    }
}