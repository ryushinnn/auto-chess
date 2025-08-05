using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SkillProcessor_Katarina : SkillProcessor {
    public const int HITS = 15;
    public const int INTERVAL = 200; //ms
    const float DMG_MUL = 0.3f;
    const float ANTI_HEAL_DURATION = 3;
    
    List<Hero> affectedTargets = new();

    public SkillProcessor_Katarina(Hero hero) : base(hero) {
        AnimationLength = 3.1f;
        Unstoppable = true;
        Name = "Đoạn Thuỷ Toái Phong Đao";
        Description = $"Biến thành lốc xoáy, liên tục gây sát thương xung quanh. " +
                      $"Mỗi lần gây ({DMG_MUL*100}% <sprite name=pdmg>) sát thương phép và có thể " +
                      $"chí mạng, tổng cộng {HITS} lần. Kẻ địch chịu sát thương sẽ nhận hiệu ứng giảm hồi máu " +
                      $"trong {ANTI_HEAL_DURATION}s.";
    }

    public override void Process(float timer) {
        if (skillExecuted == 0) {
            TurnAround();
            skillExecuted++;
        }
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
        if (((BattleHero)hero).Target == null) return;

        var dmg = attributes.GetDamage(DamageType.Magical, attributes.Crit(),
            scaledValues: new[] { (DMG_MUL, DamageType.Physical) });
        var isNewTarget = !affectedTargets.Contains(((BattleHero)hero).Target);
        
        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(dmg,isNewTarget);

        if (isNewTarget) {
            ((BattleHero)hero).Target.GetAbility<HeroStatusEffects>().AntiHeal(ANTI_HEAL_DURATION);
            affectedTargets.Add(((BattleHero)hero).Target);
        }
    }
}