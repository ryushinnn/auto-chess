using UnityEngine;

public abstract class Node {
    public Vector3 Position { get; protected set; }

    public abstract void Add(IMapNodeObject obj);
    public abstract void Remove(IMapNodeObject obj);
    public abstract T Get<T>(System.Func<T, bool> condition = null) where T : IMapNodeObject;
}