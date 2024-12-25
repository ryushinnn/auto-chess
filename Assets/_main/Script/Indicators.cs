using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class Indicators : MonoBehaviour {
    [SerializeField] Map map;
    [SerializeField] Transform hexParent;
    [SerializeField] HexIndicator hexCell;
    [SerializeField] Transform squareParent;
    [SerializeField] SquareIndicator squareCell;
    [SerializeField] float squareWidth;
    [SerializeField] float squareHeight;
    [SerializeField] float updateRate;

    [SerializeField, ReadOnly] int row;
    [SerializeField, ReadOnly] int column;
    HexIndicator[,] hexCells;
    [SerializeField, ReadOnly] Indicator selectedCell;
    HexIndicator[] selectedCells;
    SquareIndicator[] squareCells;
    Camera cam;
    LayerMask layerMask;
    float updateInterval;
    float updateTimer;
    
    public SelectNodeMethod selectNodeMethod;
    public int range;
    public Direction direction;
    public Seeker hero;
    
    void Awake() {
        cam = Camera.main;
        layerMask = LayerMask.GetMask("RaycastOnly");
        updateInterval = 1 / updateRate;
        updateTimer = 0;
    }
    
    void Start() {
        SpawnSquareIndicators();
    }
    
    void Update() {
        HandleHighlightCell();   
    }

    public void SpawnHexIndicators(Node[,] nodes) {
        row = nodes.GetLength(0);
        column = nodes.GetLength(1);
        hexCells = new HexIndicator[row, column];
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < column; j++) {
                var cell = Instantiate(hexCell, hexParent);
                cell.transform.localPosition = nodes[i, j].transform.localPosition;
                cell.transform.localScale = nodes[i, j].transform.localScale;
                cell.name = $"Hex[{i},{j}]";
                cell.SaveIndex(i,j);
                hexCells[i, j] = cell;
            }
        }

    }

    void SpawnSquareIndicators() {
        squareCells = new SquareIndicator[9];
        for (int i = 0; i < 9; i++) {
            var cell = Instantiate(squareCell, squareParent);
            cell.transform.transform.localPosition = new Vector3((i-4)*squareWidth, 0, 0);
            cell.transform.transform.localScale = new Vector3(squareWidth, 1, squareHeight);
            cell.name = $"Square[{i}]";
            squareCells[i] = cell;
        }
    }
    
    void HandleHighlightCell() {
        if (updateTimer > 0) {
            updateTimer -= Time.deltaTime;
            return;
        }

        updateTimer = updateInterval;
        selectedCell?.SetHighlight(false);
        selectedCell = null;
        if (selectedCells != null) {
            foreach (var cell in selectedCells) {
                cell?.SetHighlight(false);
            }
        }
        
        if (Input.GetMouseButton(0)) {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 1000, layerMask)) {
                var minDist = Mathf.Infinity;
                for (int i = 0; i < hexCells.GetLength(0); i++) {
                    for (int j = 0; j < hexCells.GetLength(1); j++) {
                        var dist = Vector3.Distance(hexCells[i, j].transform.position, hit.point);
                        if (dist < minDist) {
                            minDist = dist;
                            selectedCell = hexCells[i, j];     
                        }
                    }
                }
                
                for (int i=0; i<9; i++) {
                    var dist = Vector3.Distance(squareCells[i].transform.position, hit.point);
                    if (dist < minDist) {
                        minDist = dist;
                        selectedCell = squareCells[i];
                    }
                }
                
                selectedCell?.SetHighlight(true);

                if (selectedCell != null && selectedCell is HexIndicator hex) {
                    hero.transform.position = selectedCell.transform.position;
                    
                    if (selectNodeMethod == SelectNodeMethod.Adjacent) {
                        selectedCells = map.GetAdjacentNodes(hex.X, hex.Y, range).Select(GetHexIndicator).ToArray();
                    }
                    else if (selectNodeMethod == SelectNodeMethod.Line) {
                        selectedCells = map.GetLineOfNodes(hex.X, hex.Y, direction, range).Select(GetHexIndicator).ToArray();
                    }
                    else if (selectNodeMethod == SelectNodeMethod.Sector) {
                        selectedCells = map.GetSectorOfNodes(hex.X, hex.Y, direction, range).Select(GetHexIndicator).ToArray();
                    }

                    if (selectedCells != null) {
                        foreach (var cell in selectedCells) {
                            cell?.SetHighlight(true);
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1)) {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 1000, layerMask)) {
                Vector3 destination = default;
                var minDist = Mathf.Infinity;
                for (int i = 0; i < hexCells.GetLength(0); i++) {
                    for (int j = 0; j < hexCells.GetLength(1); j++) {
                        var dist = Vector3.Distance(hexCells[i, j].transform.position, hit.point);
                        if (dist < minDist) {
                            minDist = dist;
                            destination = hexCells[i, j].transform.position;     
                        }
                    }
                }

                hero.StartPath(hero.transform.position, destination, path => {
                    var sequence = DOTween.Sequence();
                    foreach (var waypoint in path.vectorPath) {
                        sequence.Append(hero.transform.DOMove(waypoint, 0.5f).SetEase(Ease.Linear));
                    }
                });
            }
        }
    }

    HexIndicator GetHexIndicator(Node node) {
        if (node == null || node.X < 0 || node.X >= row || node.Y < 0 || node.Y >= column) {
            return null;
        }
        
        return hexCells[node.X, node.Y];
    }
}