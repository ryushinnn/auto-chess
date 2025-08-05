public class SkillProcessor_MissFortune : SkillProcessor {
    const float L_PHYSICAL_DMG_MUL_0 = 0.6f;
    const float L_MAGICAL_DMG_MUL_0 = 0.3f;
    const float L_PHYSICAL_DMG_MUL_1 = 0.3f;
    const float L_MAGICAL_DMG_MUL_1 = 0.6f;
    const float R_PHYSICAL_DMG_MUL_0 = 1f;
    const float R_MAGICAL_DMG_MUL_0 = 0.5f;
    const float R_PHYSICAL_DMG_MUL_1 = 0.5f;
    const float R_MAGICAL_DMG_MUL_1 = 1f;
    const float VAMP_MUL = 0.3f;
    const float REDUCE_DEFENSE_MUL = -0.3f;
    const float REDUCE_DEFENSE_DURATION = 5f;
    const string EFFECT_KEY = "missfortune_shield_breaker";

    public SkillProcessor_MissFortune(Hero hero) : base(hero) {
        AnimationLength = 2.8f;
        Timers = new[] { 1f, 1.3f };
        Name = "Hơi thở của Hoa Cải!";
        Description = "Bắn lần lượt 2 viên đạn.\n" +
                      $"- Viên đầu tiên: Gây ({L_PHYSICAL_DMG_MUL_0 * 100}% <sprite name=pdmg> + " +
                      $"{L_MAGICAL_DMG_MUL_0*100}% <sprite name=mdmg>) sát thương vật lý cùng với " +
                      $"({L_PHYSICAL_DMG_MUL_1*100}% <sprite name=pdmg> + {L_MAGICAL_DMG_MUL_1 * 100}% <sprite name=mdmg>) " +
                      $"sát thương phép. Hồi máu bằng {VAMP_MUL * 100}% sát thương gây ra.\n" +
                      $"- Viên thứ 2: Gây ({R_PHYSICAL_DMG_MUL_0 * 100}% <sprite name=pdmg> + " +
                      $"{R_MAGICAL_DMG_MUL_0*100}% <sprite name=mdmg>) sát thương vật lý cùng với " +
                      $"({R_PHYSICAL_DMG_MUL_1*100}% <sprite name=pdmg> + {R_MAGICAL_DMG_MUL_1 * 100}% <sprite name=mdmg>) " +
                      $"sát thương phép. Mục tiêu bị giảm {REDUCE_DEFENSE_MUL * -100}% <sprite name=arm> và <sprite name=res> " +
                      $"trong {REDUCE_DEFENSE_DURATION}s.\n" +
                      $"Các viên đạn đều có thể chí mạng.";
    }

    public override void Process(float timer) {
        if (timer >= Timers[0] && skillExecuted == 0) {
            ShotLeft();
            skillExecuted++;
        }
        else if (timer >= Timers[1] && skillExecuted == 1) {
            ShotRight();
            skillExecuted++;
        }
    }

    void ShotLeft() {
        if (((BattleHero)hero).Target == null) return;

        var crit = attributes.Crit();
        var phyDmg = attributes.GetDamage(DamageType.Physical, crit, scaledValues:
            new[] {
                (L_PHYSICAL_DMG_MUL_0, DamageType.Physical),
                (L_MAGICAL_DMG_MUL_0, DamageType.Magical)
            });
        var magDmg = attributes.GetDamage(DamageType.Magical, crit, scaledValues:
            new[] {
                (L_PHYSICAL_DMG_MUL_1, DamageType.Physical),
                (L_MAGICAL_DMG_MUL_1, DamageType.Magical)
            });
        
        var outputDmg = ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(new[] {phyDmg, magDmg});
        attributes.Heal(outputDmg * VAMP_MUL);
    }

    void ShotRight() {
        if (((BattleHero)hero).Target == null) return;

        var crit = attributes.Crit();
        var phyDmg = attributes.GetDamage(DamageType.Physical, crit, scaledValues:
            new[] {
                (R_PHYSICAL_DMG_MUL_0, DamageType.Physical),
                (R_MAGICAL_DMG_MUL_0, DamageType.Magical)
            });
        var magDmg = attributes.GetDamage(DamageType.Magical, crit, scaledValues:
            new[] {
                (R_PHYSICAL_DMG_MUL_1, DamageType.Physical),
                (R_MAGICAL_DMG_MUL_1, DamageType.Magical)
            });
        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(new[] {phyDmg, magDmg});
        
        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().AddAttributeModifier(
            AttributeModifierSet.Create(
                hero,
                EFFECT_KEY,
                REDUCE_DEFENSE_DURATION,
                new[] {
                    (AttributeModifierKey.Armor, REDUCE_DEFENSE_MUL, AttributeModifier.Type.Percentage),
                    (AttributeModifierKey.Resistance, REDUCE_DEFENSE_MUL, AttributeModifier.Type.Percentage),
                }
            ));
    }
}