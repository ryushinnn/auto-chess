using UnityEngine;

public class HeroAttack : HeroAbility {
    float currentAttackCooldown;

    public override void Process() {
        if (currentAttackCooldown > 0) {
            currentAttackCooldown -= Time.deltaTime;
        }
    }

    public bool Attack() {
        if (currentAttackCooldown > 0 
            || hero.GetAbility<HeroSkill>().IsUsingSkill
            || hero.GetAbility<HeroStatusEffects>().IsAirborne
            || hero.GetAbility<HeroStatusEffects>().IsStun) return false;
        
        hero.GetAbility<HeroRotation>().Rotate(hero.Target.transform.position - hero.transform.position);
        CalculateDamage(out var dmg, out var type, out var pen);
        hero.Mecanim.Attack(() => {
            var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg, type, pen);
            hero.GetAbility<HeroAttributes>().Heal(outputDamage * hero.GetAbility<HeroAttributes>().LifeSteal);
            hero.GetAbility<HeroAttributes>().RegenEnergy(hero.Trait.energyRegenPerAttack);
        });
        currentAttackCooldown = 1 / hero.GetAbility<HeroAttributes>().AttackSpeed;
        return true;
    }

    public void Interrupt() {
        hero.Mecanim.InterruptAttack();
        currentAttackCooldown = 1 / hero.GetAbility<HeroAttributes>().AttackSpeed;
    }

    public void RefreshAttackCooldown() {
        currentAttackCooldown = Mathf.Min(currentAttackCooldown, 1 / hero.GetAbility<HeroAttributes>().AttackSpeed);
    }

    void CalculateDamage(out float damage, out DamageType type, out float penetration) {
        if (hero.GetAbility<HeroAttributes>().PhysicalDamage > hero.GetAbility<HeroAttributes>().MagicalPower) {
            damage = hero.GetAbility<HeroAttributes>().PhysicalDamage;
            type = DamageType.Physical;
            penetration = hero.GetAbility<HeroAttributes>().PhysicalPenetration;
        }
        else if (hero.GetAbility<HeroAttributes>().PhysicalDamage < hero.GetAbility<HeroAttributes>().MagicalPower) {
            damage = hero.GetAbility<HeroAttributes>().MagicalPower;
            type = DamageType.Magical;
            penetration = hero.GetAbility<HeroAttributes>().MagicalPenetration;
        }
        else {
            if (hero.GetAbility<HeroAttributes>().PhysicalPenetration > hero.GetAbility<HeroAttributes>().MagicalPenetration) {
                damage = hero.GetAbility<HeroAttributes>().PhysicalDamage;
                type = DamageType.Physical;
                penetration = hero.GetAbility<HeroAttributes>().PhysicalPenetration;
            }
            else {
                damage = hero.GetAbility<HeroAttributes>().MagicalPower;
                type = DamageType.Magical;
                penetration = hero.GetAbility<HeroAttributes>().MagicalPenetration;
            }
        }

        if (Random.value < hero.GetAbility<HeroAttributes>().CriticalChance) {
            damage *= hero.GetAbility<HeroAttributes>().CriticalDamage;
        }
    }
}

public enum DamageType {
    Physical,
    Magical,
    Pure
}