using System;
using RExt.Utils;
using UnityEngine;

public class RayInputManager : MonoBehaviour {
    void Update() {
        if (Input.GetMouseButtonDown(0) && !Utils.IsOverUI()) {
            ArenaUIManager.Instance.Collapse();
        }
    }
}