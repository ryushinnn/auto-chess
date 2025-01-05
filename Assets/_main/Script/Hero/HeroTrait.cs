using UnityEngine;

[CreateAssetMenu(menuName = "Hero/Trait")]
public class HeroTrait : ScriptableObject {
    public float hp;
    public float physicalDamage;
    public float magicalPower;
    public float armor;
    public float resistance;
    public float attackSpeed;
    public float movementSpeed;
}