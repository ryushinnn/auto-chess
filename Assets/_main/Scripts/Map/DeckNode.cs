using System;
using UnityEngine;

public enum DeckNodeState {
    Empty,
    Owned,
}

public class DeckNode : Node {
    public int LinePosition { get; private set; }
    public DeckNodeState State { get; private set; }

    public DeckNode(int index, Vector3 position) : base(position) {
        LinePosition = index;
        State = DeckNodeState.Empty;
    }

    public override bool IsEmpty() {
        return State == DeckNodeState.Empty;
    }
    
    public override void SetToEmpty() {
        State = DeckNodeState.Empty;
    }
    
    public void ChangeState(DeckNodeState state) {
        State = state;
    }
}