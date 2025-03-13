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
    [SerializeField] float updateRate;

    [SerializeField, ReadOnly] int mapRow;
    [SerializeField, ReadOnly] int mapColumn;
    [SerializeField, ReadOnly] int deckSize;
    HexCell[,] hexCells;
    [SerializeField, ReadOnly] Indicator selectedCell;
    HexCell[] selectedCells;
    SquareCell[] squareCells;
    Camera cam;
    LayerMask layerMask;
    float updateInterval;
    float updateTimer;
    
    Indicator highlightHexCell;
    Indicator notAvailableHexCell;
    Indicator highlightSquareCell;
    Indicator notAvailableSquareCell;
    
    
    
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
    
    void Update() {
        HandleHighlightCell();   
    }

    public void SpawnHexIndicators(MapNode[,] nodes, float width, float height) {
        mapRow = nodes.GetLength(0);
        mapColumn = nodes.GetLength(1);
        hexCells = new HexCell[mapRow, mapColumn];
        for (int i = 0; i < mapRow; i++) {
            for (int j = 0; j < mapColumn; j++) {
                var cell = Instantiate(hexCell, hexParent);
                cell.transform.localPosition = new Vector3(nodes[i, j].Position.x, 0, nodes[i, j].Position.z);
                cell.transform.localScale = new Vector3(width, 1, height);
                cell.name = $"Hex[{i},{j}]";
                cell.dev_SaveIndex(i,j);
                hexCells[i, j] = cell;
            }
        }
    }

    public void SpawnSquareIndicators(DeckNode[] nodes, float width, float height) {
        deckSize = nodes.Length;
        squareCells = new SquareCell[deckSize];
        for (int i = 0; i < deckSize; i++) {
            var cell = Instantiate(squareCell, squareParent);
            cell.transform.localPosition = new Vector3(nodes[i].Position.x, 0, nodes[i].Position.z);
            cell.transform.localScale = new Vector3(width, 1, height);
            cell.name = $"Square[{i}]";
            cell.dev_SaveIndex(i);
            squareCells[i] = cell;
        }
    }

    public void MarkAsNonEmpty(Node node, bool nonEmpty) {
        if (node is MapNode mNode) {
            hexCells[mNode.X, mNode.Y].SetNonEmpty(nonEmpty);
        }
        else if (node is DeckNode dNode) {
            squareCells[dNode.Index].SetNonEmpty(nonEmpty);
        }
    }

    public void MarkAsNotAvailable(bool value, Node node = null) {
        notAvailableHexCell?.SetNotAvailable(false);
        notAvailableSquareCell?.SetNotAvailable(false);
        
        if (value) {
            if (node is MapNode mNode) {
                notAvailableHexCell = hexCells[mNode.X, mNode.Y];
                notAvailableHexCell.SetNotAvailable(true);
            }
            else if (node is DeckNode dNode) {
                notAvailableSquareCell = squareCells[dNode.Index];
                notAvailableSquareCell.SetNotAvailable(true);
            }
        }
    }

    public void Highlight(bool value, Node node = null) {
        highlightHexCell?.SetHighlight(false);
        highlightSquareCell?.SetHighlight(false);

        if (value) {
            if (node is MapNode mNode) {
                highlightHexCell = hexCells[mNode.X, mNode.Y];
                highlightHexCell.SetHighlight(true);
            }
            else if (node is DeckNode dNode) {
                highlightSquareCell = squareCells[dNode.Index];
                highlightSquareCell.SetHighlight(true);
            }
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
            // var ray = cam.ScreenPointToRay(Input.mousePosition);
            // if (Physics.Raycast(ray, out var hit, 1000, layerMask)) {
            //     var minDist = Mathf.Infinity;
            //     for (int i = 0; i < hexCells.GetLength(0); i++) {
            //         for (int j = 0; j < hexCells.GetLength(1); j++) {
            //             var dist = Vector3.Distance(hexCells[i, j].transform.position, hit.point);
            //             if (dist < minDist) {
            //                 minDist = dist;
            //                 selectedCell = hexCells[i, j];     
            //             }
            //         }
            //     }
            //     
            //     for (int i=0; i<9; i++) {
            //         var dist = Vector3.Distance(squareCells[i].transform.position, hit.point);
            //         if (dist < minDist) {
            //             minDist = dist;
            //             selectedCell = squareCells[i];
            //         }
            //     }
            //     
            //     selectedCell?.SetHighlight(true);
            //
            //     if (selectedCell != null && selectedCell is HexCell hex) {
            //         hero.transform.position = selectedCell.transform.position;
            //         
            //         if (selectNodeMethod == SelectNodeMethod.Adjacent) {
            //             selectedCells = Map.Instance.GetCircle(hex.X, hex.Y, range).Select(GetHexIndicator).ToArray();
            //         }
            //         else if (selectNodeMethod == SelectNodeMethod.Line) {
            //             selectedCells = Map.Instance.GetLine(hex.X, hex.Y, direction, range).Select(GetHexIndicator).ToArray();
            //         }
            //         else if (selectNodeMethod == SelectNodeMethod.Sector) {
            //             selectedCells = Map.Instance.GetSector(hex.X, hex.Y, direction, range).Select(GetHexIndicator).ToArray();
            //         }
            //
            //         if (selectedCells != null) {
            //             foreach (var cell in selectedCells) {
            //                 cell?.SetHighlight(true);
            //             }
            //         }
            //     }
            // }
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
        if (mapNode == null || mapNode.X < 0 || mapNode.X >= mapRow || mapNode.Y < 0 || mapNode.Y >= mapColumn) {
            return null;
        }
        
        return hexCells[mapNode.X, mapNode.Y];
    }

    [Button]
    void TurnOnLabels() {
        for (int i=0; i<mapRow; i++) {
            for (int j=0; j<mapColumn; j++) {
                hexCells[i, j].dev_SwitchLabel(true);
            }
        }
        for (int i=0;i<deckSize;i++) {
            squareCells[i].dev_SwitchLabel(true);
        }
    }
    
    [Button]
    void TurnOffLabels() {
        for (int i=0; i<mapRow; i++) {
            for (int j=0; j<mapColumn; j++) {
                hexCells[i, j].dev_SwitchLabel(false);
            }
        }
        for (int i=0;i<deckSize;i++) {
            squareCells[i].dev_SwitchLabel(false);
        }
    }
}