using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RExt.Extension;
using RExt.Patterns.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

public class Map : Singleton<Map> {
    [SerializeField] Transform nodeParent;
    [SerializeField] float nodeWidth;
    [SerializeField] float nodeHeight;
    
    MapNode[,] nodes;
    
    public const int SIZE = 8;
    

    void Start() {
        SpawnNodes();
    }

    void LateUpdate() {
        for (int i=0; i<SIZE; i++) {
            for (int j=0; j<SIZE; j++) {
                var cell = MapVisual.Instance.GetHexCell(i, j);
                var node = nodes[i, j];
                cell.SetHighlight(node.State == MapNodeState.Owned);
                cell.SwitchFlag(node.State == MapNodeState.Targeted);
            }
        }
    }
    
    void SpawnNodes() {
        nodes = new MapNode[SIZE,SIZE];
        var rootOffset = new Vector3(nodeWidth/2, 0, nodeHeight/2 - nodeHeight/8);
        for (int i = 0; i < SIZE; i++) {
            for (int j = 0; j < SIZE; j++) {
                var columnOffset = new Vector3(i % 2 == 0 ? nodeWidth / 4 : nodeWidth / -4, 0, 0);
                var worldPos = new Vector3((j - SIZE / 2) * nodeWidth, 0, (i - SIZE / 2) * nodeHeight * 3 / 4) + rootOffset + columnOffset;
                nodes[i, j] = new MapNode(i, j, worldPos);
                var nodeVisual = new GameObject($"[{i},{j}]");
                nodeVisual.transform.SetParent(nodeParent);
                nodeVisual.transform.position = nodes[i,j].WorldPosition;
            }
        }

        MapVisual.Instance.SpawnHexIndicators(nodes, nodeWidth, nodeHeight);
    }
    
    public bool CheckAdjacency(MapNode target, MapNode origin, int radius) {
        return GetNeighbors(origin, radius).Contains(target);
    }

    public List<MapNode> GetNeighbors(MapNode origin, int radius, bool includeOrigin = false) {
        var result = new List<MapNode>();
        var visited = new HashSet<MapNode>() { origin };
        var queue = new Queue<(MapNode node, int depth)>();
        queue.Enqueue((origin, 0));

        while (queue.Count > 0) {
            var (node,depth) = queue.Dequeue();
            if (depth > radius) continue;
            
            if (node != origin || includeOrigin) {
                result.Add(node);
            }

            foreach (var dir in DirectionUtils.GetAllDirections()) {
                var offset = DirectionUtils.GetOffset(dir, node.X);
                var neighbor = GetNode(node.GridPosition + offset);
                if (neighbor == null || !visited.Add(neighbor)) continue;
                queue.Enqueue((neighbor, depth + 1));
            }
        }

        return result;
    }

    public MapNode GetNearestNode(Vector3 position, Func<MapNode,bool> condition = null) {
        var minDist = Mathf.Infinity;
        MapNode node = null;
        for (int i=0; i<SIZE; i++) {
            for (int j=0; j<SIZE; j++) {
                if (condition != null && !condition(nodes[i, j])) continue;
                var dist = Vector3.Distance(nodes[i, j].WorldPosition, position);
                if (dist < minDist) {
                    minDist = dist;
                    node = nodes[i, j];
                }
            }
        }
        
        return node;
    }

    public MapNode GetNearestNode(MapNode target, Func<MapNode, bool> condition = null) {
        var minDist = Mathf.Infinity;
        MapNode node = null;
        for (int i=0; i<SIZE; i++) {
            for (int j=0; j<SIZE; j++) {
                if (nodes[i,j] == target || (condition != null && !condition(nodes[i, j]))) continue;
                var dist = Vector3.Distance(nodes[i, j].WorldPosition, target.WorldPosition);
                if (dist < minDist) {
                    minDist = dist;
                    node = nodes[i, j];
                }
            }
        }
        
        return node;
    }
    
    public MapNode GetNode(int x, int y) {
        if (0 <= x && x < SIZE && 0 <= y && y < SIZE) {
            return nodes[x, y];
        }

        return null;
    }

    public MapNode GetNode(Vector2Int gridPos) {
        return GetNode(gridPos.x, gridPos.y);
    }

    List<MapNode> testpath = new();
    [Button]
    void FindPath(int x1, int y1, int x2, int y2) {
        testpath = this.FindPath(GetNode(x1, y1), GetNode(x2, y2));
    }

    [Button]
    void SetOwned(int x, int y) {
        nodes[x, y].ChangeState(MapNodeState.Owned);
        MapVisual.Instance.MarkAsNonEmpty(nodes[x, y], true);
    }

    [Button]
    void ResetAll() {
        for (int i=0; i<SIZE; i++) {
            for (int j=0; j<SIZE; j++) {
                nodes[i,j].SetToEmpty();
                MapVisual.Instance.MarkAsNonEmpty(nodes[i,j],false);
            }
        }
    }

    void OnDrawGizmos() {
        if (!Application.isPlaying || testpath == null) return;
        for (int i = 1; i < testpath.Count; i++) {
            Gizmos.DrawLine(testpath[i].WorldPosition, testpath[i - 1].WorldPosition);
        }
    }
}

public static class MapExtensions {
    public static MapNode GetNearestFrom(this List<MapNode> nodes, MapNode origin) {
        MapNode result = null;
        var minDist = Mathf.Infinity;
        foreach (var target in nodes) {
            var dist = Vector3.Distance(origin.WorldPosition, target.WorldPosition);
            if (dist < minDist) {
                minDist = dist;
                result = target;
            }
        }
        
        return result;
    }

    public static List<MapNode> Filter(this List<MapNode> nodes, Func<MapNode, bool> condition) {
        var result = new List<MapNode>();
        foreach (var node in nodes) {
            if (condition(node)) {
                result.Add(node);
            }
        }

        return result;
    }
}