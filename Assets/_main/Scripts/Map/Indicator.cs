using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Indicator : MonoBehaviour {
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material normalMat;
    [SerializeField] Material highlightMat;
    [SerializeField] Material nonEmptyMat;
    [SerializeField] Material notAvailableMat;

    [SerializeField, ReadOnly] bool highlight;
    [SerializeField, ReadOnly] bool nonEmpty;
    [SerializeField, ReadOnly] bool notAvailable;
    
    public void SetHighlight(bool highlight) {
        this.highlight = highlight;
        UpdateMaterial();
    }
    
    public void SetNonEmpty(bool nonEmpty) {
        this.nonEmpty = nonEmpty;
        UpdateMaterial();
    }
    
    public void SetNotAvailable(bool notAvailable) {
        this.notAvailable = notAvailable;
        UpdateMaterial();
    }
    
    void UpdateMaterial() {
        meshRenderer.sharedMaterial = notAvailable ? notAvailableMat 
            : highlight ? highlightMat 
            : nonEmpty ? nonEmptyMat 
            : normalMat;
    }
}