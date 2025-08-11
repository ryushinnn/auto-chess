public class AttackProcessor_Ashe : AttackProcessor {
    public AttackProcessor_Ashe(Hero hero) : base(hero) {
        AnimationLength = 1f;
        Timers = new[] { 0.2f };
    }

    public override void Process(float timer) {
        base.Process(timer);

        if (trueTimer >= Timers[0] && atkExecuted == 0) {
            if (((BattleHero)hero).Target != null) {
                var arrow = VfxPool.Instance.GetVfx<Projectile>("ashe_attack");
                arrow.SetData((BattleHero)hero, ((BattleHero)hero).Target, () => {
                    var outputDmg = ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(attributes.GetDamage(DamageType.Physical));
                    var heal = outputDmg * attributes.LifeSteal;
                    if (heal > 0) {
                        attributes.Heal(heal);
                    }
                    attributes.RegenEnergy(hero.Trait.energyRegenPerAttack);
                });
            }
            atkExecuted++;
        }
    }
}