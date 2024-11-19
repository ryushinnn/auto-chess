using UnityEngine;

public class Node : MonoBehaviour {
    public int X { get; private set; }
    public int Y { get; private set; }
    
    public void SaveIndex(int x, int y) {
        X = x;
        Y = y;
    }
}