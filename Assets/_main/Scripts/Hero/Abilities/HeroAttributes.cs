using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroAttributes : HeroAbility {
    HeroStatusEffects effects;
    HeroMovement movement;
    HeroAttack attack;
    HeroMark mark;
    
    [SerializeField] ProgressBar healthBar;
    [SerializeField] ProgressBar energyBar;
    [SerializeField] Canvas canvas;
    [SerializeField] Transform hpTextParent;

    public bool IsAlive => isAlive;
    public float MaxHp => maxHp;
    public float Hp => hp;
    public float Energy => energy;
    public float Armor => armor;
    public float Resistance => resistance;
    public float AttackSpeed => attackSpeed;
    public float PhysicalDamage => physicalDamage;
    public float MagicalDamage => magicalDamage;
    public float MovementSpeed => movementSpeed;
    public float CriticalChance => criticalChance;
    public float CriticalDamage => criticalDamage;
    public float EnergyRegenEfficient => energyRegenEfficient;
    public float PhysicalPenetration => physicalPenetration;
    public float MagicalPenetration => magicalPenetration;
    public float LifeSteal => lifeSteal;
    public float Tenacity => tenacity;

    [SerializeField, ReadOnly] bool isAlive;
    [SerializeField, ReadOnly] float maxHp;
    [SerializeField, ReadOnly] float hp;
    [SerializeField, ReadOnly] float energy;
    [SerializeField, ReadOnly] float armor;
    [SerializeField, ReadOnly] float resistance;
    [SerializeField, ReadOnly] float attackSpeed;
    [SerializeField, ReadOnly] float physicalDamage;
    [SerializeField, ReadOnly] float magicalDamage;
    [SerializeField, ReadOnly] float movementSpeed;
    [SerializeField, ReadOnly] float criticalChance;
    [SerializeField, ReadOnly] float criticalDamage;
    [SerializeField, ReadOnly] float energyRegenEfficient;
    [SerializeField, ReadOnly] float physicalPenetration;
    [SerializeField, ReadOnly] float magicalPenetration;
    [SerializeField, ReadOnly] float lifeSteal;
    [SerializeField, ReadOnly] float tenacity;

    [SerializeField, ReadOnly] List<AttributeModifierGroup> modifierGroups = new();
    [SerializeField, ReadOnly] List<DamageOverTime> damageOverTimes = new();
    [SerializeField, ReadOnly] List<HealOverTime> healOverTimes = new();

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        maxHp = this.hero.Trait.maxHp;
        armor = this.hero.Trait.armor;
        resistance = this.hero.Trait.resistance;
        attackSpeed = this.hero.Trait.attackSpeed;
        physicalDamage = this.hero.Trait.physicalDamage;
        magicalDamage = this.hero.Trait.magicalDamage;
        movementSpeed = this.hero.Trait.movementSpeed;
        criticalChance = this.hero.Trait.criticalChance;
        criticalDamage = this.hero.Trait.criticalDamage;
        energyRegenEfficient = this.hero.Trait.energyRegenEfficient;
        physicalPenetration = this.hero.Trait.physicalPenetration;
        magicalPenetration = this.hero.Trait.magicalPenetration;
        lifeSteal = this.hero.Trait.lifeSteal;
        tenacity = this.hero.Trait.tenacity;
    }

    public override void ResetAll() {
        isAlive = true;
        hp = maxHp;
        energy = 0;
        healthBar.UpdateAmount(1, true);
        energyBar.UpdateAmount(0, true);
    }

    public override void Process() {
        ProcessAttributeModifiers();
        ProcessDamageOverTimes();
        ProcessHealOverTimes();
    }

    protected override void FindReferences() {
        effects = hero.GetAbility<HeroStatusEffects>();
        movement = hero.GetAbility<HeroMovement>();
        attack = hero.GetAbility<HeroAttack>();
        mark = hero.GetAbility<HeroMark>();
    }

    public float TakeDamage(Damage damage, bool regenEnergy = true) {
        return TakeDamage(damage, regenEnergy, true).value;
    }
    
    public float TakeDamage(Damage[] damages, bool regenEnergy = true) {
        var outputDmgs = new List<Damage>();
        foreach (var dmg in damages) {
            outputDmgs.Add(TakeDamage(dmg, false, false));
        }

        if (hp > 0 && regenEnergy) {
            RegenEnergy(hero.Trait.energyRegenPerHit);
        }
        HpTextSpawner.Instance.SpawnHpTextAsDamage(hpTextParent, outputDmgs);
        return outputDmgs.Sum(x=>x.value);
    }

    Damage TakeDamage(Damage damage, bool regenEnergy, bool showText) {
        var dmgReduction = damage.type switch {
            DamageType.Physical => Mathf.Min(armor * (1-damage.penetration), HeroTrait.MAX_DMG_REDUCTION * damage.value),
            DamageType.Magical => Mathf.Min(resistance * (1-damage.penetration), HeroTrait.MAX_DMG_REDUCTION * damage.value),
            DamageType.True => 0
        };
        
        damage.value = Mathf.Max(damage.value - dmgReduction, 1);
        hp -= damage.value;
        healthBar.UpdateAmount(hp / maxHp);
        if (hp > 0) {
            if (regenEnergy) {
                RegenEnergy(hero.Trait.energyRegenPerHit);
            }
        }
        else {
            Die();
        }

        if (showText) {
            HpTextSpawner.Instance.SpawnHpTextAsDamage(hpTextParent,damage);
        }
        return damage;
    }

    public void Heal(float amount) {
        if (!isAlive) return;
        
        if (effects.IsAntiHeal) {
            amount *= HeroTrait.HEAL_UPON_ANTI_HEALTH;
        }

        hp = Mathf.Min(hp + amount, maxHp);
        healthBar.UpdateAmount(hp / maxHp);
        
        HpTextSpawner.Instance.SpawnHpTextAsHeal(hpTextParent,amount);
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

    public void AddDamageOverTime(DamageOverTime dot) {
        if (dot.overwrite) {
            var existDot = damageOverTimes.Find(x => x.key == dot.key);
            damageOverTimes.Remove(existDot);
            if (existDot.mark != null) {
                mark.RemoveMark(existDot.mark.id);
            }
        }
        damageOverTimes.Add(dot);
        if (dot.mark != null) {
            mark.AddMark(dot.mark);
        }
    }
    
    public void AddHealOverTime(HealOverTime hot) {
        if (hot.overwrite) {
            var existHot = healOverTimes.Find(x => x.key == hot.key);
            healOverTimes.Remove(existHot);
            mark.RemoveMark(existHot.mark.id);
        }
        healOverTimes.Add(hot);
        mark.AddMark(hot.mark);
    }

    void ProcessAttributeModifiers() {
        var hasChange = false;
        for (int i = modifierGroups.Count - 1; i >= 0; i--) {
            var key = modifierGroups[i].key;
            var modifiers = modifierGroups[i].modifiers;
            for (int j = modifiers.Count - 1; j >= 0; j--) {
                if (modifiers[j].permanent) continue;
                modifiers[j].duration -= Time.deltaTime;
                if (modifiers[j].duration <= 0) {
                    modifiers[j].onRemove?.Invoke();
                    modifiers.RemoveAt(j);
                    hasChange = true;
                }
            }

            if (modifiers.Count == 0) {
                modifierGroups.RemoveAt(i);
            }

            if (hasChange) {
                RecalculateAttributes(key);
            }
        }
    }

    void ProcessDamageOverTimes() {
        for (int i = damageOverTimes.Count - 1; i >= 0; i--) {
            var dot = damageOverTimes[i];
            if (dot.timer <= 0) {
                TakeDamage(dot.damagePerStack, false);
                if (--dot.stack <= 0) {
                    damageOverTimes.Remove(dot);
                    if (dot.mark != null) {
                        mark.RemoveMark(dot.mark.id);
                    }
                }
                else {
                    dot.timer = dot.interval;
                }
            }
            else {
                dot.timer -= Time.deltaTime;
            }
        }
    }
    
    void ProcessHealOverTimes() {
        for (int i = healOverTimes.Count - 1; i >= 0; i--) {
            var hot = healOverTimes[i];
            if (hot.timer <= 0) {
                Heal(hot.healPerStack);
                if (--hot.stack <= 0) {
                    healOverTimes.Remove(hot);
                    if (hot.mark != null) {
                        mark.RemoveMark(hot.mark.id);
                    }
                }
                else {
                    hot.timer = hot.interval;
                }
            }
            else {
                hot.timer -= Time.deltaTime;
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
            case AttributeModifierKey.MaxHp:
                var lastMaxHp = maxHp;
                maxHp = hero.Trait.maxHp;
                modifiers?.ForEach(x => {
                    maxHp += (x.type == ModifierType.FixedValue ? x.value : maxHp * x.value);
                });
                if (maxHp > lastMaxHp) {
                    hp += maxHp - lastMaxHp;
                }
                else {
                    hp = Mathf.Min(hp, maxHp);
                }
                healthBar.UpdateAmount(hp / maxHp);
                break;
            
            case AttributeModifierKey.PhysicalDamage:
                physicalDamage = hero.Trait.physicalDamage;
                modifiers?.ForEach(x => {
                    physicalDamage = Mathf.Max(physicalDamage + (x.type == ModifierType.FixedValue ? x.value : physicalDamage * x.value), HeroTrait.MIN_DAMAGE);
                });
                break;
            
            case AttributeModifierKey.MagicalDamage:
                magicalDamage = hero.Trait.magicalDamage;
                modifiers?.ForEach(x => {
                    magicalDamage = Mathf.Max(magicalDamage + (x.type == ModifierType.FixedValue ? x.value : magicalDamage * x.value), HeroTrait.MIN_DAMAGE);
                });
                break;
            
            case AttributeModifierKey.Armor:
                armor = hero.Trait.armor;
                modifiers?.ForEach(x => {
                    armor = Mathf.Max(armor + (x.type == ModifierType.FixedValue ? x.value : armor * x.value), HeroTrait.MIN_ARMOR_AND_RESISTANCE);
                });
                break;
            
            case AttributeModifierKey.Resistance:
                resistance = hero.Trait.resistance;
                modifiers?.ForEach(x => {
                    resistance = Mathf.Max(resistance + (x.type == ModifierType.FixedValue ? x.value : resistance * x.value), HeroTrait.MIN_ARMOR_AND_RESISTANCE);
                });
                break;
            
            case AttributeModifierKey.AttackSpeed:
                attackSpeed = hero.Trait.attackSpeed;
                modifiers?.ForEach(x => {
                    attackSpeed = Mathf.Max(attackSpeed + (x.type == ModifierType.FixedValue ? x.value : attackSpeed * x.value), HeroTrait.MIN_ATTACK_SPEED);
                });
                attack.RefreshAttackCooldown();
                hero.Mecanim.ModifyAttackTime(attackSpeed);
                break;
            
            case AttributeModifierKey.CriticalChance:
                criticalChance = hero.Trait.criticalChance;
                modifiers?.ForEach(x => {
                    criticalChance += (x.type == ModifierType.FixedValue ? x.value : criticalChance * x.value);
                });
                criticalChance = Mathf.Min(criticalChance, HeroTrait.MAX_CRITICAL_CHANCE);
                break;
            
            case AttributeModifierKey.CriticalDamage:
                criticalDamage = hero.Trait.criticalDamage;
                modifiers?.ForEach(x => {
                    criticalDamage += (x.type == ModifierType.FixedValue ? x.value : criticalDamage * x.value);
                });
                break;
            
            case AttributeModifierKey.EnergyRegenEfficient:
                energyRegenEfficient = hero.Trait.energyRegenEfficient;
                modifiers?.ForEach(x => {
                    energyRegenEfficient += (x.type == ModifierType.FixedValue ? x.value : energyRegenEfficient * x.value);
                });
                break;
            
            case AttributeModifierKey.PhysicalPenetration:
                physicalPenetration = hero.Trait.physicalPenetration;
                modifiers?.ForEach(x => {
                    physicalPenetration = Mathf.Min(physicalPenetration + (x.type == ModifierType.FixedValue ? x.value : physicalPenetration * x.value), HeroTrait.MAX_PENETRATION);
                });
                break;
            
            case AttributeModifierKey.MagicalPenetration:
                magicalPenetration = hero.Trait.magicalPenetration;
                modifiers?.ForEach(x => {
                    magicalPenetration = Mathf.Min(magicalPenetration + (x.type == ModifierType.FixedValue ? x.value : magicalPenetration * x.value), HeroTrait.MAX_PENETRATION);
                });
                break;
            
            case AttributeModifierKey.LifeSteal:
                lifeSteal = hero.Trait.lifeSteal;
                modifiers?.ForEach(x => {
                    lifeSteal = Mathf.Min(lifeSteal + (x.type == ModifierType.FixedValue ? x.value : lifeSteal * x.value), HeroTrait.MAX_LIFE_STEAL);
                });
                break;
            
            case AttributeModifierKey.Tenacity:
                tenacity = hero.Trait.tenacity;
                modifiers?.ForEach(x => {
                    tenacity = Mathf.Min(tenacity + (x.type == ModifierType.FixedValue ? x.value : tenacity * x.value), HeroTrait.MAX_TENACITY);
                });
                break;
        }
    }

    void Die() {
        isAlive = false;
        movement.StopMove(true);
        hero.Mecanim.Death();
        hero.Mecanim.InterruptAttack();
        hero.Mecanim.InterruptSkill();
        canvas.enabled = false;
        hero.name = "(DEAD) " + hero.name;
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

    [Button]
    void dev_dot(float dmg, int stack, float interval) {
        // AddDamageOverTime(DamageOverTime.Create("dot", dmg, DamageType.True, 0, stack, interval));
        
    }
    
    [Button]
    void dev_hot(float heal, int stack, float interval) {
        AddHealOverTime(HealOverTime.Create("hot", heal, (int)stack, interval));
        
    }
}

public class Damage {
    public float value;
    public DamageType type;
    public float penetration;
    public bool crit;

    public static Damage Create(float value, DamageType type, float penetration, bool crit = false) {
        return new Damage {
            value = value,
            type = type,
            penetration = penetration,
            crit = crit
        };
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
    [StringDropdown(typeof(AttributeModifierKey))]
    public string key;
    public string id;
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
    
    public static AttributeModifier Create(AttributeModifier modifier) {
        return new AttributeModifier {
            id = Guid.NewGuid().ToString(),
            key = modifier.key,
            value = modifier.value,
            type = modifier.type,
            duration = modifier.duration,
            permanent = modifier.permanent,
            onRemove = modifier.onRemove
        };
    }
}

public enum ModifierType {
    FixedValue,
    Percentage
}

public static class AttributeModifierKey {
    public const string MaxHp = "hp";
    public const string PhysicalDamage = "pdmg";
    public const string MagicalDamage = "mdmg";
    public const string Armor = "armor";
    public const string Resistance = "resistance";
    public const string AttackSpeed = "aspd";
    public const string CriticalChance = "crit";
    public const string CriticalDamage = "cdmg";
    public const string EnergyRegenEfficient = "energy";
    public const string PhysicalPenetration = "ppen";
    public const string MagicalPenetration = "mpen";
    public const string LifeSteal = "lifesteal";
    public const string Tenacity = "tenacity";
}

[Serializable]
public class DamageOverTime {
    public string key;
    public string id;
    public Damage damagePerStack;
    public int stack;
    public float interval;
    public bool overwrite;
    public Mark mark;
    public float timer;
    
    public static DamageOverTime Create(string key, Damage dmgPerStack, int stack, float interval, bool applyDmgInstantly = true, bool overwrite = true, bool createMark = false) {
        return new DamageOverTime {
            id = Guid.NewGuid().ToString(),
            key = key,
            damagePerStack = dmgPerStack,
            stack = stack,
            interval = interval,
            overwrite = overwrite,
            mark = createMark ? Mark.Create(key) : null, 
            timer = applyDmgInstantly ? 0 : interval,
        };
    }
}

[Serializable]
public class HealOverTime {
    public string key;
    public string id;
    public float healPerStack;
    public int stack;
    public float interval;
    public bool overwrite;
    public Mark mark;
    public float timer;
    
    public static HealOverTime Create(string key, float healPerStack, int stack, float interval, bool overwrite = true, bool createMark = false) {
        return new HealOverTime {
            id = Guid.NewGuid().ToString(),
            key = key,
            healPerStack = healPerStack,
            stack = stack,
            interval = interval,
            overwrite = overwrite,
            mark = createMark ? Mark.Create(key) : null,
            timer = 0,
        };
    }
}