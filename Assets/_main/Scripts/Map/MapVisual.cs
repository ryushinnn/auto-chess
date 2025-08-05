using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Pathfinding;
using RExt.Patterns.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

public class MapVisual : Singleton<MapVisual> {
    public int Layer => raycastTarget.layer;
    
    [SerializeField] Transform hexParent;
    [SerializeField] HexCell hexCell;
    [SerializeField] Transform squareParent;
    [SerializeField] SquareCell squareCell;
    [SerializeField] GameObject raycastTarget;

    [SerializeField, ReadOnly] int mapRow;
    [SerializeField, ReadOnly] int mapColumn;
    [SerializeField, ReadOnly] int deckSize;
    HexCell[,] hexCells;
    [SerializeField, ReadOnly] Cell selectedCell;
    HexCell[] selectedCells;
    SquareCell[] squareCells;
    
    Cell highlightHexCell;
    Cell notAvailableHexCell;
    Cell highlightSquareCell;
    Cell notAvailableSquareCell;

    public void Initialize() {
        Map.Instance.SpawnNodes(SpawnHexCells);
        Deck.Instance.SpawnNodes(SpawnSquareCells);
        GameManager.Instance.Stages.OnChangePhase += OnChangePhase;
    }

    void SpawnHexCells(MapNode[,] nodes, float width, float height) {
        mapRow = nodes.GetLength(0);
        mapColumn = nodes.GetLength(1);
        hexCells = new HexCell[mapRow, mapColumn];
        for (int i = 0; i < mapRow; i++) {
            for (int j = 0; j < mapColumn; j++) {
                var cell = Instantiate(hexCell, hexParent);
                cell.transform.localPosition = new Vector3(nodes[i, j].WorldPosition.x, 0, nodes[i, j].WorldPosition.z);
                cell.transform.localScale = new Vector3(width, 1, height);
                cell.name = $"Hex[{i},{j}]";
                cell.dev_SaveIndex(i,j);
                hexCells[i, j] = cell;
            }
        }
    }

    void SpawnSquareCells(DeckNode[] nodes, float width, float height) {
        deckSize = nodes.Length;
        squareCells = new SquareCell[deckSize];
        for (int i = 0; i < deckSize; i++) {
            var cell = Instantiate(squareCell, squareParent);
            cell.transform.localPosition = new Vector3(nodes[i].WorldPosition.x, 0, nodes[i].WorldPosition.z);
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
            squareCells[dNode.LinePosition].SetNonEmpty(nonEmpty);
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
                notAvailableSquareCell = squareCells[dNode.LinePosition];
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
                highlightSquareCell = squareCells[dNode.LinePosition];
                highlightSquareCell.SetHighlight(true);
            }
        }
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

    public HexCell GetHexCell(int x, int y) {
        return hexCells[x, y];
    }

    void OnChangePhase(MatchPhase phase) {
        switch (phase) {
            case MatchPhase.Preparation:
                for (int i=mapRow/2; i<mapRow; i++) {
                    for (int j=0; j<mapColumn; j++) {
                        hexCells[i, j].gameObject.SetActive(false);
                    }
                }
                break;
            
            case MatchPhase.Battle:
                for (int i=mapRow/2; i<mapRow; i++) {
                    for (int j=0; j<mapColumn; j++) {
                        hexCells[i, j].gameObject.SetActive(true);
                    }
                }
                break;
            
        }
    }
}