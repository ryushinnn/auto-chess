using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LineUp : MonoBehaviour {
    [SerializeField] Hero heroPrefab;
    [SerializeField, ReadOnly] List<Hero> heroes;

    public void Add(HeroTrait trait) {
        if (MergeHeroes(trait, HeroRank.B, 2)) {
            if (MergeHeroes(trait, HeroRank.A, 3)) {
                return;
            }

            return;
        }
        
        var hero = Instantiate(heroPrefab);
        hero.Initialize(trait, TeamSide.Ally);
        heroes.Add(hero);
    }

    public void Remove(string id) {
        var hero = heroes.Find(x => x.ID == id);
        heroes.Remove(hero);
        Destroy(hero.gameObject);
    }

    public void Remove(Hero hero) {
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
}

public enum TeamSide {
    Ally,
    Enemy
}