using System;
using UnityEngine;

public class DeckNode : Node {
    public int Index { get; private set; }
    
    IMapNodeObject obj;

    public void Initialize(int index, Vector3 position) {
        Index = index;
        Position = position;
    }

    public override void Add(IMapNodeObject obj) {
        this.obj = obj;
    }
    
    public override void Remove(IMapNodeObject obj) {
        if (this.obj == obj) {
            this.obj = null;
        }
    }
    
    public override T Get<T>(Func<T,bool> condition = null) {
        if (this.obj is T obj && (condition == null || condition(obj))) {
            return obj;
        }

        return default;
    }

    public bool HasNone() {
        return obj == null;
    }

    public bool Has(IMapNodeObject obj) {
        return obj == this.obj;
    }
    
    public void Process(Action<IMapNodeObject> action) {
        action(obj);
    }
}