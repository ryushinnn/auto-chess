public class SkillProcessor_Yone : SkillProcessor {
    const float DIVINE_DMG_MUL = 2.5f;
    const float DIVINE_AIRBORNE_TIME = 1.5f;
    const float DIVINE_REGEN_ENERGY = 50f;
    const float DEVIL_DMG_MUL_0 = 0.5f;
    const float DEVIL_DMG_MUL_1 = 2.5f;
    const float DIVINE_ANIM_LENGTH = 4;
    const float DEVIL_ANIM_LENGTH = 4.25f;
    readonly float[] DIVINE_TIMERS = { 1.66f };
    readonly float[] DEVIL_TIMERS = { 0.12f, 0.72f, 1.08f, 2.24f };

    AttackProcessor_Yone atkProcessor;
    YoneSword sword;
    
    public SkillProcessor_Yone(Hero hero) : base(hero) {
        Name = "Thiên Ân/Tuyệt Diệt";
        Description = "Thanh Kiếm sắp sử dụng (đang sẵn sàng) sẽ quyết định kỹ năng nào sẽ được kích hoạt.\n" +
                      "- <color=yellow>Thần Kiếm</color>: Lao tới chém 1 nhát gây " +
                      $"({DIVINE_DMG_MUL * 100}% <sprite name=pdmg>) sát thương vật lý và hất tung mục tiêu " +
                      $"trong {DIVINE_AIRBORNE_TIME}s. Nếu nhát chém này tiêu diệt mục tiêu, " +
                      $"hồi phục {DIVINE_REGEN_ENERGY} <sprite name=eng>.\n" +
                      $"- <color=purple>Quỷ Kiếm</color>: Chém liên hoàn 4 nhát, 3 nhát đầu gây " +
                      $"({DEVIL_DMG_MUL_0 * 100}% <sprite name=pdmg>) sát thương phép, nhát chém cuối gây " +
                      $"({DEVIL_DMG_MUL_1 * 100}% <sprite name=pdmg>) sát thương phép, các " +
                      $"nhát chém này đều có thể chí mạng.";

        atkProcessor = this.hero.GetAbility<HeroAttack>().Processor as AttackProcessor_Yone;
    }

    public override void Begin(out float animLength) {
        sword = (YoneSword)atkProcessor.CustomInt["sword"];
        AnimationLength = sword == YoneSword.Divine ? DIVINE_ANIM_LENGTH : DEVIL_ANIM_LENGTH;
        Timers = sword == YoneSword.Divine ? DIVINE_TIMERS : DEVIL_TIMERS;
        base.Begin(out animLength);
    }

    public override void Process(float timer) {
        if (sword == YoneSword.Divine) {
            if (timer >= Timers[0] && skillExecuted == 0) {
                Judge();
                skillExecuted++;
            }
        }
        else {
            if (timer >= Timers[0] && skillExecuted == 0) {
                LightSmite();
                skillExecuted++;
            }
            else if (timer >= Timers[1] && skillExecuted == 1) {
                LightSmite();
                skillExecuted++;
            }
            else if (timer >= Timers[2] && skillExecuted == 2) {
                LightSmite();
                skillExecuted++;
            }
            else if (timer >= Timers[3] && skillExecuted == 3) {
                HeavySmite();
                skillExecuted++;
            }
        }
    }

    void Judge() {
        if (((BattleHero)hero).Target == null) return;
        
        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical,false,
            scaledValues:new[]{(DIVINE_DMG_MUL, DamageType.Physical)}));
        ((BattleHero)hero).Target.GetAbility<HeroStatusEffects>().Airborne(DIVINE_AIRBORNE_TIME);
        
        if (!((BattleHero)hero).Target.GetAbility<HeroAttributes>().IsAlive){
            attributes.RegenEnergy(DIVINE_REGEN_ENERGY);
        }
    }

    void LightSmite() {
        if (((BattleHero)hero).Target == null) return;
        
        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Magical, attributes.Crit(),
            scaledValues: new[] { (DEVIL_DMG_MUL_0, DamageType.Physical) }));
    }

    void HeavySmite() {
        if (((BattleHero)hero).Target == null) return;
        
        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Magical,attributes.Crit(),
            scaledValues: new[] { (DEVIL_DMG_MUL_1, DamageType.Physical) }));
    }
}