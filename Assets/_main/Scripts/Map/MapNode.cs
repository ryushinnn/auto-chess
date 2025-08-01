using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MapNodeState {
    Empty,
    Targeted,
    Owned,
}

public class MapNode : Node {
    public int X => GridPosition.x;
    public int Y => GridPosition.y;
    public Vector2Int GridPosition { get; private set; }
    public MapNodeState State { get; private set; }
    
    public MapNode(int x, int y, Vector3 position) : base(position) {
        GridPosition = new Vector2Int(x, y);
        State = MapNodeState.Empty;
    }

    public override bool IsEmpty() {
        return State == MapNodeState.Empty;
    }
    
    public override void SetToEmpty() {
        State = MapNodeState.Empty;
    }
    
    public void ChangeState(MapNodeState state) {
        State = state;
    }

    public override string ToString() {
        return $"({X},{Y})";
    }
}