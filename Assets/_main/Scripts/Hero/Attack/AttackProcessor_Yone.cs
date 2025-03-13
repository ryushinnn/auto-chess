using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// thay doi qua lai giua than kiem va quy kiem sau moi don danh
/// don danh than kiem +1 stack suy yeu: giam 5% st cua muc tieu trong 2s, toi da 3 stack
/// don danh quy kiem cho toi da 20% hut mau, dua vao mau da mat
/// thanh kiem cuoi cung duoc su dung cung quyet dinh skill la gi
/// </summary>
public class AttackProcessor_Yone : AttackProcessor {
    public enum Sword {
        Divine,
        Devil
    }
    
    public Sword CurrentSword => sword;
    
    Sword sword;
    List<string> reduceDmgModifierIds = new List<string>();
    
    const float DIVINE_WEAKENING_PER_STACK = -0.05f;
    const int DIVINE_WEAKENING_MAX_STACK = 3;
    const int DIVINE_WEAKENING_DURATION = 2;
    const float DEVIL_VAMP_MIN = 0f;
    const float DEVIL_VAMP_MAX = 0.2f;

    public AttackProcessor_Yone(Hero hero) : base(hero) {
        this.hero = hero;
    }

    public override void Execute() {
        CalculateDamage(out var dmg, out var type, out var pen);
        var attributes = hero.GetAbility<HeroAttributes>();
        hero.Mecanim.Attack(() => {
            if (hero.Target == null) return;
            var outputDamage = hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg, type, pen);
            var healAmount = outputDamage * attributes.LifeSteal;
            if (sword == Sword.Devil) {
                healAmount += outputDamage * ((attributes.Hp / attributes.MaxHp) * (DEVIL_VAMP_MAX - DEVIL_VAMP_MIN) + DEVIL_VAMP_MIN);
            }
            attributes.Heal(healAmount);

            if (sword == Sword.Divine) {
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
        sword = (Sword)(((int)sword + 1) % 2);
    }
}