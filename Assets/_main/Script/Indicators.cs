using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Indicators : MonoBehaviour {
    [SerializeField] Map _map;
    [SerializeField] Transform _hexParent;
    [SerializeField] HexIndicator _hexCell;
    [SerializeField] Transform _squareParent;
    [SerializeField] SquareIndicator _squareCell;
    [SerializeField] float _squareWidth;
    [SerializeField] float _squareHeight;
    [SerializeField] float _updateRate;

    [SerializeField, ReadOnly] int _row;
    [SerializeField, ReadOnly] int _column;
    HexIndicator[,] _hexCells;
    [SerializeField, ReadOnly] Indicator _selectedCell;
    HexIndicator[] _adjacentCells;
    SquareIndicator[] _squareCells;
    Camera _camera;
    LayerMask _layerMask;
    float _updateInterval;
    float _updateTimer;
    
    void Awake() {
        _camera = Camera.main;
        _layerMask = LayerMask.GetMask("RaycastOnly");
        _updateInterval = 1 / _updateRate;
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
                var cell = Instantiate(_hexCell, _hexParent);
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
            var cell = Instantiate(_squareCell, _squareParent);
            cell.transform.transform.localPosition = new Vector3((i-4)*_squareWidth, 0, 0);
            cell.transform.transform.localScale = new Vector3(_squareWidth, 1, _squareHeight);
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
        if (_adjacentCells != null) {
            foreach (var cell in _adjacentCells) {
                cell?.SetHighlight(false);
            }
        }
        _adjacentCells = new HexIndicator[6];
        
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
                    _adjacentCells = _map.GetAdjacentNodes(hex.X, hex.Y).Select(GetHexIndicator).ToArray();
                    foreach (var cell in _adjacentCells) {
                        cell?.SetHighlight(true);
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