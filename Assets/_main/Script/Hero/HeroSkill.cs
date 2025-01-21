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
            "Yasuo" => new Skill_Yasuo(hero),
        };
    }

    public void RegenEnergy(float amount) {
        energy = Mathf.Min(energy + amount, HeroTrait.MAX_ENERGY);
        hero.GetAbility<HeroAttributes>().UpdateEnergyBar(energy / HeroTrait.MAX_ENERGY);
    }

    public bool UseSkill() {
        if (energy < HeroTrait.MAX_ENERGY 
            || isUsingSkill
            || hero.GetAbility<HeroStatusEffects>().IsAirborne
            || hero.GetAbility<HeroStatusEffects>().IsStun
            || hero.GetAbility<HeroStatusEffects>().IsSilent) return false;
        
        var duration = hero.Mecanim.UseSkill(skill.Events);
        isUsingSkill = true;
        if (skill.Unstoppable) {
            hero.GetAbility<HeroStatusEffects>().Unstoppable(true);
        }
        hero.GetAbility<HeroRotation>().Rotate(hero.Target.transform.position - hero.transform.position);
        energy = 0;
        hero.GetAbility<HeroAttributes>().UpdateEnergyBar(0);
        resetUsingSkillTween?.Kill();
        resetUsingSkillTween = DOVirtual.DelayedCall(duration, () => {
            isUsingSkill = false;
            if (skill.Unstoppable) {
                hero.GetAbility<HeroStatusEffects>().Unstoppable(false);
            }
        });
        return true;
    }

    public void Interrupt() {
        hero.Mecanim.InterruptSkill();
        resetUsingSkillTween?.Kill();
        isUsingSkill = false;
        if (skill.Unstoppable) {
            hero.GetAbility<HeroStatusEffects>().Unstoppable(false);
        }
    }
}