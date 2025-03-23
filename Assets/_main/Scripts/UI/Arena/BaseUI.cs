using UnityEngine;

public abstract class BaseUI : MonoBehaviour {
    public void Open() {
        gameObject.SetActive(true);
    }
    
    public void Close() {
        gameObject.SetActive(false);
    }
}