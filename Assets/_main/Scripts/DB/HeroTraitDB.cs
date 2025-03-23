using System;
using System.Collections.Generic;
using RExt.Extension;
using RExt.Patterns.Singleton;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroTraitDB", menuName = "DB/HeroTraitDB")]
public class HeroTraitDB : ScriptableObjectSingleton<HeroTraitDB> {
    [SerializeField] HeroTrait[] heroTraits;
    
    public HeroTrait Find(string id) {
        return heroTraits.Find(x => x.id == id);
    }

    public List<HeroTrait> FindAll(Func<HeroTrait,bool> condition) {
        var results = new List<HeroTrait>();
        foreach (var t in heroTraits) {
            if (condition(t)) {
                results.Add(t);
            }
        }
        return results;
    }
}