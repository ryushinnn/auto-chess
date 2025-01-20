using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class HexCell : Indicator {
    [SerializeField] TMP_Text label;
    
    public int X { get; private set; }

    public int Y { get; private set; }

    public void SaveIndex(int x, int y) {
        X = x;
        Y = y;
        label.text = $"({x},{y})";
        label.gameObject.SetActive(false);
    }

    public void Dev_SwitchLabel(bool state) {
        label.gameObject.SetActive(state);
    }
}