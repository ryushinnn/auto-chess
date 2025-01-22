using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Indicator : MonoBehaviour {
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material normalMat;
    [SerializeField] Material highlightMat;
    
    public void SetHighlight(bool highlight) {
        meshRenderer.sharedMaterial = highlight ? highlightMat : normalMat;
    }
}