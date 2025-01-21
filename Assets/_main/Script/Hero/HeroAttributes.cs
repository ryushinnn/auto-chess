using System;
using UnityEngine;

public class HeroAttributes : HeroAbility {
    [SerializeField] ProgressBar healthBar;
    [SerializeField] ProgressBar energyBar;
    [SerializeField] Canvas canvas;

    public bool IsAlive => isAlive;

    float hp;
    float maxHp;
    float armor;
    float resistance;
    float energyRegenPerHit;
    bool isAlive;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        healthBar.UpdateAmount(1, true);
        energyBar.UpdateAmount(0, true);
        hp = hero.Trait.maxHp;
        maxHp = hero.Trait.maxHp;
        armor = hero.Trait.armor;
        resistance = hero.Trait.resistance;
        energyRegenPerHit = hero.Trait.energyRegenPerHit;
        isAlive = true;
    }

    public void UpdateEnergyBar(float amount) {
        energyBar.UpdateAmount(amount);
    }

    public float TakeDamage(float damage, DamageType type, float penetration) {
        var dmgReduction = type switch {
            DamageType.Physical => Mathf.Min(armor * (1-penetration), HeroTrait.MAX_DMG_REDUCTION * damage),
            DamageType.Magical => Mathf.Min(resistance * (1-penetration), HeroTrait.MAX_DMG_REDUCTION * damage),
            DamageType.Pure => 0
        };
        
        damage -= dmgReduction;
        hp -= damage;
        healthBar.UpdateAmount(hp / maxHp);
        if (hp > 0) {
            hero.GetAbility<HeroSkill>().RegenEnergy(energyRegenPerHit);
        }
        else {
            Die();
        }

        return damage;
    }

    public void Heal(float amount) {
        if (!isAlive) return;
        
        if (hero.GetAbility<HeroStatusEffects>().IsAntiHeal) {
            amount *= HeroTrait.HEAL_UPON_ANTI_HEALTH;
        }

        hp = Mathf.Min(hp + amount, maxHp);
        healthBar.UpdateAmount(hp / maxHp);
    }

    void Die() {
        isAlive = false;
        hero.Mecanim.Death();
        hero.Mecanim.InterruptAttack();
        hero.Mecanim.InterruptSkill();
        canvas.enabled = false;
    }
}