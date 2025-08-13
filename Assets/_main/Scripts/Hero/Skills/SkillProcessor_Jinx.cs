using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

public class SkillProcessor_Jinx : SkillProcessor {
    public const int ROCKETS = 5;
    public const int INTERVAL = 200;
    const float DMG_MUL_PER_ROCKET = 0.5f;

    List<Hero> affectedTargets = new();
    
    public SkillProcessor_Jinx(BattleHero hero) : base(hero) {
        animationLength = 4.2f;
        timers = new[] { 2f };
        unstoppable = true;
        Name = "Bùm! Bùm! Bùm! Bùm! Bùm!";
        Description = $"Bắn 5 quả tên lửa, mỗi quả gây ({DMG_MUL_PER_ROCKET * 100}% <sprite name=pdmg>) sát thương, " +
                      $"Sát thương đầu ra ngẫu nhiên là sát thương vật lý, sát thương phép hoặc sát thương chuẩn " +
                      $"và có thể chí mạng.";
    }

    public override void Process(float timer) {
        if (timer >= timers[0] && skillExecuted == 0) {
            ShotRockets();
            skillExecuted++;
        }
    }

    async void ShotRockets() {
        affectedTargets.Clear();
        ShotRocket();
        for (int i = 1; i < ROCKETS; i++) {
            await Task.Delay(INTERVAL);
            ShotRocket();
        }
    }

    void ShotRocket() {
        if (((BattleHero)hero).Target == null) return;

        var type = Random.Range(0, 3);
        Damage dmg = default;
        switch (type) {
            case 0:
                dmg = attributes.GetDamage(DamageType.Physical, attributes.Crit(),
                    scaledValues: new[] { (DMG_MUL_PER_ROCKET, DamageType.Physical) });
                break;
            
            case 1:
                dmg = attributes.GetDamage(DamageType.Magical, attributes.Crit(),
                    scaledValues: new[] { (DMG_MUL_PER_ROCKET, DamageType.Physical) });
                break;
            
            case 2:
                dmg = attributes.GetDamage(DamageType.True, attributes.Crit(),
                    scaledValues: new[] { (DMG_MUL_PER_ROCKET, DamageType.Physical) });
                break;
        }

        var isNewTarget = !affectedTargets.Contains(((BattleHero)hero).Target);
        
        ((BattleHero)hero).Target.GetAbility<HeroAttributes>().TakeDamage(dmg, isNewTarget);

        if (isNewTarget) {
            affectedTargets.Add(((BattleHero)hero).Target);
        }
    }
}