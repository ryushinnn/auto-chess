public class SkillProcessor_Zed : SkillProcessor {
    Hero aimedTarget;
    
    const float DMG_MUL = 1f;
    const int RANGE = 1;
    const float STUN_MAIN = 1f;
    const float STUN_OTHERS = 0.5f;

    public SkillProcessor_Zed(Hero hero) : base(hero) {
        AnimationLength = 3;
        Timers = new[] { 0.83f, 1.66f };
        Name = "Phi Tiêu Bộc Phá";
        Description = "Nhảy lên và ném phi tiêu vào 1 mục tiêu gây " +
                      $"({DMG_MUL * 100}% <sprite name=pdmg>) sát thương vật lý sau đó nhảy xuống, kích nổ phi tiêu " +
                      $"gây sát thương chí mạng cho mục tiêu và phạm vi {RANGE} xung quanh, " +
                      $"gây choáng {STUN_MAIN}s cho mục tiêu chính và {STUN_OTHERS}s cho các mục tiêu khác.";
    }

    public override void Process(float timer) {
        if (timer >= Timers[0] && skillExecuted == 0) {
            ThrowShurikens();
            skillExecuted++;
        }
        else if (timer >= Timers[1] && skillExecuted == 1) {
            TriggerExplosion();
            skillExecuted++;
        }
    }

    void ThrowShurikens() {
        if (((BattleHero)hero).Target == null) return;

        aimedTarget = ((BattleHero)hero).Target;
        aimedTarget.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical, false,
            scaledValues: new[] { (DMG_MUL, DamageType.Physical) }));
    }

    void TriggerExplosion() {
        if (aimedTarget == null) return;
        
        aimedTarget.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical,true,
            scaledValues: new[] { (1f, DamageType.Physical) }));
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