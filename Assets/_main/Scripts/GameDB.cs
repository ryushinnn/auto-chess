using System;
using System.Linq;
using RExt.Patterns.Singleton;
using RExt.Extension;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameDB : Singleton<GameDB> {
    public AssetDB AssetDB => assetDB;
    
    [SerializeField] AssetDB assetDB;
}