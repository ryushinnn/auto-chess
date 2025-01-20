using System;
using UnityEngine;

public class HeroHealth : HeroAbility {
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

    public void TakeDamage(float damage, DamageType type, float penetration) {
        var dmgReduction = type switch {
            DamageType.Physical => Mathf.Clamp(armor - penetration, 0, HeroTrait.MAX_DMG_REDUCTION * damage),
            DamageType.Magical => Mathf.Clamp(resistance - penetration, 0, HeroTrait.MAX_DMG_REDUCTION * damage),
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
    }

    void Die() {
        isAlive = false;
        hero.Mecanim.Death();
        hero.Mecanim.DoNone();
        hero.SetNode(null);
        canvas.enabled = false;
    }
}