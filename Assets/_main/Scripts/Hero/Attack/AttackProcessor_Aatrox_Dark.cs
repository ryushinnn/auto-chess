using System;
using UnityEngine;

/// <summary>
/// don danh gay st 2 lan, moi lan gay 30-50% st vat ly
/// dua theo mau da mat cua muc tieu
/// don danh dau tien ko the chi mang
/// don danh thu 2 chac chan chi mang
/// </summary>
public class AttackProcessor_Aatrox_Dark : AttackProcessor {
    const float MIN_DMG_MUL = 0.5f;
    const float MAX_DMG_MUL = 1f;
    
    public AttackProcessor_Aatrox_Dark(Hero hero) : base(hero) { }

    public override void Execute() {
        CalculateDamage(out var damage);
        hero.Mecanim.Attack(new Action[] {
            () => {
                if (hero.Target == null) return;
                
                var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
                    Damage.Create(
                        damage.value * Mathf.Lerp(MIN_DMG_MUL, MAX_DMG_MUL, 1 - hero.Target.GetAbility<HeroAttributes>().Hp / hero.Target.GetAbility<HeroAttributes>().MaxHp),
                        damage.type,
                        damage.penetration
                    ));
                var heal = outputDamage * attributes.LifeSteal;
                if (heal > 0) {
                    attributes.Heal(heal);
                }
            },
            () => {
                if (hero.Target == null) return;
                
                var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(
                    Damage.Create(
                        damage.value * attributes.CriticalDamage * Mathf.Lerp(MIN_DMG_MUL, MAX_DMG_MUL, 1 - hero.Target.GetAbility<HeroAttributes>().Hp / hero.Target.GetAbility<HeroAttributes>().MaxHp),
                        damage.type,
                        damage.penetration,
                        true
                    ));
                var heal = outputDamage * attributes.LifeSteal;
                if (heal > 0) {
                    attributes.Heal(heal);
                }
                attributes.RegenEnergy(hero.Trait.energyRegenPerAttack);
            }
        });
    }

    protected override void CalculateDamage(out Damage damage) {
        damage = new Damage();
        damage.value = attributes.PhysicalDamage + attributes.MagicalDamage;
        if (attributes.PhysicalDamage > attributes.MagicalDamage) {
            damage.type = DamageType.Physical;
            damage.penetration = attributes.PhysicalPenetration;
        }
        else if (attributes.PhysicalDamage < attributes.MagicalDamage) {
            damage.type = DamageType.Magical;
            damage.penetration = attributes.MagicalPenetration;
        }
        else {
            if (attributes.PhysicalPenetration > attributes.MagicalPenetration) {
                damage.type = DamageType.Physical;
                damage.penetration = attributes.PhysicalPenetration;
            }
            else {
                damage.type = DamageType.Magical;
                damage.penetration = attributes.MagicalPenetration;
            }
        }
    }
}