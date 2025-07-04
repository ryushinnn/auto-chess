using System;
using RExt.Patterns.Singleton;
using UnityEngine;

public class ArenaUIManager : Singleton<ArenaUIManager> {
    public ArenaUI Arena => arena;
    public ShopUI Shop => shop;
    public LineUpUI LineUp => lineUp;
    public InventoryUI Inventory => inventory;
    public HeroInfoUI HeroInfo => heroInfo;
    
    [SerializeField] ArenaUI arena;
    [SerializeField] ShopUI shop;
    [SerializeField] LineUpUI lineUp;
    [SerializeField] InventoryUI inventory;
    [SerializeField] HeroInfoUI heroInfo;

    void Start() {
        arena.Open();
        shop.Close();
        lineUp.Open();
        inventory.Close();
        heroInfo.Close();
    }

    public void Collapse() {
        shop.Close();
        heroInfo.Close();
    }
}