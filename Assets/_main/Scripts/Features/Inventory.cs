using System.Collections.Generic;
using RExt.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

public class Inventory : MonoBehaviour {
    [SerializeField] LayerMask heroLayerMask;
    
    Dictionary<Item, int> items = new();

    public void Initialize() {
        ArenaUIManager.Instance.Inventory.SetData(items);
    }
    
    [Button]
    public void Add(Item item) {
        if (items.ContainsKey(item)) {
            items[item]++;
        }
        else {
            items[item] = 1;
        }
        ArenaUIManager.Instance.LineUp.Close();
        ArenaUIManager.Instance.Inventory.Open();
        ArenaUIManager.Instance.Inventory.SetData(items);
    }

    [Button]
    public void Equip(Item item) {
        if (!items.ContainsKey(item)) return;
        
        var ray = Utils.MainCamera().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f, heroLayerMask)) {
            if (hit.collider.TryGetComponent<LineUpHero>(out var hero)) {
                if (hero.GetAbility<HeroInventory>().Add(item)) {
                    items[item]--;
                    if (items[item] == 0) {
                        items.Remove(item);
                    }
                    ArenaUIManager.Instance.Inventory.SetData(items);
                }
            }
        }
    }
}