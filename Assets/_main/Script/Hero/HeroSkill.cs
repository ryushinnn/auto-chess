using UnityEngine;

public class HeroSkill : HeroAbility {
    float energy;
    
    public void RegenEnergy(float amount) {
        energy = Mathf.Min(energy + amount, HeroTrait.MAX_ENERGY);
        hero.GetAbility<HeroHealth>().UpdateEnergyBar(energy / HeroTrait.MAX_ENERGY);
    }

    public bool UseSkill() {
        if (energy < HeroTrait.MAX_ENERGY) return false;
        
        Debug.Log("use skill");
        energy = 0;
        hero.GetAbility<HeroHealth>().UpdateEnergyBar(0);
        return true;
    }
}