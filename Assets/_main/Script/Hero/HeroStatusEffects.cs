using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroStatusEffects : HeroAbility {
    public bool IsAirborne => isAirborne;
    public bool IsUnstoppable => isUnstoppable;
    public bool IsStun => isStun;
    public bool IsSilent => isSilent;
    public bool IsAntiHeal => isAntiHeal;

    [SerializeField, ReadOnly] float tenacity;
    [SerializeField, ReadOnly] bool isAirborne;
    [SerializeField, ReadOnly] bool isUnstoppable;
    [SerializeField, ReadOnly] bool isStun;
    [SerializeField, ReadOnly] bool isSilent;
    [SerializeField, ReadOnly] bool isAntiHeal;
    
    Sequence airborneSequence;
    float stunDuration;
    float silenceDuration;
    float antiHealDuration;
    
    const float AIRBORNE_MAX_HEIGHT = 2;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        tenacity = hero.Trait.tenacity;
    }

    public override void Process() {
        if (isStun) {
            stunDuration -= Time.deltaTime;
            if (stunDuration <= 0) {
                isStun = false;
            }
        }
        
        if (isSilent) {
            silenceDuration -= Time.deltaTime;
            if (silenceDuration <= 0) {
                isSilent = false;
            }
        }
    }

    public void Airborne(float duration) {
        if (isUnstoppable) return;
        
        var currentHeight = hero.Model.localPosition.y;
        var goUpDistance = AIRBORNE_MAX_HEIGHT - currentHeight;
        var goDownDistance = AIRBORNE_MAX_HEIGHT;
        var goUpTime = duration / (goUpDistance + goDownDistance) * goUpDistance;
        var goDownTime = duration - goUpTime;
        isAirborne = true;
        airborneSequence?.Kill();
        airborneSequence = DOTween.Sequence()
            .Append(hero.Model.DOLocalMoveY(AIRBORNE_MAX_HEIGHT, goUpTime).SetEase(Ease.OutQuad))
            .Append(hero.Model.DOLocalMoveY(0, goDownTime).SetEase(Ease.InQuad))
            .AppendCallback(()=>isAirborne = false);
        
        hero.GetAbility<HeroAttack>().Interrupt();
        hero.GetAbility<HeroSkill>().Interrupt();
        hero.GetAbility<HeroMovement>().StopMove(true);
    }
    
    public void Unstoppable(bool value) {
        isUnstoppable = value;
    }

    public void Stun(float duration) {
        if (isUnstoppable) return;

        duration *= (1-tenacity);
        isStun = true;
        stunDuration = Mathf.Max(stunDuration, duration);
        hero.GetAbility<HeroAttack>().Interrupt();
        hero.GetAbility<HeroSkill>().Interrupt();
        hero.GetAbility<HeroMovement>().StopMove(true);
    }
    
    public void Silent(float duration) {
        if (isUnstoppable) return;
        
        duration *= (1-tenacity);
        isSilent = true;
        silenceDuration = Mathf.Max(silenceDuration, duration);
        hero.GetAbility<HeroSkill>().Interrupt();
    }
    
    public void AntiHeal(float duration) {
        isAntiHeal = true;
        antiHealDuration = Mathf.Max(antiHealDuration, duration);
    }
    
    [Button]
    void Dev_Stun() {
        Stun(1);
    }
    
    [Button]
    void Dev_Silent() {
        Silent(1);
    }
}