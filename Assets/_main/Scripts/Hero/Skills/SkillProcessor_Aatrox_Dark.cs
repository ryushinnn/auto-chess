using System;

public class SkillProcessor_Aatrox_Dark : SkillProcessor {
    const float DMG_MUL_0 = 0.7f;
    const float DMG_MUL_1 = 1.5f;
    const float DMG_MUL_2 = 2f;
    const float AIRBORNE_TIME_0 = 0.2f;
    const float AIRBORNE_TIME_1 = 0.2f;
    const float AIRBORNE_TIME_2 = 0.5f;
    const float ARMOR_REDUCE_MUL = -0.25f;
    const float ARMOR_REDUCE_DURATION = 5f;
    const string EFFECT_KEY = "aatrox_curse";
    const float BONUS_TRUE_DMG_MUL = 0.5f;
    const float HP_THRESHOLD = 0.5f;

    public SkillProcessor_Aatrox_Dark(BattleHero hero) : base(hero) {
        animationLength = 5;
        timers = new[] { 0.6f, 2.3f, 4.1f };
        unstoppable = true;
        Name = "Nhật Thực: Huyết Sát Cửu U";
        Description = $"Chém 3 nhát, lần lượt gây ({DMG_MUL_0 * 100}%/{DMG_MUL_1 * 100}%/{DMG_MUL_2 * 100}% <sprite name=pdmg>) sát thương vật lý " +
                      $"và hất tung mục tiêu trong {AIRBORNE_TIME_0}s/{AIRBORNE_TIME_1}s/{AIRBORNE_TIME_2}s. " +
                      $"Nếu mục tiêu dưới {HP_THRESHOLD * 100}% <sprite name=hp>, các nhát chém " +
                      $"sẽ gây thêm hiệu ứng:\n" +
                      $"- Đầu tiên: giảm {ARMOR_REDUCE_MUL * -100}% <sprite name=arm> trong {ARMOR_REDUCE_DURATION}s\n" +
                      $"- Thứ 2: gây thêm ({BONUS_TRUE_DMG_MUL * 100}% <sprite name=pdmg>) sát thương chuẩn\n" +
                      $"- Thứ 3: Có thể chí mạng";
    }

    public override void Process(float timer) {
        if (timer >= timers[0] && skillExecuted == 0) {
            LightSlash();
            skillExecuted++;
        }
        else if (timer >= timers[1] && skillExecuted == 1) {
            MediumSlash();
            skillExecuted++;
        }
        else if (timer >= timers[2] && skillExecuted == 2) {
            HeavySlash();
            skillExecuted++;
        }
    }

    void LightSlash() {
        if (((BattleHero)hero).Target == null) return;
        
        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical, false,
            scaledValues: new[] { (DMG_MUL_0, DamageType.Physical) })
        );
        
        if (((BattleHero)hero).Target.GetAbility<HeroAttributes>().HpPercentage < HP_THRESHOLD) {
            ((BattleHero)hero).Target.GetAbility<HeroAttributes>().AddAttributeModifier(
                AttributeModifierSet.Create(
                    hero,
                    EFFECT_KEY,
                    ARMOR_REDUCE_DURATION,
                    new[] {
                        (AttributeModifierKey.Armor, ARMOR_REDUCE_MUL, AttributeModifier.Type.Percentage)
                    }
                ));
        }
        
        ((BattleHero)hero).Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_0);
    }

    void MediumSlash() {
        if (((BattleHero)hero).Target == null) return;
        
        if (((BattleHero)hero).Target.GetAbility<HeroAttributes>().HpPercentage >= HP_THRESHOLD) {
            ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical, false,
                scaledValues: new[] { (DMG_MUL_1, DamageType.Physical) })
            );
        }
        else {
            ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(
                new[] {
                    Damage.Create(
                        attributes.PhysicalDamage * DMG_MUL_1,
                        DamageType.Physical,
                        attributes.PhysicalPenetration
                    ),
                    Damage.Create(
                        attributes.PhysicalDamage * BONUS_TRUE_DMG_MUL,
                        DamageType.True,
                        0
                    )
                });
            ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(
                new[] {
                    attributes.GetDamage(DamageType.Physical, false, scaledValues:new[]{(DMG_MUL_1, DamageType.Physical)}),
                    attributes.GetDamage(DamageType.True, false, scaledValues:new[]{(BONUS_TRUE_DMG_MUL, DamageType.Physical)})
                });
        }
        
        ((BattleHero)hero).Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_1);
    }
    
    void HeavySlash() {
        if (((BattleHero)hero).Target == null) return;
        
        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical, attributes.Crit(),
            scaledValues: new[] { (DMG_MUL_2, DamageType.Physical) })
        );
        
        ((BattleHero)hero).Target.GetAbility<HeroStatusEffects>().Airborne(AIRBORNE_TIME_2);
    }
}