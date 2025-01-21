using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroAttributes : HeroAbility {
    [SerializeField] ProgressBar healthBar;
    [SerializeField] ProgressBar energyBar;
    [SerializeField] Canvas canvas;

    public bool IsAlive => isAlive;
    public float Energy => energy;
    public float AttackCooldown => attackCooldown;
    public float PhysicalDamage => physicalDamage;
    public float MagicalPower => magicalPower;
    public float PhysicalPenetration => physicalPenetration;
    public float MagicalPenetration => magicalPenetration;
    
    public float LifeSteal => lifeSteal;
    public float CriticalChance => criticalChance;
    public float CriticalDamage => criticalDamage;
    public float MovementSpeed => movementSpeed;

    [SerializeField, ReadOnly] bool isAlive;
    [SerializeField, ReadOnly] float hp;
    [SerializeField, ReadOnly] float energy;
    [SerializeField, ReadOnly] float armor;
    [SerializeField, ReadOnly] float resistance;
    [SerializeField, ReadOnly] float attackCooldown;
    [SerializeField, ReadOnly] float physicalDamage;
    [SerializeField, ReadOnly] float magicalPower;
    [SerializeField, ReadOnly] float physicalPenetration;
    [SerializeField, ReadOnly] float magicalPenetration;
    [SerializeField, ReadOnly] float lifeSteal;
    [SerializeField, ReadOnly] float criticalChance;
    [SerializeField, ReadOnly] float criticalDamage;
    [SerializeField, ReadOnly] float movementSpeed;

    [SerializeField, ReadOnly] List<AttributeModifier> modifiers = new();

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        isAlive = true;
        hp = hero.Trait.maxHp;
        energy = 0;
        healthBar.UpdateAmount(hp / this.hero.Trait.maxHp, true);
        energyBar.UpdateAmount(0, true);
        armor = hero.Trait.armor;
        resistance = hero.Trait.resistance;
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

    public override void Process() {
        for (int i = modifiers.Count - 1; i >= 0; i--) {
            modifiers[i].duration -= Time.deltaTime;
            if (modifiers[i].duration <= 0) {
                var key = modifiers[i].key;
                modifiers.RemoveAt(i);
                RecalculateAttributes(key);
            }
        }
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

    public void AddAttributeModifier(AttributeModifier modifier) {
        modifiers.Add(modifier);
        RecalculateAttributes(modifier.key);
    }

    void RecalculateAttributes(string key) {
        modifiers.Sort((a, b) => {
            var typeComparison = a.type == ModifierType.FixedValue ?
                (b.type == ModifierType.FixedValue ? 0 : -1) :
                (b.type == ModifierType.FixedValue ? 1 : 0);
            
            if (typeComparison != 0) return typeComparison;
            return b.value.CompareTo(a.value);
        });
        
        switch (key) {
            case "armor":
                armor = hero.Trait.armor;
                modifiers.ForEach(x => {
                    if (x.key == key) {
                        armor = Mathf.Max(armor + (x.type == ModifierType.FixedValue ? x.value : armor * x.value), HeroTrait.MIN_ARMOR_AND_RESISTANCE);
                    }
                });
                break;
        }
    }

    void Die() {
        isAlive = false;
        hero.GetAbility<HeroMovement>().StopMove();
        hero.Mecanim.Death();
        hero.Mecanim.InterruptAttack();
        hero.Mecanim.InterruptSkill();
        canvas.enabled = false;
    }

    [Button]
    void Dev_BuffArmorFixedValue(float val) {
        AddAttributeModifier(new AttributeModifier("armor",val,ModifierType.FixedValue,500));
    }
    
    [Button]
    void Dev_BuffArmorPercentage(float val) {
        AddAttributeModifier(new AttributeModifier("armor",val,ModifierType.Percentage,500));
    }

    [Button]
    void Dev_Reset() {
        modifiers.Clear();
        RecalculateAttributes("armor");
    }
}

[Serializable]
public class AttributeModifier {
    public string key;
    public float value;
    public ModifierType type;
    public float duration;

    public AttributeModifier(string key, float value, ModifierType type, float duration) {
        this.key = key;
        this.value = value;
        this.type = type;
        this.duration = duration;
    }
}

public enum ModifierType {
    FixedValue,
    Percentage
}