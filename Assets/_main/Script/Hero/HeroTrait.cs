using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/Trait")]
public class HeroTrait : ScriptableObject {
    [TitleGroup("Identity")]
    public Realm realm;
    public Role role;
    public int price;
    
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
    
    [TitleGroup("Configs")]
    public const float MAX_ENERGY = 100;
    public const float MAX_DMG_REDUCTION = 0.9f;
    public const float MAX_CRITICAL_CHANCE = 0.99f;
    public const float MAX_PENETRATION = 0.7f;
    public const float MAX_LIFE_STEAL = 1;
    public const float MAX_TENACITY = 0.5f;
    public const float HEAL_UPON_ANTI_HEALTH = 0.5f;
    public const float MIN_ARMOR_AND_RESISTANCE = 1;
    public const float MIN_DAMAGE = 1;
    public const float MIN_ATTACK_SPEED = 0.1f;
    public const float MIN_MOVEMENT_SPEED = 0.1f;
}