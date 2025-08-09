using System;
using RExt.Patterns.Singleton;
using UnityEngine;

public class UIManager_Arena : Singleton<UIManager_Arena> {
    public ArenaUI Arena => arena;
    public ShopUI Shop => shop;
    public LineUpUI LineUp => lineUp;
    public InventoryUI Inventory => inventory;
    public HeroInfoUI HeroInfo => heroInfo;
    public StarterPackUI StarterPack => starterPack;
    
    [SerializeField] ArenaUI arena;
    [SerializeField] ShopUI shop;
    [SerializeField] LineUpUI lineUp;
    [SerializeField] InventoryUI inventory;
    [SerializeField] HeroInfoUI heroInfo;
    [SerializeField] StarterPackUI starterPack;
    
    void Start() {
        arena.Close();
        shop.Close();
        lineUp.Close();
        inventory.Close();
        heroInfo.Close();
        starterPack.Open();
    }
}