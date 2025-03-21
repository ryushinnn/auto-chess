using System;
using System.Threading.Tasks;
using RExt.Extension;

/// <summary>
/// an banh hoi mau trong 1.5s, moi 0.25s hoi mau = 3% maxhp + 15% st vat ly
/// sau do tang 25% toc do danh trong 3s
/// </summary>
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
        events = new Action[] { DrinkTea, Focus };
        unstoppable = true;
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
                    (AttributeModifier.Key.AttackSpeed, ATK_SPD_MUL, AttributeModifier.Type.Percentage)
                }
            ));
    }
}