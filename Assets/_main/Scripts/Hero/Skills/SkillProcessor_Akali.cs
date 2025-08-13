public class SkillProcessor_Akali : SkillProcessor {
    const float ENERGY_THRESHOLD = 50;
    const float SILENT_DURATION = 2;
    const float STUN_DURATION = 1;
    
    public SkillProcessor_Akali(BattleHero hero) : base(hero) {
        animationLength = 1;
        timers = new[] { 0.2f };
        Name = "Ám Quang Chi Thuật";
        Description = $"Ném bom mù vào 1 mục tiêu, nếu mục tiêu có ít nhất {ENERGY_THRESHOLD} " +
                      $"<sprite name=eng>, gây câm lặng trong {SILENT_DURATION}s, ngược lại gây " +
                      $"choáng trong {STUN_DURATION}s.";
    }

    public override void Process(float timer) {
        if (timer >= timers[0] && skillExecuted == 0) {
            ThrowBomb();
            skillExecuted++;
        }
    }

    void ThrowBomb() {
        if (((BattleHero)hero).Target == null) return;

        if (((BattleHero)hero).Target.GetAbility<HeroAttributes>().Energy >= ENERGY_THRESHOLD) {
            ((BattleHero)hero).Target.GetAbility<HeroStatusEffects>().Silent(SILENT_DURATION);
        }
        else {
            ((BattleHero)hero).Target.GetAbility<HeroStatusEffects>().Stun(STUN_DURATION);
        }
    }
}