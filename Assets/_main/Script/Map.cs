using System;
using UnityEngine;

public class Map : MonoBehaviour {
    [SerializeField] Transform _nodeParent;
    [SerializeField] float _nodeWidth;
    [SerializeField] float _nodeHeight;
    [SerializeField] Indicators _indicators;
    [SerializeField] AstarPath _aStarPath;
    
    Node[,] _nodes;
    
    const int SIZE = 8;

    void Start() {
        SpawnNodes();
    }

    public Node[] GetAdjacentNodes(int x, int y) {
        var result = new Node[6];
        result[0] = GetNode(x, y - 1);
        result[1] = GetNode(x, y + 1);
        result[2] = GetNode(x - 1, y);
        result[3] = GetNode(x + 1, y);
        if (x % 2 == 0) {
            result[4] = GetNode(x - 1, y + 1);
            result[5] = GetNode(x + 1, y + 1);
        } else {
            result[4] = GetNode(x - 1, y - 1);
            result[5] = GetNode(x + 1, y - 1);
        }

        return result;
    }

    void SpawnNodes() {
        _nodes = new Node[SIZE,SIZE];
        var rootOffset = new Vector3(_nodeWidth/2, 0, _nodeHeight/2 - _nodeHeight/8);
        for (int i = 0; i < SIZE; i++) {
            for (int j = 0; j < SIZE; j++) {
                var columnOffset = new Vector3(i % 2 == 0 ? _nodeWidth / 4 : _nodeWidth / -4, 0, 0);
                var node = new GameObject($"Node[{i},{j}]").AddComponent<Node>();
                node.transform.SetParent(_nodeParent);
                node.transform.localPosition = new Vector3((j-SIZE/2) * _nodeWidth, 0 , (i-SIZE/2) * _nodeHeight * 3/4)
                                               + rootOffset
                                               + columnOffset;
                node.transform.localScale = new Vector3(_nodeWidth, 1, _nodeHeight);
                node.SaveIndex(i, j);
                _nodes[i, j] = node;
            }
        }

        _indicators.SpawnHexIndicators(_nodes);
        _aStarPath.Scan();
    }

    Node GetNode(int x, int y) {
        if (0 <= x && x < SIZE && 0 <= y && y < SIZE) {
            return _nodes[x, y];
        }

        return null;
    }
}