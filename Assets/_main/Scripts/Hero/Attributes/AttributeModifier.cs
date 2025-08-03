using System;
using RExt.Extensions;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public static class AttributeModifierKey {
    public const string MaxHp = "maxhp";
    public const string PhysicalDamage = "pdmg";
    public const string MagicalDamage = "mdmg";
    public const string Armor = "armor";
    public const string Resistance = "resistance";
    public const string AttackSpeed = "atkspd";
    public const string CriticalChance = "crit";
    public const string CriticalDamage = "cdmg";
    public const string EnergyRegenEfficient = "energy";
    public const string PhysicalPenetration = "ppen";
    public const string MagicalPenetration = "mpen";
    public const string LifeSteal = "lifesteal";
    public const string Tenacity = "tenacity";
}

[Serializable]
public class AttributeModifier {
    public enum Type {
        FixedValue,
        Percentage
    }
    
    const string ID_PLACE_HOLDER = "<will be auto generated>";
    
    [StringDropdown(typeof(AttributeModifierKey))] 
    public string key;
    [HideIf("@this.id == AttributeModifier.ID_PLACE_HOLDER")] 
    public string id = ID_PLACE_HOLDER;
    [HideIf("@this.id == AttributeModifier.ID_PLACE_HOLDER || this.owner == null")] 
    public Hero owner;
    [HideIf("@this.effectKey == null || this.effectKey == \"\"")] 
    public string effectKey;
    public float value;
    public Type type;
    [HideIf("@this.id == AttributeModifier.ID_PLACE_HOLDER || this.owner == null")] 
    public int stacks;
    [HideIf("@this.id == AttributeModifier.ID_PLACE_HOLDER || this.owner == null")] 
    public float duration;
    [HideIf("@this.id == AttributeModifier.ID_PLACE_HOLDER || this.owner == null")] 
    public bool permanent;

    public static AttributeModifier Create(Hero owner, string key, float value, Type type, float duration, int stacks = 1) {
        return new AttributeModifier {
            key = key,
            id = Guid.NewGuid().ToString(),
            owner = owner,
            effectKey = null,
            value = value,
            type = type,
            stacks = stacks,
            duration = duration,
            permanent = false,
        };
    }

    public static AttributeModifier Create(string effectKey, string key, float value, Type type, float duration, int stacks = 1) {
        return new AttributeModifier {
            key = key,
            id = Guid.NewGuid().ToString(),
            owner = null,
            effectKey = effectKey,
            value = value,
            type = type,
            stacks = stacks,
            duration = duration,
            permanent = false,
        };
    }
    
    public static AttributeModifier Create(AttributeModifier modifier) {
        return new AttributeModifier {
            key = modifier.key,
            id = Guid.NewGuid().ToString(),
            owner = modifier.owner,
            effectKey = modifier.effectKey,
            value = modifier.value,
            type = modifier.type,
            stacks = modifier.stacks,
            duration = modifier.duration,
            permanent = modifier.permanent
        };
    }
    
    public bool ControlledBySet() {
        return effectKey.IsValid();
    }
}

[Serializable]
public class AttributeModifierSet {
    [TableColumnWidth(150, resizable:false)] public string effectKey;
    [VerticalGroup("Effect")] public string id;
    [VerticalGroup("Effect")] public Hero owner;
    [VerticalGroup("Effect")] public AttributeModifier[] modifiers;
    [VerticalGroup("Effect")] public int stacks;
    [VerticalGroup("Effect")] public float duration;
    [HideInInspector] public Mark mark;

    public static AttributeModifierSet Create(Hero owner, string effectKey, float duration, (string key, float value, AttributeModifier.Type type)[] modifiers, int stacks = 1, bool createMark = true) {
        var set = new AttributeModifierSet {
            effectKey = effectKey,
            id = Guid.NewGuid().ToString(),
            owner = owner,
            modifiers = new AttributeModifier[modifiers.Length],
            stacks = stacks,
            duration = duration,
            mark = createMark
                ? Mark.Create(
                    effectKey,
                    owner,
                    stacks,
                    duration,
                    false
                )
                : null
        };

        for (int i = 0; i < modifiers.Length; i++) {
            set.modifiers[i] = AttributeModifier.Create(
                set.effectKey,
                modifiers[i].key,
                modifiers[i].value,
                modifiers[i].type,
                -1,    // controlled by set
                -1      // controlled by set
            );
        }

        return set;
    }

    public bool SameAs(AttributeModifierSet other) {
        return effectKey == other.effectKey && owner == other.owner;
    }
}

public static partial class Translator {
    public static string ToString(this string key) {
        return key switch {
            AttributeModifierKey.MaxHp =>                "Nguyên khí",
            AttributeModifierKey.PhysicalDamage =>       "Căn cốt",
            AttributeModifierKey.MagicalDamage =>        "Nội lực",
            AttributeModifierKey.Armor =>                "Hộ thể",
            AttributeModifierKey.Resistance =>           "Trấn hồn",
            AttributeModifierKey.AttackSpeed =>          "Thân pháp",
            AttributeModifierKey.CriticalChance =>       "Bạo kích",
            AttributeModifierKey.CriticalDamage =>       "Bộc phá",
            AttributeModifierKey.EnergyRegenEfficient => "Dưỡng linh",
            AttributeModifierKey.PhysicalPenetration =>  "Phá thể",
            AttributeModifierKey.MagicalPenetration =>   "Trảm hồn",
            AttributeModifierKey.LifeSteal =>            "Hấp huyết",
            AttributeModifierKey.Tenacity =>             "Tịnh tâm",
            _ => key,
        };
    }
}