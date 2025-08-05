using System;
using System.Collections.Generic;
using RExt.Patterns.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    public Shop Shop => shop;
    public LineUp LineUp => lineUp;
    public BattleField BattleField => battleField;
    public Level Level => level;
    public Stages Stages => stages;
    
    [SerializeField] TeamMember[] myTeam;
    [SerializeField] TeamMember[] enemyTeam;
    
    [SerializeField] Shop shop;
    [SerializeField] LineUp lineUp;
    [SerializeField] BattleField battleField;
    [SerializeField] Level level;
    [SerializeField] Stages stages;

    List<Hero> test_heroes = new();

    void Start() {
        MapVisual.Instance.Initialize();
        Map.Instance.Initialize();
        level.Initialize();
        lineUp.Initialize();
        battleField.Initialize();
        // rounds.Initialize();
        
        // dev_Battle();
    }

    void dev_spawn() {
        foreach (var e in myTeam) {
            var trait = HeroTraitDB.Instance.Find(e.id);
            if (trait == null) {
                Debug.LogError($"no hero trait with id {e.id}");
                return;
            }

            var node = Map.Instance.GetNode(e.mapNode.x, e.mapNode.y);
            var hero = battleField.SpawnHero(trait, HeroRank.B, TeamSide.Ally, node);
            foreach (var i in e.items) {
                hero.GetAbility<HeroInventory>().Add(i);
            }
            
            test_heroes.Add(hero);
        }
        
        foreach (var e in enemyTeam) {
            var trait = HeroTraitDB.Instance.Find(e.id);
            if (trait == null) {
                Debug.LogError($"no hero trait with id {e.id}");
                return;
            }

            var node = Map.Instance.GetNode(e.mapNode.x, e.mapNode.y);
            var hero = battleField.SpawnHero(trait, HeroRank.B,TeamSide.Enemy, node);
            foreach (var i in e.items) {
                hero.GetAbility<HeroInventory>().Add(i);
            }
            
            test_heroes.Add(hero);
        }
    }
    
    [Button]
    void dev_Battle() {
        dev_spawn();
    }
}

[Serializable]
public class TeamMember {
    [StringDropdown(typeof(HeroId))] public string id;
    public Vector2Int mapNode;
    public Item[] items;
}

