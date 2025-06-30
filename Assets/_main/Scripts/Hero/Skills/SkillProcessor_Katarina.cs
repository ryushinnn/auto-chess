using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SkillProcessor_Katarina : SkillProcessor {
    public const int HITS = 10;
    public const int INTERVAL = 100; //ms
    const float DMG_MUL = 0.3f;
    const float PENETRATION = 0.7f;
    const float ANTI_HEAL_DURATION = 3;
    
    List<Hero> affectedTargets = new();

    public SkillProcessor_Katarina(Hero hero) : base(hero) {
        AnimationLength = 1.1f;
        Unstoppable = true;
        Description = "Biến thành lốc xoáy liên tục gây sát thương xung quanh, tổng cộng " +
                      $"{HITS} lần. Mỗi lần gây sát thương phép bằng ({DMG_MUL*100}% " +
                      $"sát thương vật lý) kèm {PENETRATION*100}% xuyên kháng, có thể " +
                      $"chí mạng. Kẻ địch chịu sát thương sẽ nhận hiệu ứng giảm hồi máu " +
                      $"trong {ANTI_HEAL_DURATION}s.\n" +
                      $"Trong thời gian sử dụng kỹ năng, không thể bị cản phá.";
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
        if (hero.Target == null) return;

        var dmg = attributes.GetDamage(DamageType.Magical, attributes.Crit(),
            scaledValues: new[] { (DMG_MUL, DamageType.Physical) });
        dmg.penetration = PENETRATION;
        var isNewTarget = !affectedTargets.Contains(hero.Target);
        
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg,isNewTarget);

        if (isNewTarget) {
            hero.Target.GetAbility<HeroStatusEffects>().AntiHeal(ANTI_HEAL_DURATION);
            affectedTargets.Add(hero.Target);
        }
    }
}