using UnityEngine;

public abstract class Node {
    public Vector3 WorldPosition { get; protected set; }

    protected Node(Vector3 worldPosition) {
        WorldPosition = worldPosition;
    }

    public abstract bool IsEmpty();
    public abstract void SetToEmpty();
}