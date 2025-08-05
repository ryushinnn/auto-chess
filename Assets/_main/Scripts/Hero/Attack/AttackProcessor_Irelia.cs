public class AttackProcessor_Irelia : AttackProcessor {
    public AttackProcessor_Irelia(Hero hero) : base(hero) {
        AnimationLength = 1.3f;
        Timers = new[] { 0.2f };
    }
    
    public override void Process(float timer) {
        base.Process(timer);

        if (trueTimer >= Timers[0] && atkExecuted == 0) {
            if (((BattleHero)hero).Target != null) {
                var outputDmg = ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical));
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