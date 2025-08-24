using System;
using System.Collections.Generic;
using System.Linq;
using RExt.Extensions;

public class DestinyProcessor_Marksman : DestinyProcessor {
    readonly float[] atkSpdBonuses;
    readonly float burstDuration;
    
    public DestinyProcessor_Marksman(DestinyConfig cfg) : base(cfg) {
        var destinyParams = cfg.destinyParams;

        atkSpdBonuses = new[] { destinyParams[0].value, destinyParams[1].value, destinyParams[2].value };
        burstDuration = destinyParams[3].value;
    }
    
    public override void Activate(List<BattleHero> heroes, int checkpointIndex) {
        var marksmen = heroes.Where(x => x.Side == TeamSide.Ally && x.Trait.role.Has(Role.Marksman)).ToArray();
        foreach (var hero in marksmen) {
            var attribute = hero.GetAbility<HeroAttributes>();
            attribute.AddAttributeModifier(
                AttributeModifierSet.Create(
                    hero,
                    "marksman",
                    new[] {
                        (AttributeModifierKey.AttackSpeed, atkSpdBonuses[checkpointIndex], AttributeModifier.Type.Percentage)
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
                            burstDuration,
                            new[] {
                                (AttributeModifierKey.AttackSpeed, atkSpdBonuses[checkpointIndex], AttributeModifier.Type.Percentage)
                            },
                            createMark: false));
                }
            };
        }
    }
}