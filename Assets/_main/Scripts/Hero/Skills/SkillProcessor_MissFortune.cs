using System;

/// <summary>
/// ban lan luot 2 vien dan
/// moi vien gay st vat ly = 50%/80% st vat ly  + st phep = 50%/80% st vat ly
/// vien dau tien se hoi mau = 30% st gay ra
/// vien thu 2 se lam muc tieu giam 30% giap, khang phep trong 2.5s
/// </summary>
public class SkillProcessor_MissFortune : SkillProcessor {
    const float PHYSICAL_DMG_MUL_0 = 0.5f;
    const float MAGICAL_DMG_MUL_0 = 0.5f;
    const float PHYSICAL_DMG_MUL_1 = 0.8f;
    const float MAGICAL_DMG_MUL_1 = 0.8f;
    const float VAMP_MUL = 0.3f;
    const float REDUCE_DEFENSE_MUL = -0.3f;
    const float REDUCE_DEFENSE_DURATION = 2.5f;

    public SkillProcessor_MissFortune(Hero hero) : base(hero) {
        events = new Action[]{ShotLeft, ShotRight};
    }

    void ShotLeft() {
        if (hero.Target == null) return;

        var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            new[] {
                Damage.Create(attributes.PhysicalDamage * PHYSICAL_DMG_MUL_0, DamageType.Physical, attributes.PhysicalPenetration),
                Damage.Create(attributes.PhysicalDamage * MAGICAL_DMG_MUL_0, DamageType.Magical, attributes.MagicalPenetration),
            });
        
        attributes.Heal(outputDamage * VAMP_MUL);
    }

    void ShotRight() {
        if (hero.Target == null) return;

        hero.Target.GetAbility<HeroAttributes>().TakeDamage(
            new[] {
                Damage.Create(attributes.PhysicalDamage * PHYSICAL_DMG_MUL_1, DamageType.Physical, attributes.PhysicalPenetration),
                Damage.Create(attributes.PhysicalDamage * MAGICAL_DMG_MUL_1, DamageType.Magical, attributes.MagicalPenetration),
            });
        
        hero.Target.GetAbility<HeroAttributes>().AddAttributeModifier(AttributeModifier.Create(hero, AttributeModifierKey.Armor, REDUCE_DEFENSE_MUL, ModifierType.Percentage, REDUCE_DEFENSE_DURATION));
        hero.Target.GetAbility<HeroAttributes>().AddAttributeModifier(AttributeModifier.Create(hero, AttributeModifierKey.Resistance, REDUCE_DEFENSE_MUL, ModifierType.Percentage, REDUCE_DEFENSE_DURATION));
    }
}