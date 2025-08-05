using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleField : MonoBehaviour {
    [SerializeField] BattleHero heroPrefab;
    
    Dictionary<TeamSide, List<BattleHero>> aliveHeroes = new() {
        { TeamSide.Ally, new List<BattleHero>() },
        { TeamSide.Enemy, new List<BattleHero>() }
    };
    
    Dictionary<TeamSide, List<BattleHero>> deadHeroes = new() {
        { TeamSide.Ally, new List<BattleHero>() },
        { TeamSide.Enemy, new List<BattleHero>() }
    };

    Queue<BattleHero> heroPool = new();

    public void Initialize() {
        GameManager.Instance.Stages.OnChangePhase += OnChangePhase;
    }

    public BattleHero SpawnHero(HeroTrait trait, HeroRank rank, TeamSide side, MapNode node) {
        if (!heroPool.TryDequeue(out var hero)) {
            hero = Instantiate(heroPrefab);
        }
        hero.Activate();
        hero.WorldPosition = node.WorldPosition;
        hero.SetData(trait, rank, side);
        aliveHeroes[side].Add(hero);
        return hero;
    }

    public BattleHero GetNearestOpponent(BattleHero hero) {
        var opponents = aliveHeroes[hero.Side == TeamSide.Ally ? TeamSide.Enemy : TeamSide.Ally];
        BattleHero nearestHero = null;
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
    
    public BattleHero GetFurthestOpponent(BattleHero hero) {
        var opponents = aliveHeroes[hero.Side == TeamSide.Ally ? TeamSide.Enemy : TeamSide.Ally];
        BattleHero furthestHero = null;
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

    public BattleHero GetRandomOpponent(BattleHero hero) {
        var opponents = aliveHeroes[hero.Side == TeamSide.Ally ? TeamSide.Enemy : TeamSide.Ally];
        return opponents.Count == 0 ? null : opponents[Random.Range(0, opponents.Count)];
    }

    public void MarkHeroAsDead(BattleHero hero) {
        if (aliveHeroes[hero.Side].Remove(hero)) {
            deadHeroes[hero.Side].Add(hero);
            
            if (aliveHeroes[TeamSide.Ally].Count == 0) {
                GameManager.Instance.Stages.EndBattlePhase(MatchResult.Lose);
            }
            else if (aliveHeroes[TeamSide.Enemy].Count == 0) {
                GameManager.Instance.Stages.EndBattlePhase(MatchResult.Win);
            }
        }

    }

    void OnChangePhase(MatchPhase phase) {
        switch (phase) {
            case MatchPhase.Preparation:
                foreach (var (_, heroes) in aliveHeroes) {
                    DeactivateHeroes(heroes);
                }
                foreach (var (_, heroes) in deadHeroes) {
                    DeactivateHeroes(heroes);
                }
                break;
            
            case MatchPhase.Battle:
                var enemies = GameManager.Instance.Stages.GetEnemies();
                foreach (var e in enemies) {
                    var trait = HeroTraitDB.Instance.Find(e.heroId);
                    var node = Map.Instance.GetNode(e.gridPosition);
                    SpawnHero(trait, e.rank, TeamSide.Enemy, node);
                }
                break;
        }
    }

    void DeactivateHeroes(List<BattleHero> heroes) {
        for (int i = heroes.Count - 1; i >= 0; i--) {
            var hero = heroes[i];
            heroPool.Enqueue(hero);
            hero.Deactivate();
            heroes.RemoveAt(i);
        }
    }
}