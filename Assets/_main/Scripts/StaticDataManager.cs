using System;
using System.Linq;
using RExt.Core;
using RExt.Extension;
using Sirenix.OdinInspector;
using UnityEngine;

public class StaticDataManager : Singleton<StaticDataManager> {
    [SerializeField] HeroTrait[] heroTraits;
    [SerializeField] Item[] completeItems;
    [SerializeField, TableList] Icon[] markIcons;

    public HeroTrait GetHeroTrait(string id) {
        return Array.Find(heroTraits, x => x.id == id);
    }
    
    public Item GetCompleteItem(Item ingredient0, Item ingredient1) {
        return Array.Find(completeItems, x => (x.ingredients[0] == ingredient0 && x.ingredients[1] == ingredient1)
                                                        || (x.ingredients[0] == ingredient1 && x.ingredients[1] == ingredient0));
    }

    public Icon GetMarkIcon(string key) {
        return markIcons.Find(x=>x.key == key);
    }
}

[Serializable]
public class Icon {
    public string key;
    [PreviewField]
    public Sprite value;
}