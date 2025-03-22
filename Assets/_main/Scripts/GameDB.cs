using System;
using System.Linq;
using RExt.Core;
using RExt.Extension;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameDB : Singleton<GameDB> {
    public AssetDB AssetDB => assetDB;
    
    [SerializeField] AssetDB assetDB;
}