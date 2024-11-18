using System;
using UnityEngine;

public class Indicator : MonoBehaviour {
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] Material _normalMat;
    [SerializeField] Material _highlightMat;
    
    public void SetHighlight(bool highlight) {
        _meshRenderer.sharedMaterial = highlight ? _highlightMat : _normalMat;
    }
}