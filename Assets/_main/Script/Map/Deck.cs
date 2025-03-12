using System;
using RExt.Core;
using UnityEngine;

public class Deck : Singleton<Deck> {
    [SerializeField] float nodeWidth;       
    [SerializeField] float nodeHeight;

    DeckNode[] nodes;
    
    public const int SIZE = 9;
    const float DECK_OFFSET_X = -0.66f;
    const float DECK_OFFSET_Z = 7.65f;

    void Start() {
        SpawnNodes();
    }

    void SpawnNodes() {
        nodes = new DeckNode[SIZE];
        for (int i = 0; i < SIZE; i++) {
            var node = new DeckNode();
            node.Initialize(i, new Vector3((i - SIZE / 2) * nodeWidth + DECK_OFFSET_X, 0, DECK_OFFSET_Z));
            nodes[i] = node;
            var nodeVisual = new GameObject($"[{i}]");
        }
    }
}