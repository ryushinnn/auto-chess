using System;
using UnityEngine;

public class Skill_Yasuo : Skill {
    Hero hero;

    const float DMG_MUL = 1.5f;
    const int RANGE = 1;
    
    public Skill_Yasuo(Hero hero) {
        this.hero = hero;

        events = new Action[]{SpinSlash};
        unstoppable = false;
    }

    void SpinSlash() {
        var affectedNodes = Map.Instance.GetAdjacentNodes(hero.MapNode.X, hero.MapNode.Y, RANGE);
        foreach (var node in affectedNodes) {
            if (node.objects.Count > 0) {
                foreach (var obj in node.objects) {
                    if (obj is Hero target) {
                        target.GetAbility<HeroAttributes>().TakeDamage(
                            hero.GetAbility<HeroAttributes>().PhysicalDamage * DMG_MUL, 
                            DamageType.Physical, 
                            hero.GetAbility<HeroAttributes>().PhysicalPenetration);
                    }
                }
            }
        }
    }
}