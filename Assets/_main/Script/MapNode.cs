using UnityEngine;

public class MapNode {
    public int X { get; private set; }
    public int Y { get; private set; }
    public Vector3 Position { get; private set; }
    public IMapNodeObject obj;
    
    public void Initialize(int x, int y, Vector3 position) {
        X = x;
        Y = y;
        Position = position;
    }
}