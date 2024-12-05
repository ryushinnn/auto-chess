using System;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField, ReadOnly] int _row;
    [SerializeField, ReadOnly] int _column;
    HexIndicator[,] _hexCells;
    [SerializeField, ReadOnly] Indicator _selectedCell;
    HexIndicator[] _selectedCells;
    SquareIndicator[] _squareCells;
    Camera _camera;
    LayerMask _layerMask;
    float _updateInterval;
    float _updateTimer;
    
    public SelectNodeMethod selectNodeMethod;
    public int range;
    public Direction direction;
    
    void Awake() {
        _camera = Camera.main;
        _layerMask = LayerMask.GetMask("RaycastOnly");
        _updateInterval = 1 / updateRate;
        _updateTimer = 0;
    }
    
    void Start() {
        SpawnSquareIndicators();
    }
    
    void Update() {
        HandleHighlightCell();   
    }

    public void SpawnHexIndicators(Node[,] nodes) {
        _row = nodes.GetLength(0);
        _column = nodes.GetLength(1);
        _hexCells = new HexIndicator[_row, _column];
        for (int i = 0; i < _row; i++) {
            for (int j = 0; j < _column; j++) {
                var cell = Instantiate(hexCell, hexParent);
                cell.transform.localPosition = nodes[i, j].transform.localPosition;
                cell.transform.localScale = nodes[i, j].transform.localScale;
                cell.name = $"Hex[{i},{j}]";
                cell.SaveIndex(i,j);
                _hexCells[i, j] = cell;
            }
        }

    }

    void SpawnSquareIndicators() {
        _squareCells = new SquareIndicator[9];
        for (int i = 0; i < 9; i++) {
            var cell = Instantiate(squareCell, squareParent);
            cell.transform.transform.localPosition = new Vector3((i-4)*squareWidth, 0, 0);
            cell.transform.transform.localScale = new Vector3(squareWidth, 1, squareHeight);
            cell.name = $"Square[{i}]";
            _squareCells[i] = cell;
        }
    }
    
    void HandleHighlightCell() {
        if (_updateTimer > 0) {
            _updateTimer -= Time.deltaTime;
            return;
        }

        _updateTimer = _updateInterval;
        _selectedCell?.SetHighlight(false);
        _selectedCell = null;
        if (_selectedCells != null) {
            foreach (var cell in _selectedCells) {
                cell?.SetHighlight(false);
            }
        }
        
        if (Input.GetMouseButton(0)) {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 1000, _layerMask)) {
                var minDist = Mathf.Infinity;
                for (int i = 0; i < _hexCells.GetLength(0); i++) {
                    for (int j = 0; j < _hexCells.GetLength(1); j++) {
                        var dist = Vector3.Distance(_hexCells[i, j].transform.position, hit.point);
                        if (dist < minDist) {
                            minDist = dist;
                            _selectedCell = _hexCells[i, j];     
                        }
                    }
                }
                
                for (int i=0; i<9; i++) {
                    var dist = Vector3.Distance(_squareCells[i].transform.position, hit.point);
                    if (dist < minDist) {
                        minDist = dist;
                        _selectedCell = _squareCells[i];
                    }
                }
                
                _selectedCell?.SetHighlight(true);

                if (_selectedCell != null && _selectedCell is HexIndicator hex) {
                    if (selectNodeMethod == SelectNodeMethod.Adjacent) {
                        _selectedCells = map.GetAdjacentNodes(hex.X, hex.Y, range).Select(GetHexIndicator).ToArray();
                    }
                    else if (selectNodeMethod == SelectNodeMethod.Line) {
                        _selectedCells = map.GetLineOfNodes(hex.X, hex.Y, direction, range).Select(GetHexIndicator).ToArray();
                    }
                    else if (selectNodeMethod == SelectNodeMethod.Sector) {
                        _selectedCells = map.GetSectorOfNodes(hex.X, hex.Y, direction, range).Select(GetHexIndicator).ToArray();
                    }

                    if (_selectedCells != null) {
                        foreach (var cell in _selectedCells) {
                            cell?.SetHighlight(true);
                        }
                    }
                }
            }
        }
    }

    HexIndicator GetHexIndicator(Node node) {
        if (node == null || node.X < 0 || node.X >= _row || node.Y < 0 || node.Y >= _column) {
            return null;
        }
        
        return _hexCells[node.X, node.Y];
    }
}