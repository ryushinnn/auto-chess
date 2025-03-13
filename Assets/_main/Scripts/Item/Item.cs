using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class Item : ScriptableObject {
    public string name;
    [PreviewField] public Sprite icon;
    public Item[] ingredients;
    public AttributeModifier[] modifiers;

    public bool IsCompleteItem() {
        return ingredients != null && ingredients.Length == 2;
    }
    
    void OnValidate() {
        foreach (var m in modifiers) {
            m.id = "<will be auto generated>";
            m.permanent = true;
            m.duration = Mathf.Infinity;
        }
    }
}