using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// thay doi qua lai giua than kiem va quy kiem sau moi don danh
/// don danh than kiem +1 stack suy yeu: giam 5% st cua muc tieu trong 2s, toi da 3 stack
/// don danh quy kiem cho toi da 20% hut mau, dua vao mau da mat
/// thanh kiem cuoi cung duoc su dung cung quyet dinh skill la gi
/// </summary>
public class AttackProcessor_Yone : AttackProcessor {
    List<string> reduceDmgModifierIds = new();
    
    const float DIVINE_WEAKENING_PER_STACK = -0.05f;
    const int DIVINE_WEAKENING_MAX_STACK = 3;
    const int DIVINE_WEAKENING_DURATION = 2;
    const float DEVIL_VAMP_MIN = 0f;
    const float DEVIL_VAMP_MAX = 0.2f;

    public AttackProcessor_Yone(Hero hero) : base(hero) {
        this.hero = hero;
        customInt = new CustomData<int>();
        customInt["sword"] = (int)YoneSword.Divine;
    }

    public override void Execute() {
        CalculateDamage(out var dmg, out var type, out var pen, out var crit);
        hero.Mecanim.Attack(() => {
            if (hero.Target == null) return;
            var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(Damage.Create(dmg,type,pen,crit));
            var heal = outputDamage * attributes.LifeSteal;
            if ((YoneSword)customInt["sword"] == YoneSword.Devil) {
                heal += outputDamage * ((attributes.Hp / attributes.MaxHp) * (DEVIL_VAMP_MAX - DEVIL_VAMP_MIN) + DEVIL_VAMP_MIN);
            }

            if (heal > 0) {
                attributes.Heal(heal);
            }

            if ((YoneSword)customInt["sword"] == YoneSword.Divine) {
                reduceDmgModifierIds.ForEach(x=>hero.Target.GetAbility<HeroAttributes>().RemoveAttributeModifier(x));
                reduceDmgModifierIds.Clear();
                var modifier0 = AttributeModifier.Create(AttributeModifierKey.PhysicalDamage, DIVINE_WEAKENING_PER_STACK, ModifierType.Percentage, DIVINE_WEAKENING_DURATION);
                var modifier1 = AttributeModifier.Create(AttributeModifierKey.MagicalDamage, DIVINE_WEAKENING_PER_STACK, ModifierType.Percentage, DIVINE_WEAKENING_DURATION);
                hero.Target.GetAbility<HeroAttributes>().AddAttributeModifier(modifier0);
                hero.Target.GetAbility<HeroAttributes>().AddAttributeModifier(modifier1);
                reduceDmgModifierIds.Add(modifier0.id);
                reduceDmgModifierIds.Add(modifier1.id);
            }
        });
        customInt["sword"] = (customInt["sword"] + 1) % 2;
    }
}

public enum YoneSword {
    Divine,
    Devil
}