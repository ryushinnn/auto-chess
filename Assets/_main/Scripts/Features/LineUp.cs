using System;
using System.Collections.Generic;
using System.Linq;
using RExt.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

public class LineUp : MonoBehaviour {
    [SerializeField] LineUpHero heroPrefab;

    Dictionary<Role, int> roleNumbers = new();
    Dictionary<Realm, int> realmNumbers = new();
    HashSet<HeroTrait> uniqueTraits = new();

    Dictionary<LineUpHero,Node> heroes = new();
    Queue<LineUpHero> heroPool = new();
    
    public bool Full => heroesOnMap >= maxHeroesOnMap;

    int heroesOnMap;
    int maxHeroesOnMap;

    void Start() {
        heroesOnMap = 0;
        SetHeroesLimit(1);
        ArenaUIManager.Instance.LineUp.Initialize(roleNumbers, realmNumbers);
    }

    public void Initialize() {
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
        if (MergeHeroes(trait, HeroRank.B, GameConfigs.NUMBER_OF_HEROES_TO_RANK_UP - 1)) {
            // merge into rank S hero
            if (MergeHeroes(trait, HeroRank.A, GameConfigs.NUMBER_OF_HEROES_TO_RANK_UP)) {
                return true;
            }

            return true;
        }

        var availableDeckNode = Deck.Instance.GetLowestAvailableNode();
        if (availableDeckNode != null) {
            // create new hero
            if (!heroPool.TryDequeue(out var hero)) {
                hero = Instantiate(heroPrefab);
            }
            hero.Activate();
            hero.WorldPosition = availableDeckNode.WorldPosition;
            hero.SetData(trait, HeroRank.B);
            heroes.Add(hero, availableDeckNode);
            availableDeckNode.ChangeState(NodeState.Occupied);
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

    public void Remove(LineUpHero hero) {
        heroes[hero].SetToEmpty();
        heroes.Remove(hero);
        heroPool.Enqueue(hero);
        hero.Deactivate();
    }
    
    public void RecalculateHeroesOnMap() {
        heroesOnMap = 0;
        foreach (var (_, node) in heroes) {
            if (node is MapNode) heroesOnMap++;
        }
        ArenaUIManager.Instance.Arena.UpdateLineUpText(heroesOnMap, maxHeroesOnMap);
    }

    bool MergeHeroes(HeroTrait trait, HeroRank rank, int required) {
        var duplicates = new List<LineUpHero>();
        foreach (var (h, _) in heroes) {
            if (h.Trait == trait && h.Rank == rank) {
                duplicates.Add(h);
            }
        }
        if (duplicates.Count < required) return false;

        for (int i = required - 1; i >= 1; i--) {
            Remove(duplicates[i]);
        }
        duplicates[0].Upgrade();
        RecalculateHeroesOnMap();
        return true;
    }

    public LineUpHero FindHeroOnNode(Node node) {
        foreach (var (h, n) in heroes) {
            if (node == n) return h;
        }

        return null;
    }

    public Node FindNodeOfHero(LineUpHero hero) {
        return heroes.GetValueOrDefault(hero);
    }

    public void UpdateHeroNode(LineUpHero hero, Node node) {
        var oldNode = heroes[hero];
        if (oldNode != node) {
            oldNode.SetToEmpty();
            node.ChangeState(NodeState.Occupied);
            heroes[hero] = node;
        }
        
        hero.UpdatePosition(node);
    }

    public void SwapHeroNodes(LineUpHero heroA, LineUpHero heroB) {
        (heroes[heroA], heroes[heroB]) = (heroes[heroB], heroes[heroA]);
        heroA.UpdatePosition(heroes[heroA]);
        heroB.UpdatePosition(heroes[heroB]);
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