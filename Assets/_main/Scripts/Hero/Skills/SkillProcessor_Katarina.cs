using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

/// <summary>
/// bien thanh loc xoay lien tuc gay st xung quanh 10 lan
/// moi lan gay st phep = 30% st vat ly, kem theo 80% xuyen khang phep, co the chi mang
/// ke dich chiu st se nhan antiheal 3s
/// </summary>
public class SkillProcessor_Katarina : SkillProcessor {
    public const int HITS = 10;
    public const int INTERVAL = 100; //ms
    const float DMG_MUL = 0.3f;
    const float PENETRATION = 0.7f;
    const float ANTI_HEAL_DURATION = 3;
    
    List<Hero> affectedTargets = new();

    public SkillProcessor_Katarina(Hero hero) : base(hero) {
        events = new Action[] { TurnAround };
        unstoppable = true;
    }

    async void TurnAround() {
        affectedTargets.Clear();
        Cut();
        for (int i=1; i<HITS; i++) {
            await Task.Delay(INTERVAL);
            Cut();
        }
    }

    void Cut() {
        if (hero.Target == null) return;
        
        var crit = Random.value < attributes.CriticalChance;
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                attributes.PhysicalDamage * DMG_MUL,
                DamageType.Magical,
                PENETRATION,
                crit
            ),!affectedTargets.Contains(hero.Target));

        if (!affectedTargets.Contains(hero.Target)) {
            hero.Target.GetAbility<HeroStatusEffects>().AntiHeal(ANTI_HEAL_DURATION);
        }
        
        affectedTargets.Add(hero.Target);
    }
}