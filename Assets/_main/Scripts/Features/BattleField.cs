using System.Collections.Generic;
using UnityEngine;

public class BattleField : MonoBehaviour {
    [SerializeField] Hero heroPrefab;
    
    Dictionary<TeamSide, List<Hero>> aliveHeroes = new() {
        { TeamSide.Ally, new List<Hero>() },
        { TeamSide.Enemy, new List<Hero>() }
    };
    
    Dictionary<TeamSide, List<Hero>> deadHeroes = new() {
        { TeamSide.Ally, new List<Hero>() },
        { TeamSide.Enemy, new List<Hero>() }
    };

    public Hero SpawnHero(HeroTrait trait, TeamSide side, MapNode node) {
        var hero = Instantiate(heroPrefab);
        hero.Initialize(trait, side);
        node.ChangeState(NodeState.Occupied);
        hero.UpdatePosition(node,true);
        aliveHeroes[side].Add(hero);
        return hero;
    }

    public Hero GetNearestOpponent(Hero hero) {
        var opponents = aliveHeroes[hero.Side == TeamSide.Ally ? TeamSide.Enemy : TeamSide.Ally];
        Hero nearestHero = null;
        var minDist = Mathf.Infinity;
        foreach (var h in opponents) {
            var dist = Vector3.Distance(hero.transform.position, h.transform.position);
            if (dist < minDist) {
                nearestHero = h;
                minDist = dist;
            }
        }

        return nearestHero;
    }
    
    public Hero GetFurthestOpponent(Hero hero) {
        var opponents = aliveHeroes[hero.Side == TeamSide.Ally ? TeamSide.Enemy : TeamSide.Ally];
        Hero furthestHero = null;
        var minDist = Mathf.NegativeInfinity;
        foreach (var h in opponents) {
            var dist = Vector3.Distance(hero.transform.position, h.transform.position);
            if (dist > minDist) {
                furthestHero = h;
                minDist = dist;
            }
        }

        return furthestHero;
    }

    public Hero GetRandomOpponent(Hero hero) {
        var opponents = aliveHeroes[hero.Side == TeamSide.Ally ? TeamSide.Enemy : TeamSide.Ally];
        return opponents.Count == 0 ? null : opponents[Random.Range(0, opponents.Count)];
    }

    public void MarkHeroAsDead(Hero hero) {
        if (aliveHeroes[hero.Side].Remove(hero)) {
            deadHeroes[hero.Side].Add(hero);
        }
    }
}