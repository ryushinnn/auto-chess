using System;
using UnityEngine;

public class Indicators : MonoBehaviour {
    [SerializeField] Transform _hexParent;
    [SerializeField] Indicator _hexCell;
    [SerializeField] float _hexWidth;
    [SerializeField] float _hexHeight;

    [SerializeField] Transform _squareParent;
    [SerializeField] Indicator _squareCell;
    [SerializeField] float _squareWidth;
    [SerializeField] float _squareHeight;
    
    Indicator[,] _hexCells;
    Indicator[] _squareCells;
    Indicator _currentHighlightCell;
    Camera _camera;
    LayerMask _layerMask;
    
    const int SIZE = 8;

    void Awake() {
        _camera = Camera.main;
        _layerMask = LayerMask.GetMask("RaycastOnly");
    }

    void Start() {
        _hexCells = new Indicator[SIZE/2,SIZE];
        var rootOffset = new Vector3(_hexWidth/2, 0, _hexHeight/2 - _hexHeight/8);
        for (int i = 0; i < SIZE/2; i++) {
            for (int j=0; j<SIZE; j++) {
                var columnOffset = new Vector3(i % 2 == 0 ? _hexWidth / 4 : _hexWidth / -4, 0, 0);
                var cell = Instantiate(_hexCell, _hexParent);
                cell.transform.localPosition = new Vector3((j-SIZE/2) * _hexWidth, 0 , (i-SIZE/2) * _hexHeight * 3/4)
                                              + rootOffset
                                              + columnOffset;
                cell.transform.localScale = new Vector3(_hexWidth, 1, _hexHeight);
                _hexCells[i, j] = cell;
            }
        }

        _squareCells = new Indicator[9];
        for (int i = 0; i < 9; i++) {
            var cell = Instantiate(_squareCell, _squareParent);
            cell.transform.transform.localPosition = new Vector3((i-4)*_squareWidth, 0, 0);
            cell.transform.transform.localScale = new Vector3(_squareWidth, 1, _squareHeight);
            _squareCells[i] = cell;
        }
    }

    void Update() {
        if (Input.GetMouseButton(0)) {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 1000, _layerMask)) {
                _currentHighlightCell?.SetHighlight(false);
                var minDist = Mathf.Infinity;
                for (int i = 0; i < SIZE/2; i++) {
                    for (int j = 0; j < SIZE; j++) {
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