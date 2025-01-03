using UnityEngine;

public abstract class HeroAbility : MonoBehaviour {
    [SerializeField] protected bool isActive = true;

    public bool IsActive => isActive;

    protected Hero hero;
    
    public void Initialize(Hero hero) {
        this.hero = hero;
    }
    
    public virtual void PreProcess() { }
    public virtual void Process() { }
    public virtual void PostProcess() { }
}