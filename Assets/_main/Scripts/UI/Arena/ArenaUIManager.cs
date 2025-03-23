using RExt.Patterns.Singleton;
using UnityEngine;

public class ArenaUIManager : Singleton<ArenaUIManager> {
    public ShopUI Shop => shop;
    
    [SerializeField] ShopUI shop;
}