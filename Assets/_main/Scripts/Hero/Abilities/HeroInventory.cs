using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HeroInventory : HeroAbility {
    HeroAttributes attributes;
    
    [SerializeField] Image[] itemIcons;
    [SerializeField] RectTransform ui;
    [SerializeField, ReadOnly] List<ItemSlot> itemSlots = new();
    
    const int CAPACITY = 3;
    const float UI_HEIGHT_NO_ITEM = 2.5f;
    const float UI_HEIGHT_WITH_ITEM = 3.25f;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        foreach (var i in itemIcons) {
            i.enabled = false;
        }
        ui.anchoredPosition = new Vector2(ui.anchoredPosition.x, UI_HEIGHT_NO_ITEM);
    }

    protected override void FindReferences() {
        attributes = hero.GetAbility<HeroAttributes>();
    }

    public void Add(Item item) {
        if (itemSlots.Count == CAPACITY) return;

        ItemSlot slot;
        if (item.IsCompleteItem() || itemSlots.Count == 0 || itemSlots[^1].item.IsCompleteItem()) {
            slot = new ItemSlot(item);
        }
        else {
            var completeItem = StaticDataManager.Instance.GetCompleteItem(item, itemSlots[^1].item);
            itemSlots.RemoveAt(itemSlots.Count - 1);
            slot = new ItemSlot(completeItem);
        }

        foreach (var m in slot.item.modifiers) {
            var modifier = AttributeModifier.Create(m);
            attributes.AddAttributeModifier(modifier);
            slot.modifierIds.Add(modifier.id);
        }
        itemSlots.Add(slot);
        itemSlots.Sort((a, b) => {
            if (a.item.IsCompleteItem() && !b.item.IsCompleteItem()) return -1;
            if (!a.item.IsCompleteItem() && b.item.IsCompleteItem()) return 1;
            return 0;
        });
        for (int i = 0; i < itemIcons.Length; i++) {
            if (i >= itemSlots.Count) {
                itemIcons[i].enabled = false;
                continue;
            }
            
            itemIcons[i].enabled = true;
            itemIcons[i].sprite = itemSlots[i].item.icon;
        }
        ui.anchoredPosition = new Vector2(ui.anchoredPosition.x, itemSlots.Count > 0 ? UI_HEIGHT_WITH_ITEM : UI_HEIGHT_NO_ITEM);
    }

    [Button]
    void Dev_Add(Item item) {
        Add(item);
    }

    [Button]
    void Dev_RemoveAll() {
        foreach (var s in itemSlots) {
            foreach (var id in s.modifierIds) {
                attributes.RemoveAttributeModifier(id);
            }
        }
        itemSlots.Clear();
    }
}

[Serializable]
public class ItemSlot {
    public Item item;
    public List<string> modifierIds = new();
    
    public ItemSlot(Item item) {
        this.item = item;
    }
}