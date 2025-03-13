using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RExt.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public enum Direction {
    TopLeft,
    TopRight,
    Left,
    Right,
    BotLeft,
    BotRight
}

public enum SelectNodeMethod {
    Adjacent,
    Line,
    Sector,
}

public class Map : Singleton<Map> {
    [SerializeField] Transform nodeParent;
    [SerializeField] float nodeWidth;
    [SerializeField] float nodeHeight;
    [SerializeField] AstarPath aStarPath;
    
    MapNode[,] nodes;
    
    public const int SIZE = 8;
    
    public List<Dev_MapNode> dev_mapNodes = new();

    [Serializable]
    public class Dev_MapNode {
        public Vector2 xy;
        public List<string> objs;

        public Dev_MapNode(MapNode node) {
            xy = new Vector2(node.X, node.Y);
            objs = new List<string>();
            node.Process(x => {
                if (x is Hero h) {
                    objs.Add(h.name);
                }
                else if (x is DestinationMark m) {
                    objs.Add($"mark_{m.Owner.name}");
                }
            });
        }
    }

    void Start() {
        SpawnNodes();
    }

    void LateUpdate() {
        dev_mapNodes.Clear();
        
        for (int i=0; i<SIZE; i++) {
            for (int j=0; j<SIZE; j++) {
                MapVisual.Instance.MarkAsNonEmpty(nodes[i,j],nodes[i,j].Any(x=>x is Hero));

                if (!nodes[i, j].HasNone()) {
                    dev_mapNodes.Add(new Dev_MapNode(nodes[i, j]));
                }
            }
        }
    }
    
    public bool CheckAdjacency(MapNode target, MapNode origin, int radius = 1) {
        var potentialNodes = GetCircle(target.X, target.Y, radius);
        return potentialNodes.Any(x => x == origin);
    }

    public MapNode GetNearestAdjacentNode(MapNode target, MapNode origin, int radius, Func<MapNode,bool> condition = null) {
        var potentialNodes = GetCircle(target.X, target.Y, radius);
        var minDist = Mathf.Infinity;
        var node = default(MapNode);
        foreach (var n in potentialNodes) {
            if (condition != null && !condition(n)) continue;
            var dist = Vector3.Distance(origin.Position, n.Position);
            if (dist < minDist) {
                minDist = dist;
                node = n;
            }
        }
        
        return node;
    }
    
    public MapNode GetNearestNode(MapNode origin, Func<MapNode, bool> condition) {
        var minDist = Mathf.Infinity;
        var node = default(MapNode);
        for (int i=0; i<SIZE; i++) {
            for (int j=0; j<SIZE; j++) {
                if (condition(nodes[i, j])) {
                    var dist = Vector3.Distance(nodes[i, j].Position, origin.Position);
                    if (dist < minDist) {
                        minDist = dist;
                        node = nodes[i, j];
                    }
                }
            }
        }
        
        return node;
    }

    public MapNode GetNode(Vector3 position, Func<int,int,bool> condition = null) {
        var minDist = Mathf.Infinity;
        MapNode node = null;
        for (int i=0; i<SIZE; i++) {
            for (int j=0; j<SIZE; j++) {;
                var dist = Vector3.Distance(nodes[i, j].Position, position);
                if (dist < minDist && (condition == null || condition(i, j))) {
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

    public MapNode[] GetCircle(int x, int y, int range, bool containRoot = false) {
        var result = new HashSet<MapNode>();
        if (containRoot) {
            result.Add(GetNode(x, y));
        }
        GetCircle(x, y, range, result);
        return result.ToArray();
    }

    public MapNode[] GetLine(int x, int y, Direction direction, int range, bool containRoot = false) {
        var result = new HashSet<MapNode>();
        if (containRoot) {
            result.Add(GetNode(x, y));
        }
        GetLine(x, y, direction, range, result);
        return result.ToArray();
    }
    
    public MapNode[] GetSector(int x, int y, Direction direction, int range, bool containRoot = false) {
        var result = new HashSet<MapNode>();
        if (containRoot) {
            result.Add(GetNode(x, y));
        }
        GetSector(x, y, direction, range, result);
        return result.ToArray();
    }

    void SpawnNodes() {
        nodes = new MapNode[SIZE,SIZE];
        var rootOffset = new Vector3(nodeWidth/2, 0, nodeHeight/2 - nodeHeight/8);
        for (int i = 0; i < SIZE; i++) {
            for (int j = 0; j < SIZE; j++) {
                var columnOffset = new Vector3(i % 2 == 0 ? nodeWidth / 4 : nodeWidth / -4, 0, 0);
                var node = new MapNode();
                node.Initialize(i, j, new Vector3((j-SIZE/2) * nodeWidth, 0 , (i-SIZE/2) * nodeHeight * 3/4)
                                      + rootOffset
                                      + columnOffset);
                nodes[i, j] = node;
                var nodeVisual = new GameObject($"[{i},{j}]");
                nodeVisual.transform.SetParent(nodeParent);
                nodeVisual.transform.position = node.Position;
            }
        }

        MapVisual.Instance.SpawnHexIndicators(nodes, nodeWidth, nodeHeight);
        aStarPath.Scan();
    }

    MapNode GetAdjacentNode(int x, int y, Direction direction) {
        switch (direction) {
            case Direction.TopLeft:
                return x % 2 == 0 ? GetNode(x + 1, y) : GetNode(x + 1, y - 1);
            
            case Direction.TopRight:
                return x % 2 == 0 ? GetNode(x + 1, y + 1) : GetNode(x + 1, y);
            
            case Direction.Left:
                return GetNode(x, y - 1);
            
            case Direction.Right:
                return GetNode(x, y + 1);
            
            case Direction.BotLeft:
                return x % 2 == 0 ? GetNode(x - 1, y) : GetNode(x - 1, y - 1);
            
            case Direction.BotRight:
                return x % 2 == 0 ? GetNode(x - 1, y + 1) : GetNode(x - 1, y);
        }

        return null;
    }

    void GetCircle(int x, int y, int range, HashSet<MapNode> result) {
        if (range == 0) return;
        
        var topLeftNode = GetAdjacentNode(x, y, Direction.TopLeft);
        if (topLeftNode != null) {
            result.Add(topLeftNode);
            GetCircle(topLeftNode.X, topLeftNode.Y, range - 1, result);
        }
        
        var topRightNode = GetAdjacentNode(x, y, Direction.TopRight);
        if (topRightNode != null) {
            result.Add(topRightNode);
            GetCircle(topRightNode.X, topRightNode.Y, range - 1, result);
        }
        
        var leftNode = GetAdjacentNode(x, y, Direction.Left);
        if (leftNode != null) {
            result.Add(leftNode);
            GetCircle(leftNode.X, leftNode.Y, range - 1, result);
        }
        
        var rightNode = GetAdjacentNode(x, y, Direction.Right);
        if (rightNode != null) {
            result.Add(rightNode);
            GetCircle(rightNode.X, rightNode.Y, range - 1, result);
        }
        
        var botLeftNode = GetAdjacentNode(x, y, Direction.BotLeft);
        if (botLeftNode != null) {
            result.Add(botLeftNode);
            GetCircle(botLeftNode.X, botLeftNode.Y, range - 1, result);
        }
        
        var botRightNode = GetAdjacentNode(x, y, Direction.BotRight);
        if (botRightNode != null) {
            result.Add(botRightNode);
            GetCircle(botRightNode.X, botRightNode.Y, range - 1, result);
        }
    }

    void GetLine(int x, int y, Direction direction, int range, HashSet<MapNode> result) {
        if (range == 0) return;

        var node = GetAdjacentNode(x, y, direction);
        if (node != null) {
            result.Add(node);
            GetLine(node.X, node.Y, direction, range - 1, result);
        }
    }

    void GetSector(int x, int y, Direction direction, int range, HashSet<MapNode> result) {
        if (range == 0) return;

        if (direction is Direction.Left or Direction.TopLeft or Direction.TopRight) {
            var topLeftNode = GetAdjacentNode(x, y, Direction.TopLeft);
            if (topLeftNode != null) {
                result.Add(topLeftNode);
                GetSector(topLeftNode.X, topLeftNode.Y, direction, range - 1, result);
            }
        }

        if (direction is Direction.TopLeft or Direction.TopRight or Direction.Right) {
            var topRightNode = GetAdjacentNode(x, y, Direction.TopRight);
            if (topRightNode != null) {
                result.Add(topRightNode);
                GetSector(topRightNode.X, topRightNode.Y, direction, range - 1, result);
            }
        }

        if (direction is Direction.BotLeft or Direction.Left or Direction.TopLeft) {
            var leftNode = GetAdjacentNode(x, y, Direction.Left);
            if (leftNode != null) {
                result.Add(leftNode);
                GetSector(leftNode.X, leftNode.Y, direction, range - 1, result);
            }
        }
        
        if (direction is Direction.TopRight or Direction.Right or Direction.BotRight) {
            var rightNode = GetAdjacentNode(x, y, Direction.Right);
            if (rightNode != null) {
                result.Add(rightNode);
                GetSector(rightNode.X, rightNode.Y, direction, range - 1, result);
            }
        }

        if (direction is Direction.Left or Direction.BotLeft or Direction.BotRight) {
            var botLeftNode = GetAdjacentNode(x, y, Direction.BotLeft);
            if (botLeftNode != null) {
                result.Add(botLeftNode);
                GetSector(botLeftNode.X, botLeftNode.Y, direction, range - 1, result);
            }
        }
        
        if (direction is Direction.BotLeft or Direction.BotRight or Direction.Right) {
            var botRightNode = GetAdjacentNode(x, y, Direction.BotRight);
            if (botRightNode != null) {
                result.Add(botRightNode);
                GetSector(botRightNode.X, botRightNode.Y, direction, range - 1, result);
            }
        }
    }

    [Button]
    public void Dev_DetectObject() {
        for (int i=0; i<SIZE; i++) {
            for (int j=0; j<SIZE; j++) {
                if (!nodes[i, j].HasNone()) {
                    Debug.Log($"[{i},{j}] has object");
                }
            }
        }
    }
}