using System;
using System.Linq;
using RExt.Core;
using UnityEngine;

public class StaticDataManager : Singleton<StaticDataManager> {
    [SerializeField] HeroTrait[] heroTraits;
    [SerializeField] Item[] completeItems;

    public HeroTrait GetHeroTrait(string id) {
        return Array.Find(heroTraits, x => x.id == id);
    }
    
    public Item GetCompleteItem(Item ingredient0, Item ingredient1) {
        return Array.Find(completeItems, x => x.ingredients.Contains(ingredient0) && x.ingredients.Contains(ingredient1));
    }
}