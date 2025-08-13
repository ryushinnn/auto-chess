using System;
using System.Text.RegularExpressions;
using RExt.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/Trait")]
public class HeroTrait : ScriptableObject {
    [TitleGroup("Identity")]
    [StringDropdown(typeof(HeroId))] public string id;
    public new string name;
    public string subName;
    public bool summoned;
    [HideIf("summoned")] public Realm realm;
    [HideIf("summoned")] public Role role;
    [HideIf("summoned")] public Reputation reputation;
    public bool moveable;
    
    [TitleGroup("Asset")]
    public Mecanim mecanim;
    [PreviewField(ObjectFieldAlignment.Left), HideIf("summoned")] public Sprite thumbnail;
    [PreviewField(ObjectFieldAlignment.Left)] public Sprite skillIcon;
    
    [TitleGroup("Basic")]
    public float maxHp;
    public float physicalDamage;
    public float magicalDamage;
    public float armor;
    public float resistance;
    public float attackSpeed;
    public float movementSpeed;

    [TitleGroup("Unmodifiable")]
    public float energyRegenPerAttack;
    public float energyRegenPerHit;
    public int attackRange;

    [TitleGroup("Evolution")]
    public Evolution[] evolutions;

    [TitleGroup("Skill")]
    [TableList(ShowIndexLabels = true)] public HeroSkillParam[] skillParams;
    [TextArea(1,100)] public string attackDescription;
    public string skillName;
    [TextArea(1,100)] public string skillDescription;
    public bool unstoppable;
    public string[] specialKeys;

    [TitleGroup("Contants & Limits")]
    public const float BASE_CRITICAL_CHANCE = 0.15f;
    public const float BASE_CRITICAL_DAMAGE = 1.5f;
    public const float BASE_ENERGY_REGEN_EFFICIENCY = 1;
    public const float BASE_PENETRATION = 0;
    public const float BASE_LIFE_STEAL = 0;
    public const float BASE_TENACITY = 0;
    public const float MAX_ENERGY = 100;
    public const float DAMAGE_REDUCTION_CONSTANT = 100;
    public const float MAX_CRITICAL_CHANCE = 1;
    public const float MAX_PENETRATION = 0.7f;
    public const float MAX_LIFE_STEAL = 1;
    public const float MAX_TENACITY = 0.5f;
    public const float ANTI_HEAL_EFFICIENCY = 0.5f;
    public const float MIN_ARMOR_AND_RESISTANCE = 1;
    public const float MIN_DAMAGE_DEALT = 1;
    public const float MIN_DAMAGE = 1;
    public const float MIN_ATTACK_SPEED = 0.1f;
    public const float MIN_MOVEMENT_SPEED = 0.1f;

    [TitleGroup("Calculated Result")]
    [SerializeField, ReadOnly] float dps;
    [SerializeField, ReadOnly, LabelText("Physical Reduction (%)")] float physicalReduction;
    [SerializeField, ReadOnly, LabelText("Magical Reduction (%)")] float magicalReduction;

    void OnValidate() {
        var outputDamage = physicalDamage + magicalDamage;
        dps = outputDamage * attackSpeed;
        physicalReduction = 100 * (1 - DAMAGE_REDUCTION_CONSTANT / (DAMAGE_REDUCTION_CONSTANT + armor));
        magicalReduction = 100 * (1 - DAMAGE_REDUCTION_CONSTANT / (DAMAGE_REDUCTION_CONSTANT + resistance));
        
        if (summoned) {
            realm = Realm.None;
            role = Role.None;
            reputation = Reputation.None;
        }
    }

    public string DisplayName() {
        return name + (subName.IsValid() ? "\n" + subName : "");
    }

    public string SkillDescription() {
        var attack = attackDescription.IsValid() ? $"<color=grey><i>Nội tại: {attackDescription}</color></i>\n\n" : "";
        
        var placeholderPattern = @"#\[(\d+)\]";
        var skill = Regex.Replace(skillDescription, placeholderPattern, match => {
            var index = int.Parse(match.Groups[1].Value);
            if (index >= 0 && index < skillParams.Length) {
                var param = skillParams[index];
                return param.isPercentage ? $"{param.value * 100}%" : $"{param.value}";
            }

            return match.Value;
        });

        var note = unstoppable ? "\n<color=grey><i>(Khi đang sử dụng kỹ năng, không thể bị cản phá)</color></i>" : "";

        return $"<uppercase>{skillName}</uppercase>\n\n{attack}{skill}{note}";
    }
}

[Serializable]
public class Evolution {
    public HeroRank rank;
    public AttributeModifier[] modifiers;
}

[Serializable]
public class HeroSkillParam {
    public string key;
    public float value;
    public bool isPercentage;
}

public static class HeroId {
    public const string D_u_m_m_y = "D_u_m_m_y";
    public const string Aatrox_Dark = "Aatrox_Dark";
    public const string Aatrox_Light = "Aatrox_Light";
    public const string Akali = "Akali";
    public const string Ashe = "Ashe";
    public const string Caitlyn = "Caitlyn";
    public const string Irelia = "Irelia";
    public const string Jinx = "Jinx";
    public const string Katarina = "Katarina";
    public const string Malphite = "Malphite";
    public const string MissFortune = "MissFortune";
    public const string Morgana = "Morgana";
    public const string Teemo = "Teemo";
    public const string Tristana = "Tristana";
    public const string Yasuo = "Yasuo";
    public const string Yone = "Yone";
    public const string Zed = "Zed";
}

public enum Realm {
    None,
    Mortal,
    Divine,
    Nether,
    Mecha,
    Chaos
}

[Flags]
public enum Role {
    None = 0x00,
    Duelist = 0x01,
    Sorcerer = 0x02,
    Marksman = 0x04,
    Assassin = 0x08,
    Bruiser = 0x10,
    Cultist = 0x20,
}

public enum Reputation {
    None,
    Unknown,
    Elite,
    Legendary,
}