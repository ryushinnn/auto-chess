using System;

/// <summary>
/// tang 200% toc do danh, 20% ti le chi mang, trong 4s
/// </summary>
public class SkillProcessor_Tristana : SkillProcessor {
    const float ATK_SPD_MUL = 5f;
    const float CRIT_CHANCE_MUL = 0.2f;
    const float DURATION = 4f;
    const string EFFECT_KEY = "tristana_furious";
    
    public SkillProcessor_Tristana(Hero hero) : base(hero) {
        events = new Action[]{Greeting};
        unstoppable = true;
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