using System;
using System.Linq;
using RExt.Core;
using UnityEngine;

public class CompleteItems : Singleton<CompleteItems> {
    [SerializeField] Item[] items;
    
    public Item GetItem(Item ingredient0, Item ingredient1) {
        return Array.Find(items, x => x.ingredients.Contains(ingredient0) && x.ingredients.Contains(ingredient1));
    }
}