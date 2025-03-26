using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class HeroAttributes : HeroAbility {
    HeroStatusEffects effects;
    HeroMovement movement;
    HeroAttack attack;
    HeroMark mark;
    HeroSkill skill;
    
    [SerializeField] ProgressBar healthBar;
    [SerializeField] ProgressBar energyBar;
    [SerializeField] Canvas canvas;
    [SerializeField] Transform hpTextParent;
    
    public bool IsAlive => isAlive;
    public float MaxHp => maxHp;
    public float Hp => hp;
    public float HpPercentage => Mathf.Clamp01(hp / maxHp);
    public float HpLostPercentage => 1 - HpPercentage;
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

    [SerializeField, ReadOnly, TableList] List<AttributeModifierSet> modifierSets = new();
    [SerializeField, ReadOnly, TableList] List<AttributeModifierGroup> modifierGroups = new();
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
        skill = hero.GetAbility<HeroSkill>();
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
        
        //                              pre-mitigation dmg
        // post-mitigation dmg =   _______________________________
        //                                   armor/resistance
        //                         1 +   _________________________
        //                               DAMAGE_REDUCTION_CONSTANT
        
        var finalDmg = Damage.Create(
            Mathf.Max(damage.type switch {
                        DamageType.Physical => damage.value / (1 + Mathf.Max(HeroTrait.MIN_ARMOR_AND_RESISTANCE, armor - damage.penetration) / HeroTrait.DAMAGE_REDUCTION_CONSTANT),
                        DamageType.Magical => damage.value / (1 + Mathf.Max(HeroTrait.MIN_ARMOR_AND_RESISTANCE, resistance - damage.penetration) / HeroTrait.DAMAGE_REDUCTION_CONSTANT),
                        DamageType.True => damage.value
                    },
                HeroTrait.MIN_DAMAGE_DEALT), 
            damage.type, 
            damage.penetration, 
            damage.crit
        );
        
        hp -= finalDmg.value;
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
            HpTextSpawner.Instance.SpawnHpTextAsDamage(hpTextParent,finalDmg);
        }
        return finalDmg;
    }

    public void Heal(float amount) {
        if (!isAlive) return;

        if (0 < amount && amount < 1) {
            amount = 1;
        }
        
        if (effects.IsAntiHeal) {
            amount *= (1 - HeroTrait.ANTI_HEAL_EFFICIENCY);
        }

        hp = Mathf.Min(hp + amount, maxHp);
        healthBar.UpdateAmount(hp / maxHp);
        
        HpTextSpawner.Instance.SpawnHpTextAsHeal(hpTextParent,amount);
    }

    public void RegenEnergy(float amount) {
        if (!isAlive || skill.IsUsingSkill) return;
        
        amount *= energyRegenEfficient;
        energy = Mathf.Min(energy + amount, HeroTrait.MAX_ENERGY);
        energyBar.UpdateAmount(energy / HeroTrait.MAX_ENERGY);
    }

    public void UseAllEnergy() {
        energy = 0;
        energyBar.UpdateAmount(0);
    }

    public bool Crit() {
        return Random.value < criticalChance;
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
    
    public void AddAttributeModifier(AttributeModifierSet modifierSet) {
        var existSet = modifierSets.Find(x => x.SameAs(modifierSet));
        if (existSet != null) {
            foreach (var modifier in existSet.modifiers) {
                RemoveAttributeModifier(modifier);
            }
            if (existSet.mark != null) {
                mark.RemoveMark(existSet.mark);
            }
            modifierSets.Remove(existSet);
        }
        
        modifierSets.Add(modifierSet);
        foreach (var modifier in modifierSet.modifiers) {
            AddAttributeModifier(modifier);
        }

        if (modifierSet.mark != null) {
            mark.AddMark(modifierSet.mark);
        }
    }

    public void RemoveAttributeModifier(AttributeModifier modifier) {
        for (int i = modifierGroups.Count - 1; i >= 0; i--) {
            var key = modifierGroups[i].key;
            if (modifierGroups[i].modifiers.Contains(modifier)) {
                modifierGroups[i].modifiers.Remove(modifier);
                if (modifierGroups[i].modifiers.Count == 0) {
                    modifierGroups.RemoveAt(i);
                }
                RecalculateAttributes(key);
                return;
            }
        }
    }
    
    public void RemoveAttributeModifier(string effectKey) {
        for (int i = modifierSets.Count - 1; i >= 0; i--) {
            if (modifierSets[i].effectKey == effectKey) {
                foreach (var m in modifierSets[i].modifiers) {
                    RemoveAttributeModifier(m);
                }
                if (modifierSets[i].mark != null) {
                    mark.RemoveMark(modifierSets[i].mark);
                }
                modifierSets.RemoveAt(i);
                return;
            }
        }
    }
    
    void ProcessAttributeModifiers() {
        var changedKeys = new HashSet<string>();

        for (int i = modifierSets.Count - 1; i >= 0; i--) {
            modifierSets[i].duration -= Time.deltaTime;
            if (modifierSets[i].duration <= 0) {
                foreach (var m in modifierSets[i].modifiers) {
                    changedKeys.Add(m.key);
                    var group = modifierGroups.Find(x=>x.key == m.key);
                    group.modifiers.Remove(m);
                    if (group.modifiers.Count == 0) {
                        modifierGroups.Remove(group);
                    }
                }
                if (modifierSets[i].mark != null) {
                    mark.RemoveMark(modifierSets[i].mark);
                }
                modifierSets.RemoveAt(i);
            }
        }
        
        for (int i = modifierGroups.Count - 1; i >= 0; i--) {
            var key = modifierGroups[i].key;
            var modifiers = modifierGroups[i].modifiers;
            for (int j = modifiers.Count - 1; j >= 0; j--) {
                if (modifiers[j].permanent || modifiers[j].ControlledBySet()) continue;
                
                modifiers[j].duration -= Time.deltaTime;
                if (modifiers[j].duration <= 0) {
                    modifiers.RemoveAt(j);
                    changedKeys.Add(key);
                }
            }

            if (modifiers.Count == 0) {
                modifierGroups.RemoveAt(i);
            }
        }

        foreach (var key in changedKeys) {
            RecalculateAttributes(key);
        }
    }

    public void AddDamageOverTime(DamageOverTime dot) {
        for (int i = damageOverTimes.Count - 1; i >= 0; i--) {
            if (damageOverTimes[i].SameAs(dot)) {
                if (damageOverTimes[i].mark != null) {
                    mark.RemoveMark(damageOverTimes[i].mark);
                }
                damageOverTimes.RemoveAt(i);
            }
        }
        damageOverTimes.Add(dot);
        if (dot.mark != null) {
            mark.AddMark(dot.mark);
        }
    }
    
    void ProcessDamageOverTimes() {
        for (int i = damageOverTimes.Count - 1; i >= 0; i--) {
            var dot = damageOverTimes[i];
            if (dot.timer <= 0) {
                TakeDamage(dot.damage, false);
                if (--dot.times <= 0) {
                    damageOverTimes.Remove(dot);
                    if (dot.mark != null) {
                        mark.RemoveMark(dot.mark);
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
    
    public void AddHealOverTime(HealOverTime hot) {
        for (int i = healOverTimes.Count - 1; i >= 0; i--) {
            if (healOverTimes[i].SameAs(hot)) {
                if (healOverTimes[i].mark != null) {
                    mark.RemoveMark(healOverTimes[i].mark);
                }
                healOverTimes.RemoveAt(i);
            }
        }
        healOverTimes.Add(hot);
        if (hot.mark != null) {
            mark.AddMark(hot.mark);
        }
    }
    
    void ProcessHealOverTimes() {
        for (int i = healOverTimes.Count - 1; i >= 0; i--) {
            var hot = healOverTimes[i];
            if (hot.timer <= 0) {
                Heal(hot.amount);
                if (--hot.times <= 0) {
                    healOverTimes.Remove(hot);
                    if (hot.mark != null) {
                        mark.RemoveMark(hot.mark);
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
            var typeComparison = a.type == AttributeModifier.Type.FixedValue ?
                (b.type == AttributeModifier.Type.FixedValue ? 0 : -1) :
                (b.type == AttributeModifier.Type.FixedValue ? 1 : 0);
            
            if (typeComparison != 0) return typeComparison;
            return b.value.CompareTo(a.value);
        });
        
        switch (key) {
            case AttributeModifierKey.MaxHp:
                var lastMaxHp = maxHp;
                maxHp = hero.Trait.maxHp;
                modifiers?.ForEach(x => {
                    maxHp += (x.type == AttributeModifier.Type.FixedValue ? x.value : maxHp * x.value);
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
                    physicalDamage = Mathf.Max(physicalDamage + (x.type == AttributeModifier.Type.FixedValue ? x.value : physicalDamage * x.value), HeroTrait.MIN_DAMAGE);
                });
                break;
            
            case AttributeModifierKey.MagicalDamage:
                magicalDamage = hero.Trait.magicalDamage;
                modifiers?.ForEach(x => {
                    magicalDamage = Mathf.Max(magicalDamage + (x.type == AttributeModifier.Type.FixedValue ? x.value : magicalDamage * x.value), HeroTrait.MIN_DAMAGE);
                });
                break;
            
            case AttributeModifierKey.Armor:
                armor = hero.Trait.armor;
                modifiers?.ForEach(x => {
                    armor = Mathf.Max(armor + (x.type == AttributeModifier.Type.FixedValue ? x.value : armor * x.value), HeroTrait.MIN_ARMOR_AND_RESISTANCE);
                });
                break;
            
            case AttributeModifierKey.Resistance:
                resistance = hero.Trait.resistance;
                modifiers?.ForEach(x => {
                    resistance = Mathf.Max(resistance + (x.type == AttributeModifier.Type.FixedValue ? x.value : resistance * x.value), HeroTrait.MIN_ARMOR_AND_RESISTANCE);
                });
                break;
            
            case AttributeModifierKey.AttackSpeed:
                attackSpeed = hero.Trait.attackSpeed;
                modifiers?.ForEach(x => {
                    attackSpeed = Mathf.Max(attackSpeed + (x.type == AttributeModifier.Type.FixedValue ? x.value : attackSpeed * x.value), HeroTrait.MIN_ATTACK_SPEED);
                });
                attack.RefreshAttackCooldown();
                hero.Mecanim.ModifyAttackTime(attackSpeed);
                break;
            
            case AttributeModifierKey.CriticalChance:
                criticalChance = hero.Trait.criticalChance;
                modifiers?.ForEach(x => {
                    criticalChance += (x.type == AttributeModifier.Type.FixedValue ? x.value : criticalChance * x.value);
                });
                criticalChance = Mathf.Min(criticalChance, HeroTrait.MAX_CRITICAL_CHANCE);
                break;
            
            case AttributeModifierKey.CriticalDamage:
                criticalDamage = hero.Trait.criticalDamage;
                modifiers?.ForEach(x => {
                    criticalDamage += (x.type == AttributeModifier.Type.FixedValue ? x.value : criticalDamage * x.value);
                });
                break;
            
            case AttributeModifierKey.EnergyRegenEfficient:
                energyRegenEfficient = hero.Trait.energyRegenEfficient;
                modifiers?.ForEach(x => {
                    energyRegenEfficient += (x.type == AttributeModifier.Type.FixedValue ? x.value : energyRegenEfficient * x.value);
                });
                break;
            
            case AttributeModifierKey.PhysicalPenetration:
                physicalPenetration = hero.Trait.physicalPenetration;
                modifiers?.ForEach(x => {
                    physicalPenetration = Mathf.Min(physicalPenetration + (x.type == AttributeModifier.Type.FixedValue ? x.value : physicalPenetration * x.value), HeroTrait.MAX_PENETRATION);
                });
                break;
            
            case AttributeModifierKey.MagicalPenetration:
                magicalPenetration = hero.Trait.magicalPenetration;
                modifiers?.ForEach(x => {
                    magicalPenetration = Mathf.Min(magicalPenetration + (x.type == AttributeModifier.Type.FixedValue ? x.value : magicalPenetration * x.value), HeroTrait.MAX_PENETRATION);
                });
                break;
            
            case AttributeModifierKey.LifeSteal:
                lifeSteal = hero.Trait.lifeSteal;
                modifiers?.ForEach(x => {
                    lifeSteal = Mathf.Min(lifeSteal + (x.type == AttributeModifier.Type.FixedValue ? x.value : lifeSteal * x.value), HeroTrait.MAX_LIFE_STEAL);
                });
                break;
            
            case AttributeModifierKey.Tenacity:
                tenacity = hero.Trait.tenacity;
                modifiers?.ForEach(x => {
                    tenacity = Mathf.Min(tenacity + (x.type == AttributeModifier.Type.FixedValue ? x.value : tenacity * x.value), HeroTrait.MAX_TENACITY);
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
    void dev_addAttSet() {
        AddAttributeModifier(AttributeModifierSet.Create(
            hero,
            "test_effect",
            10,
            new [] {
                (AttributeModifierKey.Armor, 0.3f, AttributeModifier.Type.Percentage),
                (AttributeModifierKey.Resistance, 0.3f, AttributeModifier.Type.Percentage),
            }));
    }

    [Button]
    void dev_addAtt() {
        AddAttributeModifier(AttributeModifier.Create(
            hero,
            AttributeModifierKey.Armor,
            10,
            AttributeModifier.Type.FixedValue,
            10
        ));
    }
}

[Serializable]
public class AttributeModifierGroup {
    [TableColumnWidth(100, resizable:false)] public string key;
    public List<AttributeModifier> modifiers;

    public AttributeModifierGroup(string key) {
        this.key = key;
        modifiers = new List<AttributeModifier>();
    }
}