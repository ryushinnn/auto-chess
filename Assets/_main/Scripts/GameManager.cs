using System;
using System.Collections.Generic;
using RExt.Patterns.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    public Shop Shop => shop;
    public LineUp LineUp => lineUp;
    public Inventory Inventory => inventory;
    public BattleField BattleField => battleField;
    public Level Level => level;
    public Progress Progress => progress;
    
    [SerializeField] Shop shop;
    [SerializeField] LineUp lineUp;
    [SerializeField] Inventory inventory;
    [SerializeField] BattleField battleField;
    [SerializeField] Level level;
    [SerializeField] Progress progress;
    
    void Start() {
        MapVisual.Instance.Initialize();
        level.Initialize();
        lineUp.Initialize();
        inventory.Initialize();
        progress.Initialize();
    }
}