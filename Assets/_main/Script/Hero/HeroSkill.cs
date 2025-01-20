using DG.Tweening;
using UnityEngine;

public class HeroSkill : HeroAbility {
    public bool IsUsingSkill => isUsingSkill;
    
    float energy;
    bool isUsingSkill;
    Tween resetUsingSkillTween;
    
    public void RegenEnergy(float amount) {
        energy = Mathf.Min(energy + amount, HeroTrait.MAX_ENERGY);
        hero.GetAbility<HeroHealth>().UpdateEnergyBar(energy / HeroTrait.MAX_ENERGY);
    }

    public bool UseSkill() {
        if (energy < HeroTrait.MAX_ENERGY) return false;
        
        Debug.Log("use skill");
        var duration = hero.Mecanim.UseSkill();
        isUsingSkill = true;
        hero.GetAbility<HeroRotation>().Rotate(hero.Target.transform.position - hero.transform.position);
        energy = 0;
        hero.GetAbility<HeroHealth>().UpdateEnergyBar(0);
        resetUsingSkillTween?.Kill();
        resetUsingSkillTween = DOVirtual.DelayedCall(duration, () => {
            isUsingSkill = false;
        });
        return true;
    }
}