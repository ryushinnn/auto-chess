using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapNode : Node {
    public int X { get; private set; }
    public int Y { get; private set; }
    
    List<IMapNodeObject> objects = new();
    
    public void Initialize(int x, int y, Vector3 position) {
        X = x;
        Y = y;
        Position = position;
    }

    public void Add(IMapNodeObject obj) {
        objects.Add(obj);
    }
    
    public void Remove(IMapNodeObject obj) {
        objects.Remove(obj);
    }

    public T Get<T>(Func<T, bool> condition = null) where T : IMapNodeObject {
        return (T)objects.FirstOrDefault(x=> x is T obj && (condition == null || condition(obj)));
    }

    public bool Any(Func<IMapNodeObject, bool> condition) {
        return objects.Any(condition);
    }

    public bool HasOnly(IMapNodeObject obj) {
        return objects.Count == 1 && objects[0] == obj;
    }
    
    public bool HasAtFirst(IMapNodeObject obj) {
        return objects.Count > 0 && objects[0] == obj;
    }

    public bool HasNone() {
        return objects.Count == 0;
    }

    public void Process(Action<IMapNodeObject> action) {
        objects.ForEach(action);
    }
}