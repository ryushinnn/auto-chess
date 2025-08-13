public class AttackProcessor_Aatrox_Light : AttackProcessor {
    public AttackProcessor_Aatrox_Light(BattleHero hero) : base(hero) {
        AnimationLength = 1.367f;
        Timers = new[] { 0.18f };
    }

    public override void Process(float timer) {
        base.Process(timer);

        if (trueTimer >= Timers[0] && atkExecuted == 0) {
            if (((BattleHero)hero).Target != null) {
                var outputDmg = ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Magical));
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