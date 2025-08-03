using System;
using System.Threading.Tasks;
using RExt.Extensions;

public class SkillProcessor_Caitlyn : SkillProcessor {
    const float HP_MUL_TO_HEAL = 0.03f;
    const float DMG_MUL_TO_HEAL = 0.15f;
    const int TOTAL_TIME = 1500; //ms
    const int INTERVAL = 250; //ms
    const float ATK_SPD_MUL = 0.25f;
    const float ATK_SPD_DURATION = 3f;
    const string HOT_KEY = "caitlyn_cake";
    const string ATK_SPD_KEY = "caitlyn_cake_atkspd";
    
    public SkillProcessor_Caitlyn(Hero hero) : base(hero) {
        AnimationLength = 6.5f;
        Timers = new[] { 2.2f, 5.6f };
        Unstoppable = true;
        Name = "Đến giờ trà chiều rồi! ^_^";
        Description = $"Ăn bánh giúp hồi máu trong {TOTAL_TIME/1000f}s, " +
                      $"mỗi {INTERVAL/1000f}s hồi máu bằng ({HP_MUL_TO_HEAL*100}% <sprite name=hp> tối đa + " +
                      $"{DMG_MUL_TO_HEAL*100}% <sprite name=pdmg>). Sau đó tăng " +
                      $"{ATK_SPD_MUL*100}% <sprite name=aspd> trong {ATK_SPD_DURATION}s.";        
    }

    public override void Process(float timer) {
        if (timer >= Timers[0] && skillExecuted == 0) {
            DrinkTea();
            skillExecuted++;
        }
        else if (timer >= Timers[1] && skillExecuted == 1) {
            Focus();
            skillExecuted++;
        }
    }

    void DrinkTea() {
        if (!attributes.IsAlive) return;
        
        attributes.AddHealOverTime(
            HealOverTime.Create(
                HOT_KEY,
                hero,
                attributes.MaxHp * HP_MUL_TO_HEAL + attributes.PhysicalDamage * DMG_MUL_TO_HEAL,
                TOTAL_TIME / INTERVAL,
                INTERVAL.ToSeconds()
            ));
    }

    void Focus() {
        if (!attributes.IsAlive) return;
        
        attributes.AddAttributeModifier(
            AttributeModifierSet.Create(
                hero,
                ATK_SPD_KEY,
                ATK_SPD_DURATION,
                new[] {
                    (AttributeModifierKey.AttackSpeed, ATK_SPD_MUL, AttributeModifier.Type.Percentage)
                }
            ));
    }
}