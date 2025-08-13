public class SkillProcessor_Irelia : SkillProcessor {
    const float DMG_MUL = 2.5f;
    const float AIRBRONE_TIME = 1; //ms
    
    public SkillProcessor_Irelia(BattleHero hero) : base(hero) {
        animationLength = 4.8f;
        timers = new[] { 4.23f };
        Name = "Hắc Long Chưởng";
        Description = $"Triệu hồi rồng thần tấn công kẻ địch gây ({DMG_MUL * 100}% <sprite name=pdmg>) sát thương phép " +
                      $"và hất tung mục tiêu trong {AIRBRONE_TIME}s.";
        unstoppable = true;
    }

    public override void Process(float timer) {
        if (timer >= timers[0] && skillExecuted == 0) {
            SummonDragon();
            skillExecuted++;
        }
    }

    void SummonDragon() {
        if (((BattleHero)hero).Target == null) return;

        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Magical, false,
            scaledValues: new[]{(DMG_MUL, DamageType.Physical)}));
        ((BattleHero)hero).Target.GetAbility<HeroStatusEffects>().Airborne(AIRBRONE_TIME);
    }
}