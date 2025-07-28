public class SkillProcessor_Irelia : SkillProcessor {
    const float DMG_MUL = 2.5f;
    const float AIRBRONE_TIME = 1; //ms
    
    public SkillProcessor_Irelia(Hero hero) : base(hero) {
        AnimationLength = 4.8f;
        Timers = new[] { 4.23f };
        Name = "Hắc Long Chưởng";
        Description = $"Triệu hồi rồng thần tấn công kẻ địch gây ({DMG_MUL * 100}% <sprite name=pdmg>) sát thương phép " +
                      $"và hất tung mục tiêu trong {AIRBRONE_TIME}s.";
        Unstoppable = true;
    }

    public override void Process(float timer) {
        if (timer >= Timers[0] && skillExecuted == 0) {
            SummonDragon();
            skillExecuted++;
        }
    }

    void SummonDragon() {
        if (hero.Target == null) return;

        hero.Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Magical, false,
            scaledValues: new[]{(DMG_MUL, DamageType.Physical)}));
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBRONE_TIME);
    }
}