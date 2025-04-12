using System;
using System.Collections.Generic;
using System.Linq;
using RExt.Extension;
using Sirenix.OdinInspector;
using UnityEngine;

public class LineUp : MonoBehaviour {
    [SerializeField] Hero heroPrefab;
    [SerializeField, ReadOnly] List<Hero> heroes;

    Dictionary<Role, int> roleNumbers = new();
    Dictionary<Realm, int> realmNumbers = new();
    HashSet<HeroTrait> uniqueTraits = new();

    int heroesOnMap;
    int maxHeroesOnMap;
    
    void Awake() {
        Initialize();
    }

    void Start() {
        heroesOnMap = 0;
        SetHeroesLimit(1);
        ArenaUIManager.Instance.LineUp.Initialize(roleNumbers, realmNumbers);
    }

    void Initialize() {
        var allRoles = ((Role[])Enum.GetValues(typeof(Role))).Where(x => x != 0).ToArray();
        var allRealms = ((Realm[])Enum.GetValues(typeof(Realm))).Where(x => x != 0).ToArray();
        foreach (var role in allRoles) {
            roleNumbers.Add(role, 0);
        }
        foreach (var realm in allRealms) {
            realmNumbers.Add(realm, 0);
        }
    }

    public void SetHeroesLimit(int value) {
        maxHeroesOnMap = value;
        ArenaUIManager.Instance.Arena.UpdateLineUpText(heroesOnMap, maxHeroesOnMap);
    }

    public bool Add(HeroTrait trait) {
        // merge into rank A hero
        if (MergeHeroes(trait, HeroRank.B, GameConfigs.NUMBER_OF_HEROES_TO_LEVEL_UP - 1)) {
            // merge into rank S hero
            if (MergeHeroes(trait, HeroRank.A, GameConfigs.NUMBER_OF_HEROES_TO_LEVEL_UP)) {
                return true;
            }

            return true;
        }

        var availableDeckNode = Deck.Instance.GetLowestAvailableNode();
        if (availableDeckNode != null) {
            // create new hero
            var hero = Instantiate(heroPrefab);
            hero.Initialize(trait, TeamSide.Ally);
            hero.SetNode(availableDeckNode);
            hero.ResetPosition(true);
            heroes.Add(hero);
            RecalculateHeroesOnMap();

            if (uniqueTraits.Add(hero.Trait)) {
                var roles = hero.Trait.role.GetAllFlags().Where(x => x != 0).ToArray();
                foreach (var role in roles) {
                    roleNumbers[role]++;
                }
                realmNumbers[hero.Trait.realm]++;
                ArenaUIManager.Instance.LineUp.Initialize(roleNumbers, realmNumbers);
            }
            
            return true;
        }

        return false;
    }

    public void Remove(string id) {
        var hero = heroes.Find(x => x.ID == id);
        heroes.Remove(hero);
        Destroy(hero.gameObject);
    }

    public void Remove(Hero hero) {
        hero.DeleteNode();
        heroes.Remove(hero);
        Destroy(hero.gameObject);
    }
    
    public void RecalculateHeroesOnMap() {
        heroesOnMap = heroes.Count(x => x.MNode != null);
        ArenaUIManager.Instance.Arena.UpdateLineUpText(heroesOnMap, maxHeroesOnMap);
    }

    bool MergeHeroes(HeroTrait trait, HeroRank rank, int required) {
        var duplicates = heroes.FindAll(x => x.Trait == trait && x.Rank == rank);
        if (duplicates.Count < required) return false;

        for (int i = required - 1; i >= 1; i--) {
            Remove(duplicates[i]);
        }
        duplicates[0].Upgrade();
        RecalculateHeroesOnMap();
        return true;
    }

    [Button]
    void dev_checkAllNumber() {
        foreach (var it in roleNumbers) {
            Debug.Log($"{it.Key}:{it.Value}");
        }
        foreach (var it in realmNumbers) {
            Debug.Log($"{it.Key}:{it.Value}");
        }
    }
}

public enum TeamSide {
    Ally,
    Enemy
}