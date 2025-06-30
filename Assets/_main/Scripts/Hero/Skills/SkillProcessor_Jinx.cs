using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

public class SkillProcessor_Jinx : SkillProcessor {
    public const int ROCKETS = 5;
    public const int INTERVAL = 200;
    const float DMG_MUL_PER_ROCKET = 0.5f;
    const float PENETRATION = 0.5f;

    List<Hero> affectedTargets = new();
    
    public SkillProcessor_Jinx(Hero hero) : base(hero) {
        AnimationLength = 4.2f;
        Timers = new[] { 2f };
        Unstoppable = true;
        Description = "Bắn 5 quả tên lửa, mỗi quả gây sát thương bằng " +
                      $"({DMG_MUL_PER_ROCKET * 100}% sát thương vật lý), " +
                      $"có thể chí mạng. Sát thương đầu ra ngẫu nhiên là " +
                      $"sát thương vật lý, sát thương phép hoặc sát thương chuẩn. " +
                      $"Nếu là sát thương vật lý thì sẽ kèm {PENETRATION * 100}% xuyên giáp. " +
                      $"Nếu là sát thương phép thì sẽ kèm {PENETRATION * 100}% xuyên kháng.\n" +
                      $"Trong thời gian sử dụng kỹ năng, không thể bị cản phá.";
    }

    public override void Process(float timer) {
        if (timer >= Timers[0] && skillExecuted == 0) {
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
        if (hero.Target == null) return;

        var type = Random.Range(0, 3);
        Damage dmg = default;
        switch (type) {
            case 0:
                dmg = attributes.GetDamage(DamageType.Physical, attributes.Crit(),
                    scaledValues: new[] { (DMG_MUL_PER_ROCKET, DamageType.Physical) });
                dmg.penetration = PENETRATION;
                break;
            
            case 1:
                dmg = attributes.GetDamage(DamageType.Magical, attributes.Crit(),
                    scaledValues: new[] { (DMG_MUL_PER_ROCKET, DamageType.Physical) });
                dmg.penetration = PENETRATION;
                break;
            
            case 2:
                dmg = attributes.GetDamage(DamageType.True, attributes.Crit(),
                    scaledValues: new[] { (DMG_MUL_PER_ROCKET, DamageType.Physical) });
                break;
        }

        var isNewTarget = !affectedTargets.Contains(hero.Target);
        
        hero.Target.GetAbility<HeroAttributes>().TakeDamage(dmg, isNewTarget);

        if (isNewTarget) {
            affectedTargets.Add(hero.Target);
        }
    }
}