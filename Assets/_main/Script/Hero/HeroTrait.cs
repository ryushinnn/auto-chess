using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/Trait")]
public class HeroTrait : ScriptableObject {
    [TitleGroup("Basic")]
    public float hp;
    public float physicalDamage;
    public float magicalPower;
    public float armor;
    public float resistance;
    public float attackSpeed;
    public float movementSpeed;
    public float criticalChance;
    public float criticalDamage;
    
    [TitleGroup("Advanced")]
    public float physicalPenetration;
    public float magicalPenetration;
    public float lifeSteal;
    public float tenacity;
}