using System;
using RExt.Patterns.Singleton;
using UnityEngine;

public class ArenaUIManager : Singleton<ArenaUIManager> {
    public ArenaUI Arena => arena;
    public ShopUI Shop => shop;
    public LineUpUI LineUp => lineUp;
    public InventoryUI Inventory => inventory;
    
    [SerializeField] ArenaUI arena;
    [SerializeField] ShopUI shop;
    [SerializeField] LineUpUI lineUp;
    [SerializeField] InventoryUI inventory;

    void Start() {
        arena.Open();
        shop.Close();
        lineUp.Open();
        inventory.Close();
    }
}