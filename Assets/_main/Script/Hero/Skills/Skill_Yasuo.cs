using System;
using UnityEngine;

public class Skill_Yasuo : Skill {
    Hero hero;
    
    public Skill_Yasuo(Hero hero) {
        this.hero = hero;

        events = new Action[]{SpinSlash};
        unstoppable = false;
    }

    void SpinSlash() {
        var affectedNodes = Map.Instance.GetAdjacentNodes(hero.MapNode.X, hero.MapNode.Y, 1);
        foreach (var node in affectedNodes) {
            if (node.objects.Count > 0) {
                foreach (var obj in node.objects) {
                    if (obj is Hero target) {
                        target.GetAbility<HeroAttributes>().TakeDamage(100, DamageType.Physical, 0);
                    }
                }
            }
        }
    }
}