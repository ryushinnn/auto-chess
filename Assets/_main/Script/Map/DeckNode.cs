using System;
using UnityEngine;

public class DeckNode : Node {
    public int Index { get; private set; }
    
    IMapNodeObject obj;

    public void Initialize(int index, Vector3 position) {
        Index = index;
        Position = position;
    }

    public bool HasNone() {
        return obj == null;
    }
    
    public void Process(Action<IMapNodeObject> action) {
        action(obj);
    }
}