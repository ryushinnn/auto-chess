public class AttackProcessor_Malphite : AttackProcessor {
    public AttackProcessor_Malphite(Hero hero) : base(hero) {
        AnimationLength = 1.1f;
        Timers = new[] { 0.17f };
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