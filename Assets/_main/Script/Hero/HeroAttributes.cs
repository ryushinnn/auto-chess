using System;
using UnityEngine;

public class HeroAttributes : HeroAbility {
    [SerializeField] ProgressBar healthBar;
    [SerializeField] ProgressBar energyBar;
    [SerializeField] Canvas canvas;

    public bool IsAlive => isAlive;
    public float Energy => energy;
    public int AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public float PhysicalDamage => physicalDamage;
    public float MagicalPower => magicalPower;
    public float PhysicalPenetration => physicalPenetration;
    public float MagicalPenetration => magicalPenetration;
    
    public float LifeSteal => lifeSteal;
    public float CriticalChance => criticalChance;
    public float CriticalDamage => criticalDamage;
    public float MovementSpeed => movementSpeed;

    bool isAlive;
    float hp;
    float energy;
    float armor;
    float resistance;
    int attackRange;
    float attackCooldown;
    float physicalDamage;
    float magicalPower;
    float physicalPenetration;
    float magicalPenetration;
    float lifeSteal;
    float criticalChance;
    float criticalDamage;
    float movementSpeed;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        isAlive = true;
        hp = hero.Trait.maxHp;
        energy = 0;
        healthBar.UpdateAmount(hp / this.hero.Trait.maxHp, true);
        energyBar.UpdateAmount(0, true);
        armor = hero.Trait.armor;
        resistance = hero.Trait.resistance;
        attackRange = this.hero.Trait.attackRange;
        attackCooldown = 1 / this.hero.Trait.attackSpeed;
        physicalDamage = this.hero.Trait.physicalDamage;
        magicalPower = this.hero.Trait.magicalPower;
        physicalPenetration = this.hero.Trait.physicalPenetration;
        magicalPenetration = this.hero.Trait.magicalPenetration;
        lifeSteal = this.hero.Trait.lifeSteal;
        criticalChance = this.hero.Trait.criticalChance;
        criticalDamage = this.hero.Trait.criticalDamage;
        movementSpeed = this.hero.Trait.movementSpeed;
    }

    public float TakeDamage(float damage, DamageType type, float penetration) {
        var dmgReduction = type switch {
            DamageType.Physical => Mathf.Min(armor * (1-penetration), HeroTrait.MAX_DMG_REDUCTION * damage),
            DamageType.Magical => Mathf.Min(resistance * (1-penetration), HeroTrait.MAX_DMG_REDUCTION * damage),
            DamageType.Pure => 0
        };
        
        damage -= dmgReduction;
        hp -= damage;
        healthBar.UpdateAmount(hp / hero.Trait.maxHp);
        if (hp > 0) {
            RegenEnergy(hero.Trait.energyRegenPerHit);
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

        hp = Mathf.Min(hp + amount, hero.Trait.maxHp);
        healthBar.UpdateAmount(hp / hero.Trait.maxHp);
    }

    public void RegenEnergy(float amount) {
        energy = Mathf.Min(energy + amount, HeroTrait.MAX_ENERGY);
        energyBar.UpdateAmount(energy / HeroTrait.MAX_ENERGY);
    }

    public void UseAllEnergy() {
        energy = 0;
        energyBar.UpdateAmount(0);
    }

    void Die() {
        isAlive = false;
        hero.Mecanim.Death();
        hero.Mecanim.InterruptAttack();
        hero.Mecanim.InterruptSkill();
        canvas.enabled = false;
    }
}