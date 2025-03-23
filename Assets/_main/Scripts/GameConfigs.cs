public static class GameConfigs {
    public const int MAX_LEVEL = 3;
    public const int NUMBER_OF_HEROES_TO_LEVEL_UP = 3;
    public const float STATS_MUL_UPON_LV2 = 2;
    public const float STATS_MUL_UPON_LV3 = 5;
    public const int XP_GAIN_PER_ROUND = 2;
    public static readonly LevelConfig[] LEVEL_CONFIGS = {
        new() {
            level = 1,
            xpToNextLevel = 2,
            heroSlot = 1,
            rates = new[] {90, 10, 0}
        },
        new() {
            level = 2,
            xpToNextLevel = 4,
            heroSlot = 1,
            rates = new[] {70, 29, 1}
        },
        new() {
            level = 3,
            xpToNextLevel = 8,
            heroSlot = 1,
            rates = new[] {50, 47, 3}
        },
        new() {
            level = 4,
            xpToNextLevel = 16,
            heroSlot = 1,
            rates = new[] {30, 65, 5}
        },
        new() {
            level = 5,
            xpToNextLevel = 32,
            heroSlot = 1,
            rates = new[] {20, 70, 10}
        },
        new() {
            level = 6,
            xpToNextLevel = 64,
            heroSlot = 1,
            rates = new[] {10, 70, 20}
        },
        new() {
            level = 7,
            xpToNextLevel = 128,
            heroSlot = 1,
            rates = new[] {1, 64, 35}
        },
        new() {
            level = 8,
            xpToNextLevel = 256,
            heroSlot = 1,
            rates = new[] {0, 40, 60}
        },
        new() {
            level = 9,
            xpToNextLevel = 0,
            heroSlot = 1,
            rates = new[] {0, 1, 99}
        }
    };

    public static readonly int[] HERO_PRICES = new[] { 1, 3, 5 };
    public const int REFRESH_COST = 2;
}

public class LevelConfig {
    public int level;
    public int xpToNextLevel;
    public int heroSlot;
    public int[] rates;
}