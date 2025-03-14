using System;

/// <summary>
/// nem bom mu vao 1 muc tieu
/// neu muc tieu co nhieu hon hoac bang 50 nang luong, gay cam lang 2s
/// neu muc tieu co it hon 50 nang luong, gay choang 1s,
/// </summary>
public class SkillProcessor_Akali : SkillProcessor {
    const float ENERGY_THRESHOLD = 50;
    const float SILENT_DURATION = 2;
    const float STUN_DURATION = 1;
    
    public SkillProcessor_Akali(Hero hero) : base(hero) {
        this.hero = hero;
        events = new Action[]{ThrowBomb};
        unstoppable = false;
    }

    void ThrowBomb() {
        if (hero.Target == null) return;

        if (hero.Target.GetAbility<HeroAttributes>().Energy >= ENERGY_THRESHOLD) {
            hero.Target.GetAbility<HeroStatusEffects>().Silent(SILENT_DURATION);
        }
        else {
            hero.Target.GetAbility<HeroStatusEffects>().Stun(STUN_DURATION);
        }
    }
}