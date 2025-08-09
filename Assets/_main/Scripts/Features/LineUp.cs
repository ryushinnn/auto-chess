using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RExt.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class LineUp : MonoBehaviour {
    [SerializeField] LineUpHero heroPrefab;

    Dictionary<Role, int> roleNumbers = new();
    Dictionary<Realm, int> realmNumbers = new();
    HashSet<HeroTrait> uniqueTraits = new();

    Dictionary<LineUpHero, Node> heroes = new();
    Dictionary<LineUpHero, MapNode> heroesOnMap = new();
    Queue<LineUpHero> heroPool = new();
    
    public bool Full => heroesOnMap.Count >= maxHeroesOnMap;

    int maxHeroesOnMap;

    public void Initialize() {
        var allRoles = ((Role[])Enum.GetValues(typeof(Role))).Where(x => x != 0).ToArray();
        var allRealms = ((Realm[])Enum.GetValues(typeof(Realm))).Where(x => x != 0).ToArray();
        foreach (var role in allRoles) {
            roleNumbers.Add(role, 0);
        }
        foreach (var realm in allRealms) {
            realmNumbers.Add(realm, 0);
        }
        SetHeroesLimit(1);
        ArenaUIManager.Instance.LineUp.Initialize(roleNumbers, realmNumbers);
        
        var matchedHeroes = HeroTraitDB.Instance.FindAll(e => e.reputation == Reputation.Unknown && !e.summoned);
        var initHero = matchedHeroes[Random.Range(0, matchedHeroes.Count)];
        Add(initHero);
    }

    public void SetHeroesLimit(int value) {
        maxHeroesOnMap = value;
        ArenaUIManager.Instance.Arena.UpdateLineUpText(heroesOnMap.Count, maxHeroesOnMap);
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
            MapVisual.Instance.SetOccupied(availableDeckNode, true);
            RecalculateHeroesOnMap();

            if (uniqueTraits.Add(hero.Trait)) {
                var roles = hero.Trait.role.GetAllFlags().Where(x => x != 0).ToArray();
                foreach (var role in roles) {
                    roleNumbers[role]++;
                }
                realmNumbers[hero.Trait.realm]++;
                ArenaUIManager.Instance.Inventory.Close();
                ArenaUIManager.Instance.LineUp.Open();
                ArenaUIManager.Instance.LineUp.Initialize(roleNumbers, realmNumbers);
            }
            
            return true;
        }

        return false;
    }

    public void Remove(LineUpHero hero) {
        var node = heroes[hero];
        node.SetToEmpty();
        MapVisual.Instance.SetOccupied(node, false);
        heroes.Remove(hero);
        heroPool.Enqueue(hero);
        hero.Deactivate();
    }
    
    public void RecalculateHeroesOnMap() {
        heroesOnMap.Clear();
        foreach (var (hero, node) in heroes) {
            if (node is MapNode mn) heroesOnMap.Add(hero, mn);
        }
        ArenaUIManager.Instance.Arena.UpdateLineUpText(heroesOnMap.Count, maxHeroesOnMap);
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
            MapVisual.Instance.SetOccupied(oldNode, false);
            node.ChangeState(NodeState.Occupied);
            MapVisual.Instance.SetOccupied(node, true);
            heroes[hero] = node;
        }
        
        hero.UpdatePosition(node);
    }

    public void SwapHeroNodes(LineUpHero heroA, LineUpHero heroB) {
        (heroes[heroA], heroes[heroB]) = (heroes[heroB], heroes[heroA]);
        heroA.UpdatePosition(heroes[heroA]);
        heroB.UpdatePosition(heroes[heroB]);
    }

    public Dictionary<LineUpHero, MapNode> GetHeroOnMap() {
        return heroesOnMap;
    }

    public void ShowHeroesOnMap() {
        foreach (var (hero, node) in heroesOnMap) {
            hero.Activate();
            hero.SwitchCanvas(true);
            node.ChangeState(NodeState.Occupied);
            MapVisual.Instance.SetOccupied(node, true);
            hero.Mecanim.SetRendererVisibility(true);
            hero.Mecanim.ResetPosition();
        }
    }

    public void HideHeroesOnMap() {
        foreach (var (hero, node) in heroesOnMap) {
            hero.Deactivate();
            node.SetToEmpty();
        }
    }

    public void SetHeroesPickable(bool value) {
        foreach (var hero in heroes.Keys) {
            hero.SwitchPickable(value);
        }
    }

    public void FillHeroesOnMap() {
        var heroesOnDeck = heroes.Where(x => x.Value is DeckNode)
            .Select(x=>x.Key).ToList();
        while (!Full && heroesOnDeck.Count > 0) {
            var hero = heroesOnDeck[0];
            var mapNode = Map.Instance.GetLowestAvailableNode();
            heroes[hero].SetToEmpty();
            MapVisual.Instance.SetOccupied(heroes[hero], false);
            mapNode.ChangeState(NodeState.Occupied);
            MapVisual.Instance.SetOccupied(mapNode, true);
            heroes[hero] = mapNode;
            hero.WorldPosition = mapNode.WorldPosition;
            heroesOnMap.Add(hero, mapNode);
            heroesOnDeck.Remove(hero);
        }
        
        ArenaUIManager.Instance.Arena.UpdateLineUpText(heroesOnMap.Count, maxHeroesOnMap);
    }

    public void PlayHeroesDiveInAnimation() {
        foreach (var (hero, node) in heroesOnMap) {
            hero.Mecanim.DiveIn();
            hero.SwitchCanvas(false);
            MapVisual.Instance.SetOccupied(node, false);
        }
        MapVisual.Instance.SwitchPortal(true);
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