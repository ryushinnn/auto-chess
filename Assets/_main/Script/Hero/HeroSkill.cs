using DG.Tweening;
using UnityEngine;

public class HeroSkill : HeroAbility {
    public bool IsUsingSkill => isUsingSkill;
    
    float energy;
    bool isUsingSkill;
    Skill skill;
    Tween resetUsingSkillTween;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        skill = hero.name switch {
            "Aatrox" => new Skill_Aatrox(hero),
            "Yasuo" => new Skill_Yasuo(),
        };
    }

    public void RegenEnergy(float amount) {
        energy = Mathf.Min(energy + amount, HeroTrait.MAX_ENERGY);
        hero.GetAbility<HeroHealth>().UpdateEnergyBar(energy / HeroTrait.MAX_ENERGY);
    }

    public bool UseSkill() {
        if (energy < HeroTrait.MAX_ENERGY || isUsingSkill) return false;
        
        Debug.Log("use skill");
        var duration = hero.Mecanim.UseSkill(skill.Events);
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