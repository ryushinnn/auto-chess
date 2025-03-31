using System;
using RExt.Extension;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/Trait")]
public class HeroTrait : ScriptableObject {
    [TitleGroup("Identity")]
    public string id;
    public new string name;
    public string subName;
    public bool summoned;
    [HideIf("summoned")] public Realm realm;
    [HideIf("summoned")] public Role role;
    [HideIf("summoned")] public int price;
    
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
    public float criticalChance;
    public float criticalDamage;
    public float energyRegenEfficient;
    
    [TitleGroup("Advanced")]
    public float physicalPenetration;
    public float magicalPenetration;
    public float lifeSteal;
    public float tenacity;

    [TitleGroup("Unmodifiable")]
    public float energyRegenPerAttack;
    public float energyRegenPerHit;
    public int attackRange;
    
    [TitleGroup("Contants & Limits")]
    public const float MAX_ENERGY = 100;
    public const float DAMAGE_REDUCTION_CONSTANT = 100;
    public const float MAX_CRITICAL_CHANCE = 0.99f;
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
    [SerializeField, ReadOnly] float minDps;
    [SerializeField, ReadOnly] float avgDps;
    [SerializeField, ReadOnly, LabelText("Physical Reduction (%)")] float physicalReduction;
    [SerializeField, ReadOnly, LabelText("Magical Reduction (%)")] float magicalReduction;

    void OnValidate() {
        var outputDamage = physicalDamage + magicalDamage;
        minDps = outputDamage * attackSpeed;
        avgDps = minDps * (1 + criticalChance * (criticalDamage - 1));
        physicalReduction = 100 * (1 - DAMAGE_REDUCTION_CONSTANT / (DAMAGE_REDUCTION_CONSTANT + armor));
        magicalReduction = 100 * (1 - DAMAGE_REDUCTION_CONSTANT / (DAMAGE_REDUCTION_CONSTANT + resistance));
        
        if (summoned) {
            realm = Realm.None;
            role = Role.None;
            price = -1;
        }
    }

    public string DisplayName() {
        return name + (subName.IsValid() ? "\n" + subName : "");
    }
}