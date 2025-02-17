using System;
using UnityEngine;

public class HeroPicker : MonoBehaviour {

    void OnMouseDown() {
        Debug.Log("mouse down");
    }

    void OnMouseDrag() {
        Debug.Log("mouse drag");
    }

    void OnMouseUp() {
        Debug.Log("mouse up");
    }
}