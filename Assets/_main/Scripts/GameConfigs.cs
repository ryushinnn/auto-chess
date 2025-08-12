using System.Collections.Generic;

public static class GameConfigs {
    public const int NUMBER_OF_HEROES_TO_RANK_UP = 3;
    public const int XP_GAIN_PER_MATCH = 2;
    public const int XP_GAIN_PER_PURCHASE = 4;
    public static readonly LevelConfig[] LEVEL_CONFIGS = {
        new() {
            // level 1
            xpToNextLevel = 2,
            maxHeroesOnMap = 1,
            rates = new[] {100, 0, 0}
        },
        new() {
            // level 2
            xpToNextLevel = 4,
            maxHeroesOnMap = 2,
            rates = new[] {90, 10, 0}
        },
        new() {
            // level 3
            xpToNextLevel = 8,
            maxHeroesOnMap = 3,
            rates = new[] {79, 20, 1}
        },
        new() {
            // level 4
            xpToNextLevel = 16,
            maxHeroesOnMap = 4,
            rates = new[] {67, 30, 3}
        },
        new() {
            // level 5
            xpToNextLevel = 32,
            maxHeroesOnMap = 5,
            rates = new[] {50, 45, 5}
        },
        new() {
            // level 6
            xpToNextLevel = 64,
            maxHeroesOnMap = 6,
            rates = new[] {30, 60, 10}
        },
        new() {
            // level 7
            xpToNextLevel = 128,
            maxHeroesOnMap = 7,
            rates = new[] {15, 70, 15}
        },
        new() {
            // level 8
            xpToNextLevel = 256,
            maxHeroesOnMap = 8,
            rates = new[] {5, 65, 30}
        },
        new() {
            // level 9
            xpToNextLevel = -1,
            maxHeroesOnMap = 9,
            rates = new[] {1, 49, 50}
        }
    };

    public static readonly Dictionary<Reputation, int> HERO_PRICES = new() {
        { Reputation.Unknown, 1 },
        { Reputation.Elite, 3 },
        { Reputation.Legendary, 5 },
    };
    public const int REFRESH_COST = 2;

    public static readonly Dictionary<Role,int[]> ROLE_CONFIGS = new() {
        { Role.Duelist, new[] { 2, 3, 4 } },
        { Role.Sorcerer, new[] { 2, 3, 4 } },
        { Role.Marksman, new[] { 2, 3, 4 } },
        { Role.Assassin, new[] { 2, 3, 4 } },
        { Role.Bruiser, new[] { 2, 3, 4 } },
        { Role.Cultist, new[] { 2, 3, 4 } },
    };
    
    public static readonly Dictionary<Realm,int[]> REALM_CONFIGS = new() {
        { Realm.Mortal, new[] { 2, 3, 4, 5} },
        { Realm.Divine, new[] { 2, 3, 4, 5} },
        { Realm.Nether, new[] { 2, 3, 4, 5} },
        { Realm.Mecha, new[] { 2, 3, 4, 5} },
        { Realm.Chaos, new[] { 2, 3, 4, 5} }
    };

    public static readonly Dictionary<MatchPhase, int> MATCH_PHASE_DURATIONS = new() {
        { MatchPhase.Preparation, 999 },
        { MatchPhase.Transition, 5 },
        { MatchPhase.Battle, 999 },
        { MatchPhase.Summary, 2 },
    };

    public const float SPAWN_ENEMIES_DELAY = 2f;
    public const float PORTAL_CLOSE_DELAY = 1f;
    
    public static readonly Dictionary<HeroRank,float> HEROES_SCALE = new() {
        { HeroRank.B, 1 },
        { HeroRank.A, 1.15f },
        { HeroRank.S, 1.4f },
    };
}

public class LevelConfig {
    public int xpToNextLevel;
    public int maxHeroesOnMap;
    public int[] rates;
}

public class RoleConfig {
    public Role role;
    public int[] stages;
}

public class RealmConfig {
    public Realm realm;
    public int[] stages;
}