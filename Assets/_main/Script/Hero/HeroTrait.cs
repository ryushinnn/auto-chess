using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/Trait")]
public class HeroTrait : ScriptableObject {
    [TitleGroup("Basic")]
    public float maxHp;
    public float physicalDamage;
    public float magicalPower;
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
    
    [TitleGroup("Hidden")]
    public const float MAX_ENERGY = 100;
    public const float MAX_DMG_REDUCTION = 0.9f;
    public const float MAX_CRITICAL_CHANCE = 0.99f;
    public const float MAX_PENETRATION = 0.7f;
    public const float MAX_LIFE_STEAL = 1;
    public const float MAX_TENACITY = 0.5f;
}

public class HeroAttributes {
    public float hp;
    public float energy;
}