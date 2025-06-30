using System;
using System.Collections.Generic;

public class SkillProcessor_Malphite : SkillProcessor {
    const string EFFECT_KEY = "malphite_unstoppable";
    const float DEFENSE_MUL = 2;
    const float ATK_SPEED_REDUCE_MUL = -0.5f;
    const float EFFECT_DURATION = 5;
    const float DMG_MUL = 1f;
    const float AIRBORNE_DURATION = 2f;
    
    public SkillProcessor_Malphite(Hero hero) : base(hero) {
        AnimationLength = 4.7f;
        Timers = new[] { 0.46f, 2.3f };
        Description = $"Tăng {DEFENSE_MUL * 100}% giáp và kháng phép, tối đa kháng hiệu ứng, " +
                      $"nhưng bị giảm {ATK_SPEED_REDUCE_MUL * -100}% tốc độ đánh, " +
                      $"duy trì {EFFECT_DURATION}s. Vận sức nhảy lên rồi giáng xuống " +
                      $"gây sát thương vật lý bằng ({DMG_MUL * 100}% sát thương vật lý) " +
                      $"và hất tung kẻ địch trong {AIRBORNE_DURATION}s.";
    }

    public override void Process(float timer) {
        if (timer >= Timers[0] && skillExecuted == 0) {
            Strengthen();
            skillExecuted++;
        }
        else if (timer >= Timers[1] && skillExecuted == 1) {
            Slam();
            skillExecuted++;
        }
    }

    void Strengthen() {
        if (!attributes.IsAlive) return;
        
        attributes.AddAttributeModifier(
            AttributeModifierSet.Create(
                hero,
                EFFECT_KEY,
                EFFECT_DURATION,
                new [] {
                    (AttributeModifierKey.Armor, DEFENSE_MUL, AttributeModifier.Type.Percentage),
                    (AttributeModifierKey.Resistance, DEFENSE_MUL, AttributeModifier.Type.Percentage),
                    (AttributeModifierKey.AttackSpeed, ATK_SPEED_REDUCE_MUL, AttributeModifier.Type.Percentage),
                    (AttributeModifierKey.Tenacity, HeroTrait.MAX_TENACITY, AttributeModifier.Type.FixedValue),
                }
            ));
    }

    void Slam() {
        if (hero.Target == null) return;
        
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical, false,
            scaledValues: new[] { (DMG_MUL, DamageType.Physical) }));
        
        hero.Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_DURATION);
    }
}