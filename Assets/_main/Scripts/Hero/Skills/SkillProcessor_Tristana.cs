﻿public class SkillProcessor_Tristana : SkillProcessor {
    const float ATK_SPD_MUL = 5f;
    const float CRIT_CHANCE_MUL = 0.2f;
    const float DURATION = 4f;
    const string EFFECT_KEY = "tristana_furious";
    
    public SkillProcessor_Tristana(Hero hero) : base(hero) {
        AnimationLength = 2.2f;
        Timers = new[] { 0.625f };
        Unstoppable = true;
        Name = "Chiến Pháo Cuồng Nộ";
        Description = $"Tăng {ATK_SPD_MUL*100}% <sprite name=aspd> và " +
                      $"{CRIT_CHANCE_MUL * 100}% <sprite name=crc>, duy trì {DURATION}s.";
    }

    public override void Process(float timer) {
        if (timer >= Timers[0] && skillExecuted == 0) {
            Greeting();
            skillExecuted++;
        }
    }

    void Greeting() {
        if (!attributes.IsAlive) return;
        
        attributes.AddAttributeModifier(
            AttributeModifierSet.Create(
                hero,
                EFFECT_KEY,
                DURATION,
                new[] {
                    (AttributeModifierKey.AttackSpeed, ATK_SPD_MUL, AttributeModifier.Type.Percentage),
                    (AttributeModifierKey.CriticalChance, CRIT_CHANCE_MUL, AttributeModifier.Type.Percentage)
                }
            ));
    }
}