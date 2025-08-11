using System;
using UnityEngine;

public class PositionBind : MonoBehaviour {
    Transform target;
    float offsetY;
    
    public void SetTarget(Transform target, float offsetY = 0) {
        this.target = target;
        this.offsetY = offsetY;
    }

    void Update() {
        if (target) {
            transform.position = new Vector3(0, offsetY, 0) + target.position;
        }
    }
}