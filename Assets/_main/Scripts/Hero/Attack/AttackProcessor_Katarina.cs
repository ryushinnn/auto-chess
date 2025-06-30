public class AttackProcessor_Katarina : AttackProcessor {
    public AttackProcessor_Katarina(Hero hero) : base(hero) {
        AnimationLength = 1.067f;
        Timers = new[] { 0.14f };
        Description = "Gây sát thương vật lý bằng (100% sát thương vật lý)";
    }

    public override void Process(float timer) {
        base.Process(timer);

        if (trueTimer >= Timers[0] && atkExecuted == 0) {
            if (hero.Target != null) {
                var outputDmg = hero.Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical));
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