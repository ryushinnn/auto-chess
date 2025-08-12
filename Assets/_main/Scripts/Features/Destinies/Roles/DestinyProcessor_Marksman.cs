using System;
using System.Collections.Generic;
using System.Linq;
using RExt.Extensions;

public class DestinyProcessor_Marksman : DestinyProcessor {
    readonly float[] ATK_SPD_BONUSES = { 0.2f, 0.4f, 0.6f };
    const int BURST_DURATION = 60;
    
    public DestinyProcessor_Marksman() : base() {
        var stages = GameConfigs.ROLE_CONFIGS[Role.Marksman];

        Description = "Tất cả xạ thủ nhận thêm tốc độ đánh. Mỗi khi một xạ thủ tử trận, tất cả xạ thủ còn sống " +
                      $"nhận thêm lượng tốc độ đánh đó trong {BURST_DURATION}s (có thể cộng dồn).\n\n" +
                      $"- Mốc {stages[0]}: {ATK_SPD_BONUSES[0]}<sprite name=aspd>\n" +
                      $"- Mốc {stages[2]}: {ATK_SPD_BONUSES[1]}<sprite name=aspd>\n" +
                      $"- Mốc {stages[1]}: {ATK_SPD_BONUSES[2]}<sprite name=aspd>\n";
    }
    
    public override void Activate(List<BattleHero> heroes, int stage) {
        var marksmen = heroes.Where(x => x.Side == TeamSide.Ally && x.Trait.role.Has(Role.Marksman)).ToArray();
        foreach (var hero in marksmen) {
            var attribute = hero.GetAbility<HeroAttributes>();
            attribute.AddAttributeModifier(
                AttributeModifierSet.Create(
                    hero,
                    "marksman",
                    new[] {
                        (AttributeModifierKey.AttackSpeed, ATK_SPD_BONUSES[stage], AttributeModifier.Type.Percentage)
                    },
                    createMark: false));

            attribute.OnDeath += () => {
                foreach (var h in marksmen) {
                    if (h == hero) continue;
                    var att = h.GetAbility<HeroAttributes>();
                    if (!att.IsAlive) continue;
                        
                    att.AddAttributeModifier(
                        AttributeModifierSet.Create(
                            hero,
                            "marksman_burst",
                            BURST_DURATION,
                            new[] {
                                (AttributeModifierKey.AttackSpeed, ATK_SPD_BONUSES[stage], AttributeModifier.Type.Percentage)
                            },
                            createMark: false));
                }
            };
        }
    }
}