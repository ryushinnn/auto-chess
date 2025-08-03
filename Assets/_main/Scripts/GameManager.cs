using System;
using System.Collections.Generic;
using RExt.Patterns.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    public event Action<int> onLevelUp; 
    public Shop Shop => shop;
    public LineUp LineUp => lineUp;
    public BattleField BattleField => battleField;
    
    [SerializeField] Hero heroPrefab;
    [SerializeField] TeamMember[] myTeam;
    [SerializeField] TeamMember[] enemyTeam;
    [SerializeField] Shop shop;
    [SerializeField] LineUp lineUp;
    [SerializeField] BattleField battleField;
    [SerializeField, ReadOnly] int level;
    [SerializeField, ReadOnly] int xp;

    LevelConfig currentLevelConfig;
    
    public int Level => level;

    // List<Hero> heroes = new();

    int dev_count = 0;

    void Start() {
        level = 1;
        xp = 0;
        currentLevelConfig = GameConfigs.LEVEL_CONFIGS[0];
        ArenaUIManager.Instance.Arena.UpdateLevelText(1);
        ArenaUIManager.Instance.Arena.UpdateXpText(0, currentLevelConfig.xpToNextLevel);
    }

    [Button]
    public void GainXP(int amount) {
        xp += amount;
        ArenaUIManager.Instance.Arena.UpdateXpText(xp, currentLevelConfig.xpToNextLevel);
        while (xp >= currentLevelConfig.xpToNextLevel && level < GameConfigs.LEVEL_CONFIGS.Length) {
            LevelUp();
        }
    }
    
    void LevelUp() {
        level++;
        xp -= currentLevelConfig.xpToNextLevel;
        currentLevelConfig = GameConfigs.LEVEL_CONFIGS[level - 1];
        ArenaUIManager.Instance.Arena.UpdateLevelText(level);
        ArenaUIManager.Instance.Arena.UpdateXpText(xp, currentLevelConfig.xpToNextLevel);
        lineUp.SetHeroesLimit(currentLevelConfig.maxHeroesOnMap);
        onLevelUp?.Invoke(level);
    }
    
    [Button]
    void Initialize() {
        foreach (var e in myTeam) {
            var trait = HeroTraitDB.Instance.Find(e.id);
            if (trait == null) {
                Debug.LogError($"no hero trait with id {e.id}");
                return;
            }

            var node = Map.Instance.GetNode(e.mapNode.x, e.mapNode.y);
            var hero = battleField.SpawnHero(trait, TeamSide.Ally, node);
            foreach (var i in e.items) {
                hero.GetAbility<HeroInventory>().Add(i);
            }
        }
        
        foreach (var e in enemyTeam) {
            var trait = HeroTraitDB.Instance.Find(e.id);
            if (trait == null) {
                Debug.LogError($"no hero trait with id {e.id}");
                return;
            }

            var node = Map.Instance.GetNode(e.mapNode.x, e.mapNode.y);
            var hero = battleField.SpawnHero(trait, TeamSide.Enemy, node);
            foreach (var i in e.items) {
                hero.GetAbility<HeroInventory>().Add(i);
            }
        }
    }

    [Button]
    void Pause() {
        
    }
}

[Serializable]
public class TeamMember {
    [StringDropdown(typeof(HeroId))] public string id;
    public int star;
    public Vector2Int mapNode;
    public Item[] items;
}

