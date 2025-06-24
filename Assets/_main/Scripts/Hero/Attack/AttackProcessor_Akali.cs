public class AttackProcessor_Akali : AttackProcessor {
    public AttackProcessor_Akali(Hero hero) : base(hero) {
        AnimationLength = 1.2f;
        Timers = new[] { 0.18f };
        Description = "Gây sát thương phép bằng (100% sát thương phép)";
    }
    
    public override void Process(float timer) {
        base.Process(timer);

        if (trueTimer >= Timers[0] && atkExecuted == 0) {
            if (hero.Target != null) {
                var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Magical));
                var heal = outputDmg * attributes.LifeSteal;
                if (heal > 0) {
                    attributes.Heal(heal);
                }
                attributes.RegenEnergy(hero.Trait.energyRegenPerAttack);
            }
            atkExecuted++;
        }
    }
}