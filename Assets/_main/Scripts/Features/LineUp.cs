using System;
using System.Collections.Generic;
using System.Linq;
using RExt.Extension;
using Sirenix.OdinInspector;
using UnityEngine;

public class LineUp : MonoBehaviour {
    [SerializeField] Hero heroPrefab;
    [SerializeField, ReadOnly] List<Hero> heroes;

    Dictionary<Role, int> roleStages = new();
    Dictionary<Realm, int> realmStages = new();
    HashSet<HeroTrait> uniqueTraits = new();
    
    void Awake() {
        Initialize();
    }

    void Start() {
        ArenaUIManager.Instance.LineUp.Initialize(roleStages, realmStages);
    }

    void Initialize() {
        var allRoles = ((Role[])Enum.GetValues(typeof(Role))).Where(x => x != 0).ToArray();
        var allRealms = ((Realm[])Enum.GetValues(typeof(Realm))).Where(x => x != 0).ToArray();
        foreach (var role in allRoles) {
            roleStages.Add(role, 0);
        }
        foreach (var realm in allRealms) {
            realmStages.Add(realm, 0);
        }
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

            if (uniqueTraits.Add(hero.Trait)) {
                var roles = hero.Trait.role.GetAllFlags().Where(x => x != 0).ToArray();
                foreach (var role in roles) {
                    roleStages[role]++;
                }
                realmStages[hero.Trait.realm]++;
                ArenaUIManager.Instance.LineUp.Initialize(roleStages, realmStages);
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

    bool MergeHeroes(HeroTrait trait, HeroRank rank, int required) {
        var duplicates = heroes.FindAll(x => x.Trait == trait && x.Rank == rank);
        if (duplicates.Count < required) return false;

        for (int i = required - 1; i >= 1; i--) {
            Remove(duplicates[i]);
        }
        duplicates[0].Upgrade();
        return true;
    }

    [Button]
    void dev_checkAllStages() {
        foreach (var it in roleStages) {
            Debug.Log($"{it.Key}:{it.Value}");
        }
        foreach (var it in realmStages) {
            Debug.Log($"{it.Key}:{it.Value}");
        }
    }
}

public enum TeamSide {
    Ally,
    Enemy
}