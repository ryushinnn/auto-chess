using System;
using UnityEngine;

public class Indicators : MonoBehaviour {
    [SerializeField] Transform _hexParent;
    [SerializeField] Indicator _hexCell;
    [SerializeField] Transform _squareParent;
    [SerializeField] Indicator _squareCell;
    [SerializeField] float _squareWidth;
    [SerializeField] float _squareHeight;
    
    Indicator[,] _hexCells;
    Indicator[] _squareCells;
    Indicator _currentHighlightCell;
    Camera _camera;
    LayerMask _layerMask;
    
    void Awake() {
        _camera = Camera.main;
        _layerMask = LayerMask.GetMask("RaycastOnly");
    }
    
    void Start() {
        SpawnSquareIndicators();
    }
    
    void Update() {
        HandleHighlightCell();   
    }

    public void SpawnHexIndicators(GameObject[,] nodes) {
        var row = nodes.GetLength(0) / 2;
        var col = nodes.GetLength(1);
        _hexCells = new Indicator[row, col];
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                var cell = Instantiate(_hexCell, _hexParent);
                cell.transform.localPosition = nodes[i, j].transform.localPosition;
                cell.transform.localScale = nodes[i, j].transform.localScale;
                _hexCells[i, j] = cell;
            }
        }

    }

    void SpawnSquareIndicators() {
        _squareCells = new Indicator[9];
        for (int i = 0; i < 9; i++) {
            var cell = Instantiate(_squareCell, _squareParent);
            cell.transform.transform.localPosition = new Vector3((i-4)*_squareWidth, 0, 0);
            cell.transform.transform.localScale = new Vector3(_squareWidth, 1, _squareHeight);
            _squareCells[i] = cell;
        }
    }
    
    void HandleHighlightCell() {
        if (Input.GetMouseButton(0)) {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 1000, _layerMask)) {
                _currentHighlightCell?.SetHighlight(false);
                var minDist = Mathf.Infinity;
                for (int i = 0; i < _hexCells.GetLength(0); i++) {
                    for (int j = 0; j < _hexCells.GetLength(1); j++) {
                        var dist = Vector3.Distance(_hexCells[i, j].transform.position, hit.point);
                        if (dist < minDist) {
                            minDist = dist;
                            _currentHighlightCell = _hexCells[i, j];     
                        }
                    }
                }
                
                for (int i=0; i<9; i++) {
                    var dist = Vector3.Distance(_squareCells[i].transform.position, hit.point);
                    if (dist < minDist) {
                        minDist = dist;
                        _currentHighlightCell = _squareCells[i];
                    }
                }
                
                _currentHighlightCell?.SetHighlight(true);
            } else {
                _currentHighlightCell?.SetHighlight(false);
            }
        } else {
            _currentHighlightCell?.SetHighlight(false);
        }
    }
}