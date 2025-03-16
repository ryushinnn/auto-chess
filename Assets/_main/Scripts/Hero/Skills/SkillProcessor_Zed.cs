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
        events = new Action[]{ThrowShurikens, TriggerExplosion};
    }

    void ThrowShurikens() {
        if (hero.Target == null) return;

        aimedTarget = hero.Target;
        aimedTarget.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                attributes.PhysicalDamage * DMG_MUL, 
                DamageType.Physical, 
                attributes.PhysicalPenetration
            ));
    }

    void TriggerExplosion() {
        if (aimedTarget == null) return;
        
        aimedTarget.GetAbility<HeroAttributes>().TakeDamage(
            Damage.Create(
                attributes.PhysicalDamage * attributes.CriticalDamage,
                DamageType.Physical,
                attributes.PhysicalPenetration
            ));
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