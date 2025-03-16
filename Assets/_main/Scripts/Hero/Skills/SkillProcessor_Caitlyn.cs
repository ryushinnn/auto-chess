using System;
using System.Threading.Tasks;
using RExt.Extension;

/// <summary>
/// uong tra hoi mau trong 2s, moi 0.25s hoi mau = 3% maxhp + 15% st vat ly
/// sau do tang 25% giap va khang phep trong 3s
/// </summary>
public class SkillProcessor_Caitlyn : SkillProcessor {
    const float HP_MUL_TO_HEAL = 0.03f;
    const float DMG_MUL_TO_HEAL = 0.15f;
    const int TOTAL_TIME = 2000; //ms
    const int INTERVAL = 250; //ms
    const float DEFENSE_MUL = 0.25f;
    const float DEFENSE_DURATION = 3f;
    const string HOT_KEY = "caitlyn_tea";
    
    public SkillProcessor_Caitlyn(Hero hero) : base(hero) {
        events = new Action[] { DrinkTea };
        unstoppable = true;
    }

    async void DrinkTea() {
        Heal();
        await Task.Delay(TOTAL_TIME);
        GainDefense();
    }

    void Heal() {
        if (!attributes.IsAlive) return;
        
        attributes.AddHealOverTime(
            HealOverTime.Create(
                HOT_KEY,
                attributes.MaxHp * HP_MUL_TO_HEAL + attributes.PhysicalDamage * DMG_MUL_TO_HEAL,
                TOTAL_TIME / INTERVAL,
                INTERVAL.ToSeconds()
            ));
    }

    void GainDefense() {
        if (!attributes.IsAlive) return;
        
        attributes.AddAttributeModifier(
            AttributeModifier.Create(
                AttributeModifierKey.Armor,
                attributes.Armor * DEFENSE_MUL,
                ModifierType.Percentage,
                DEFENSE_DURATION
            ));
        
        attributes.AddAttributeModifier(
            AttributeModifier.Create(
                AttributeModifierKey.Resistance,
                attributes.Resistance * DEFENSE_MUL,
                ModifierType.Percentage,
                DEFENSE_DURATION
            ));
    }
}