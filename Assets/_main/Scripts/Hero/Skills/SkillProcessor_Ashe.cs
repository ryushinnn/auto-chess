using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

/// <summary>
/// ban ngau nhien 10-20 loat mua ten vao muc tieu va pham vi 1 xung quanh
/// moi loat mua ten gay 20% st vat ly kem 50% xuyen giap
/// </summary>
public class SkillProcessor_Ashe : SkillProcessor {
    const int MIN_ARROW_SET = 10;
    const int MAX_ARROW_SET = 20;
    const float DMG_MUL = 0.2f;
    const float PENETRATION = 0.5f;
    
    List<Hero> affectedTargets = new();
    
    public SkillProcessor_Ashe(Hero hero) : base(hero) {
        this.hero = hero;
        events = new Action[] { ShotArrows };
        unstoppable = false;
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
        
        var attribute = hero.GetAbility<HeroAttributes>();
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            attribute.PhysicalDamage * DMG_MUL,
            DamageType.Physical, 
            PENETRATION,
            !affectedTargets.Contains(hero.Target));
        
        affectedTargets.Add(hero.Target);
    }
}