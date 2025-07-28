using System;
using System.Collections.Generic;
using RExt.Patterns.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    public event Action<int> onLevelUp; 
    public Shop Shop => shop;
    public LineUp LineUp => lineUp;
    
    [SerializeField] Hero heroPrefab;
    [SerializeField] TeamMember[] myTeam;
    [SerializeField] TeamMember[] enemyTeam;
    [SerializeField] Shop shop;
    [SerializeField] LineUp lineUp;
    [SerializeField, ReadOnly] int level;
    [SerializeField, ReadOnly] int xp;

    LevelConfig currentLevelConfig;
    
    public int Level => level;

    List<Hero> heroes = new();

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
            if (!e.active) continue;
            var trait = HeroTraitDB.Instance.Find(e.id);
            if (trait == null) {
                Debug.LogError($"no hero trait with id {e.id}");
                return;
            }
            var hero = Instantiate(heroPrefab);
            hero.name = $"{dev_count++}" + e.id;
            hero.SetNode(Map.Instance.GetNode(e.mapNode.x, e.mapNode.y));
            hero.ResetPosition(true);
            hero.Initialize(trait, TeamSide.Ally);
            foreach (var i in e.items) {
                hero.GetAbility<HeroInventory>().Add(i);
            }
            heroes.Add(hero);
        }
        
        foreach (var e in enemyTeam) {
            if (!e.active) continue;
            var trait = HeroTraitDB.Instance.Find(e.id);
            if (trait == null) {
                Debug.LogError($"no hero trait with id {e.id}");
                return;
            }
            var hero = Instantiate(heroPrefab);
            hero.name = $"{dev_count++}" + e.id + " [Enemy]";
            hero.SetNode(Map.Instance.GetNode(e.mapNode.x, e.mapNode.y));
            hero.ResetPosition(true);
            hero.Initialize(trait, TeamSide.Enemy);
            foreach (var i in e.items) {
                hero.GetAbility<HeroInventory>().Add(i);
            }
            heroes.Add(hero);
        }
    }

    [Button]
    void Pause() {
        
    }

    [Button]
    void Clear() {
        foreach (var h in heroes) {
            Destroy(h.gameObject);
        }
    }

    [Button]
    void dev_ChangeState(HeroState state) {
        heroes.ForEach(x=>x.Switch(state));
    }

    [Button]
    void dev_Battle() {
        Initialize();
        heroes.ForEach(x=>x.Switch(HeroState.ReadyToFight));
        heroes.ForEach(x=>x.Switch(HeroState.InBattle));
    }
}

[Serializable]
public class TeamMember {
    public bool active;
    [StringDropdown(typeof(HeroId))] public string id;
    public int star;
    public Vector2Int mapNode;
    public Item[] items;
}

