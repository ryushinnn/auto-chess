using System;
using UnityEngine;

public class Map : MonoBehaviour {
    [SerializeField] Transform _nodeParent;
    [SerializeField] float _nodeWidth;
    [SerializeField] float _nodeHeight;
    [SerializeField] Indicators _indicators;
    [SerializeField] AstarPath _aStarPath;
    
    GameObject[,] _nodes;
    
    const int SIZE = 8;

    void Start() {
        SpawnNodes();
    }

    void SpawnNodes() {
        _nodes = new GameObject[SIZE,SIZE];
        var rootOffset = new Vector3(_nodeWidth/2, 0, _nodeHeight/2 - _nodeHeight/8);
        for (int i = 0; i < SIZE; i++) {
            for (int j = 0; j < SIZE; j++) {
                var columnOffset = new Vector3(i % 2 == 0 ? _nodeWidth / 4 : _nodeWidth / -4, 0, 0);
                var node = new GameObject($"Node[{i},{j}]");
                node.transform.SetParent(_nodeParent);
                node.transform.localPosition = new Vector3((j-SIZE/2) * _nodeWidth, 0 , (i-SIZE/2) * _nodeHeight * 3/4)
                                               + rootOffset
                                               + columnOffset;
                node.transform.localScale = new Vector3(_nodeWidth, 1, _nodeHeight);
                _nodes[i, j] = node;
            }
        }

        _indicators.SpawnHexIndicators(_nodes);
        _aStarPath.Scan();
    }
}