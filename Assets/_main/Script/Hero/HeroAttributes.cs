using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroAttributes : HeroAbility {
    [SerializeField] ProgressBar healthBar;
    [SerializeField] ProgressBar energyBar;
    [SerializeField] Canvas canvas;

    public bool IsAlive => isAlive;
    public float Hp => hp;
    public float Energy => energy;
    public float Armor => armor;
    public float Resistance => resistance;
    public float AttackSpeed => attackSpeed;
    public float PhysicalDamage => physicalDamage;
    public float MagicalPower => magicalPower;
    public float MovementSpeed => movementSpeed;
    public float CriticalChance => criticalChance;
    public float CriticalDamage => criticalDamage;
    public float EnergyRegenEfficient => energyRegenEfficient;
    public float PhysicalPenetration => physicalPenetration;
    public float MagicalPenetration => magicalPenetration;
    public float LifeSteal => lifeSteal;
    public float Tenacity => tenacity;

    [SerializeField, ReadOnly] bool isAlive;
    [SerializeField, ReadOnly] float hp;
    [SerializeField, ReadOnly] float energy;
    [SerializeField, ReadOnly] float armor;
    [SerializeField, ReadOnly] float resistance;
    [SerializeField, ReadOnly] float attackSpeed;
    [SerializeField, ReadOnly] float physicalDamage;
    [SerializeField, ReadOnly] float magicalPower;
    [SerializeField, ReadOnly] float movementSpeed;
    [SerializeField, ReadOnly] float criticalChance;
    [SerializeField, ReadOnly] float criticalDamage;
    [SerializeField, ReadOnly] float energyRegenEfficient;
    [SerializeField, ReadOnly] float physicalPenetration;
    [SerializeField, ReadOnly] float magicalPenetration;
    [SerializeField, ReadOnly] float lifeSteal;
    [SerializeField, ReadOnly] float tenacity;

    [SerializeField, ReadOnly] List<AttributeModifierGroup> modifierGroups = new();

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        isAlive = true;
        hp = this.hero.Trait.maxHp;
        energy = 0;
        healthBar.UpdateAmount(hp / this.hero.Trait.maxHp, true);
        energyBar.UpdateAmount(0, true);
        armor = this.hero.Trait.armor;
        resistance = this.hero.Trait.resistance;
        attackSpeed = this.hero.Trait.attackSpeed;
        physicalDamage = this.hero.Trait.physicalDamage;
        magicalPower = this.hero.Trait.magicalPower;
        movementSpeed = this.hero.Trait.movementSpeed;
        criticalChance = this.hero.Trait.criticalChance;
        criticalDamage = this.hero.Trait.criticalDamage;
        energyRegenEfficient = this.hero.Trait.energyRegenEfficient;
        physicalPenetration = this.hero.Trait.physicalPenetration;
        magicalPenetration = this.hero.Trait.magicalPenetration;
        lifeSteal = this.hero.Trait.lifeSteal;
        tenacity = this.hero.Trait.tenacity;
    }

    public override void Process() {
        for (int i = modifierGroups.Count - 1; i >= 0; i--) {
            var key = modifierGroups[i].key;
            var modifiers = modifierGroups[i].modifiers;
            for (int j = modifiers.Count - 1; j >= 0; j--) {
                if (modifiers[j].permanent) continue;
                modifiers[j].duration -= Time.deltaTime;
                if (modifiers[j].duration <= 0) {
                    modifiers[j].onRemove?.Invoke();
                    modifiers.RemoveAt(j);
                }
            }

            if (modifiers.Count == 0) {
                modifierGroups.RemoveAt(i);
            }
            RecalculateAttributes(key);
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
        amount *= energyRegenEfficient;
        energy = Mathf.Min(energy + amount, HeroTrait.MAX_ENERGY);
        energyBar.UpdateAmount(energy / HeroTrait.MAX_ENERGY);
    }

    public void UseAllEnergy() {
        energy = 0;
        energyBar.UpdateAmount(0);
    }

    public void AddAttributeModifier(AttributeModifier modifier) {
        var group = modifierGroups.Find(x => x.key == modifier.key);
        if (group == null) {
            group = new AttributeModifierGroup(modifier.key);
            modifierGroups.Add(group);
        }
        group.modifiers.Add(modifier);
        RecalculateAttributes(modifier.key);
    }
    
    public void RemoveAttributeModifier(string id) {
        for (int i = modifierGroups.Count - 1; i >= 0; i--) {
            var key = modifierGroups[i].key;
            var modifier = modifierGroups[i].modifiers.Find(x => x.id == id);
            if (modifier != null) {
                modifier.onRemove?.Invoke();
                modifierGroups[i].modifiers.Remove(modifier);
                if (modifierGroups[i].modifiers.Count == 0) {
                    modifierGroups.RemoveAt(i);
                }
                RecalculateAttributes(key);
                return;
            }
        }
    }

    void RecalculateAttributes(string key) {
        var modifiers = modifierGroups.Find(x => x.key == key)?.modifiers;
        modifiers?.Sort((a, b) => {
            var typeComparison = a.type == ModifierType.FixedValue ?
                (b.type == ModifierType.FixedValue ? 0 : -1) :
                (b.type == ModifierType.FixedValue ? 1 : 0);
            
            if (typeComparison != 0) return typeComparison;
            return b.value.CompareTo(a.value);
        });
        
        switch (key) {
            case AttributeModifierKey.Armor:
                armor = hero.Trait.armor;
                modifiers?.ForEach(x => {
                    if (x.key == key) {
                        armor = Mathf.Max(armor + (x.type == ModifierType.FixedValue ? x.value : armor * x.value), HeroTrait.MIN_ARMOR_AND_RESISTANCE);
                    }
                });
                break;
            
            case AttributeModifierKey.AttackSpeed:
                attackSpeed = hero.Trait.attackSpeed;
                modifiers?.ForEach(x => {
                    if (x.key == key) {
                        attackSpeed = Mathf.Max(attackSpeed + (x.type == ModifierType.FixedValue ? x.value : attackSpeed * x.value), HeroTrait.MIN_ATTACK_SPEED);
                    }
                });
                hero.GetAbility<HeroAttack>().RefreshAttackCooldown();
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
    void Dev_FixedValue(float val) {
        AddAttributeModifier(AttributeModifier.Create(AttributeModifierKey.AttackSpeed,val,ModifierType.FixedValue,5));
    }
    
    [Button]
    void Dev_Percentage(float val) {
        AddAttributeModifier(AttributeModifier.Create(AttributeModifierKey.AttackSpeed,val,ModifierType.Percentage,5));
    }

    [Button]
    void Dev_Permanent() {
        AddAttributeModifier(AttributeModifier.Create(AttributeModifierKey.AttackSpeed,0.5f,ModifierType.Percentage));
    }
}

[Serializable]
public class AttributeModifierGroup {
    public string key;
    public List<AttributeModifier> modifiers;

    public AttributeModifierGroup(string key) {
        this.key = key;
        modifiers = new List<AttributeModifier>();
    }
}

[Serializable]
public class AttributeModifier {
    public string id;
    public string key;
    public float value;
    public ModifierType type;
    public float duration;
    public bool permanent;
    public Action onRemove;

    public static AttributeModifier Create(string key, float value, ModifierType type, float duration, Action onRemove = null) {
        return new AttributeModifier {
            id = Guid.NewGuid().ToString(),
            key = key,
            value = value,
            type = type,
            duration = duration,
            permanent = false,
            onRemove = onRemove
        };
    }

    public static AttributeModifier Create(string key, float value, ModifierType type, Action onRemove = null) {
        return new AttributeModifier {
            id = Guid.NewGuid().ToString(),
            key = key,
            value = value,
            type = type,
            duration = Mathf.Infinity,
            permanent = true,
            onRemove = onRemove
        };
    }
}

public enum ModifierType {
    FixedValue,
    Percentage
}

public static class AttributeModifierKey {
    public const string Armor = "armor";
    public const string AttackSpeed = "atk_spd";
}