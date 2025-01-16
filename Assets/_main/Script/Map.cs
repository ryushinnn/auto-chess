using System;
using System.Collections.Generic;
using System.Linq;
using RExt.Core;
using Sirenix.OdinInspector;
using Unity.Burst.Intrinsics;
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
    
    [TitleGroup("DEVELOPMENT")]
    public Hero dev_hero;
    public int dev_nodeX;
    public int dev_nodeY;
    
    Node[,] nodes;
    
    const int SIZE = 8;

    void Start() {
        SpawnNodes();
    }

    public Node GetNode(Vector3 position) {
        var minDist = Mathf.Infinity;
        var node = default(Node);
        for (int i=0; i<SIZE; i++) {
            for (int j=0; j<SIZE; j++) {;
                var dist = Vector3.Distance(nodes[i, j].Position, position);
                if (dist < minDist) {
                    minDist = dist;
                    node = nodes[i, j];
                }
            }
        }
        
        return node;
    }
    
    public Node GetNode(int x, int y) {
        if (0 <= x && x < SIZE && 0 <= y && y < SIZE) {
            return nodes[x, y];
        }

        return null;
    }

    public Node[] GetAdjacentNodes(int x, int y, int range) {
        var result = new HashSet<Node>();
        GetAdjacentNodes(x, y, range, result);
        return result.ToArray();
    }

    public Node[] GetLineOfNodes(int x, int y, Direction direction, int range) {
        var result = new HashSet<Node>();
        GetLineOfNodes(x, y, direction, range, result);
        return result.ToArray();
    }
    
    public Node[] GetSectorOfNodes(int x, int y, Direction direction, int range) {
        var result = new HashSet<Node>();
        GetSectorOfNodes(x, y, direction, range, result);
        return result.ToArray();
    }

    void SpawnNodes() {
        nodes = new Node[SIZE,SIZE];
        var rootOffset = new Vector3(nodeWidth/2, 0, nodeHeight/2 - nodeHeight/8);
        for (int i = 0; i < SIZE; i++) {
            for (int j = 0; j < SIZE; j++) {
                var columnOffset = new Vector3(i % 2 == 0 ? nodeWidth / 4 : nodeWidth / -4, 0, 0);
                var node = new Node();
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
        
        dev_hero.SetNode(GetNode(dev_nodeX, dev_nodeY));
        dev_hero.ResetPosition();
    }

    Node GetAdjacentNode(int x, int y, Direction direction) {
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

    void GetAdjacentNodes(int x, int y, int range, HashSet<Node> result) {
        if (range == 0) return;
        
        var topLeftNode = GetAdjacentNode(x, y, Direction.TopLeft);
        if (topLeftNode != null) {
            result.Add(topLeftNode);
            GetAdjacentNodes(topLeftNode.X, topLeftNode.Y, range - 1, result);
        }
        
        var topRightNode = GetAdjacentNode(x, y, Direction.TopRight);
        if (topRightNode != null) {
            result.Add(topRightNode);
            GetAdjacentNodes(topRightNode.X, topRightNode.Y, range - 1, result);
        }
        
        var leftNode = GetAdjacentNode(x, y, Direction.Left);
        if (leftNode != null) {
            result.Add(leftNode);
            GetAdjacentNodes(leftNode.X, leftNode.Y, range - 1, result);
        }
        
        var rightNode = GetAdjacentNode(x, y, Direction.Right);
        if (rightNode != null) {
            result.Add(rightNode);
            GetAdjacentNodes(rightNode.X, rightNode.Y, range - 1, result);
        }
        
        var botLeftNode = GetAdjacentNode(x, y, Direction.BotLeft);
        if (botLeftNode != null) {
            result.Add(botLeftNode);
            GetAdjacentNodes(botLeftNode.X, botLeftNode.Y, range - 1, result);
        }
        
        var botRightNode = GetAdjacentNode(x, y, Direction.BotRight);
        if (botRightNode != null) {
            result.Add(botRightNode);
            GetAdjacentNodes(botRightNode.X, botRightNode.Y, range - 1, result);
        }
    }

    void GetLineOfNodes(int x, int y, Direction direction, int range, HashSet<Node> result) {
        if (range == 0) return;

        var node = GetAdjacentNode(x, y, direction);
        if (node != null) {
            result.Add(node);
            GetLineOfNodes(node.X, node.Y, direction, range - 1, result);
        }
    }

    void GetSectorOfNodes(int x, int y, Direction direction, int range, HashSet<Node> result) {
        if (range == 0) return;

        if (direction is Direction.Left or Direction.TopLeft or Direction.TopRight) {
            var topLeftNode = GetAdjacentNode(x, y, Direction.TopLeft);
            if (topLeftNode != null) {
                result.Add(topLeftNode);
                GetSectorOfNodes(topLeftNode.X, topLeftNode.Y, direction, range - 1, result);
            }
        }

        if (direction is Direction.TopLeft or Direction.TopRight or Direction.Right) {
            var topRightNode = GetAdjacentNode(x, y, Direction.TopRight);
            if (topRightNode != null) {
                result.Add(topRightNode);
                GetSectorOfNodes(topRightNode.X, topRightNode.Y, direction, range - 1, result);
            }
        }

        if (direction is Direction.BotLeft or Direction.Left or Direction.TopLeft) {
            var leftNode = GetAdjacentNode(x, y, Direction.Left);
            if (leftNode != null) {
                result.Add(leftNode);
                GetSectorOfNodes(leftNode.X, leftNode.Y, direction, range - 1, result);
            }
        }
        
        if (direction is Direction.TopRight or Direction.Right or Direction.BotRight) {
            var rightNode = GetAdjacentNode(x, y, Direction.Right);
            if (rightNode != null) {
                result.Add(rightNode);
                GetSectorOfNodes(rightNode.X, rightNode.Y, direction, range - 1, result);
            }
        }

        if (direction is Direction.Left or Direction.BotLeft or Direction.BotRight) {
            var botLeftNode = GetAdjacentNode(x, y, Direction.BotLeft);
            if (botLeftNode != null) {
                result.Add(botLeftNode);
                GetSectorOfNodes(botLeftNode.X, botLeftNode.Y, direction, range - 1, result);
            }
        }
        
        if (direction is Direction.BotLeft or Direction.BotRight or Direction.Right) {
            var botRightNode = GetAdjacentNode(x, y, Direction.BotRight);
            if (botRightNode != null) {
                result.Add(botRightNode);
                GetSectorOfNodes(botRightNode.X, botRightNode.Y, direction, range - 1, result);
            }
        }
    }

    [Button]
    public void Dev_DetectObject() {
        for (int i=0; i<SIZE; i++) {
            for (int j=0; j<SIZE; j++) {
                if (nodes[i, j].obj != null) {
                    Debug.Log($"[{i},{j}] has object");
                }
            }
        }
    }
}