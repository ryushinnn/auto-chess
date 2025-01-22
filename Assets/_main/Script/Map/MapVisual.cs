using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Pathfinding;
using RExt.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public class MapVisual : Singleton<MapVisual> {
    [SerializeField] Transform hexParent;
    [SerializeField] HexCell hexCell;
    [SerializeField] Transform squareParent;
    [SerializeField] SquareCell squareCell;
    [SerializeField] float squareWidth;
    [SerializeField] float squareHeight;
    [SerializeField] float updateRate;

    [SerializeField, ReadOnly] int row;
    [SerializeField, ReadOnly] int column;
    HexCell[,] hexCells;
    [SerializeField, ReadOnly] Indicator selectedCell;
    HexCell[] selectedCells;
    SquareCell[] squareCells;
    Camera cam;
    LayerMask layerMask;
    float updateInterval;
    float updateTimer;
    
    public SelectNodeMethod selectNodeMethod;
    public int range;
    public Direction direction;
    public Hero hero;
    
    protected override void OnAwake() {
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

    public void SpawnHexIndicators(MapNode[,] nodes, float width, float height) {
        row = nodes.GetLength(0);
        column = nodes.GetLength(1);
        hexCells = new HexCell[row, column];
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < column; j++) {
                var cell = Instantiate(hexCell, hexParent);
                cell.transform.localPosition = new Vector3(nodes[i, j].Position.x, 0, nodes[i, j].Position.z);
                cell.transform.localScale = new Vector3(width, 1, height);
                cell.name = $"Hex[{i},{j}]";
                cell.SaveIndex(i,j);
                hexCells[i, j] = cell;
            }
        }

    }

    public void Highlight(MapNode node, bool active) {
        hexCells[node.X, node.Y].SetHighlight(active);
    }

    void SpawnSquareIndicators() {
        squareCells = new SquareCell[9];
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

                if (selectedCell != null && selectedCell is HexCell hex) {
                    hero.transform.position = selectedCell.transform.position;
                    
                    if (selectNodeMethod == SelectNodeMethod.Adjacent) {
                        selectedCells = Map.Instance.GetAdjacentNodes(hex.X, hex.Y, range).Select(GetHexIndicator).ToArray();
                    }
                    else if (selectNodeMethod == SelectNodeMethod.Line) {
                        selectedCells = Map.Instance.GetLineOfNodes(hex.X, hex.Y, direction, range).Select(GetHexIndicator).ToArray();
                    }
                    else if (selectNodeMethod == SelectNodeMethod.Sector) {
                        selectedCells = Map.Instance.GetSectorOfNodes(hex.X, hex.Y, direction, range).Select(GetHexIndicator).ToArray();
                    }

                    if (selectedCells != null) {
                        foreach (var cell in selectedCells) {
                            cell?.SetHighlight(true);
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButton(1)) {
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

                // hero.GetAbility<HeroMovement>().StartMove(destination);
            }
        }
    }

    HexCell GetHexIndicator(MapNode mapNode) {
        if (mapNode == null || mapNode.X < 0 || mapNode.X >= row || mapNode.Y < 0 || mapNode.Y >= column) {
            return null;
        }
        
        return hexCells[mapNode.X, mapNode.Y];
    }

    [Button]
    void TurnOnLabels() {
        for (int i=0; i<row; i++) {
            for (int j=0; j<column; j++) {
                hexCells[i, j].Dev_SwitchLabel(true);
            }
        }
    }
    
    [Button]
    void TurnOffLabels() {
        for (int i=0; i<row; i++) {
            for (int j=0; j<column; j++) {
                hexCells[i, j].Dev_SwitchLabel(false);
            }
        }
    }
}