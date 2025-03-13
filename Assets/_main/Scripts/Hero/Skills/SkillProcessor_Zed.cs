using System;

/// <summary>
/// nem phi tieu vao 1 muc tieu, gay 100% st vat ly
/// sau do nhay xuong va kich no phi tieu gay st chi mang nen muc tieu chinh
/// va pham vi 1 xung quanh, gay choang 1s cho muc tieu chinh va choang 0.5s cho muc tieu khac
/// </summary>
public class SkillProcessor_Zed : SkillProcessor {
    Hero aimedTarget;
    
    const float DMG_MUL = 1f;
    const int RANGE = 1;
    const float STUN_MAIN = 1f;
    const float STUN_OTHERS = 0.5f;

    public SkillProcessor_Zed(Hero hero) : base(hero) {
        this.hero = hero;
        events = new Action[]{ThrowShurikens, TriggerExplosion};
        unstoppable = false;
    }

    void ThrowShurikens() {
        if (hero.Target == null) return;

        aimedTarget = hero.Target;
        var attribute = hero.GetAbility<HeroAttributes>();
        aimedTarget.GetAbility<HeroAttributes>().TakeDamage(
            attribute.PhysicalDamage * DMG_MUL, 
            DamageType.Physical, 
            attribute.PhysicalPenetration);
    }

    void TriggerExplosion() {
        if (aimedTarget == null) return;
        
        var attribute = hero.GetAbility<HeroAttributes>();
        aimedTarget.GetAbility<HeroAttributes>().TakeDamage(
            attribute.PhysicalDamage * attribute.CriticalDamage,
            DamageType.Physical,
            attribute.PhysicalPenetration);
        aimedTarget.GetAbility<HeroStatusEffects>().Stun(STUN_MAIN);
        
        // same error with yasuo skill, review later
        
        // var affectedNodes = Map.Instance.GetCircle(aimedTarget.MNode.X, aimedTarget.MNode.Y, RANGE, true);
        // foreach (var node in affectedNodes) {
        //     if (!node.HasNone()) {
        //         node.Process(x => {
        //             if (x is Hero h && h.Side != hero.Side) {
        //                 h.GetAbility<HeroAttributes>().TakeDamage(
        //                     attribute.PhysicalDamage * attribute.CriticalDamage,
        //                     DamageType.Physical,
        //                     attribute.PhysicalPenetration);
        //                 
        //                 h.GetAbility<HeroStatusEffects>().Stun(h == aimedTarget ? STUN_MAIN : STUN_OTHERS);
        //             }
        //         });
        //     }
        // }
        
        aimedTarget = null;
    }
}