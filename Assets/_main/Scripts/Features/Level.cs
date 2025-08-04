using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Level : MonoBehaviour {
    public event Action<int> OnLevelUp;
    public LevelConfig LevelConfig { get; private set; }
    
    [SerializeField, ReadOnly] int level;
    [SerializeField, ReadOnly] int xp;
    
    public void Initialize() {
        level = 1;
        xp = 0;
        LevelConfig = GameConfigs.LEVEL_CONFIGS[0];
        ArenaUIManager.Instance.Arena.UpdateLevelText(1);
        ArenaUIManager.Instance.Arena.UpdateXpText(0, LevelConfig.xpToNextLevel);
    }
    
    [Button]
    public void GainXp(int amount) {
        xp += amount;
        ArenaUIManager.Instance.Arena.UpdateXpText(xp, LevelConfig.xpToNextLevel);
        while (xp >= LevelConfig.xpToNextLevel && level < GameConfigs.LEVEL_CONFIGS.Length) {
            LevelUp();
        }
    }
    
    void LevelUp() {
        level++;
        xp -= LevelConfig.xpToNextLevel;
        LevelConfig = GameConfigs.LEVEL_CONFIGS[level - 1];
        ArenaUIManager.Instance.Arena.UpdateLevelText(level);
        ArenaUIManager.Instance.Arena.UpdateXpText(xp, LevelConfig.xpToNextLevel);
        GameManager.Instance.LineUp.SetHeroesLimit(LevelConfig.maxHeroesOnMap);
        OnLevelUp?.Invoke(level);
    }
}