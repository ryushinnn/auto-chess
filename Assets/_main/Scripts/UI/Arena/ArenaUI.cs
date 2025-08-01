using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArenaUI : BaseUI {
    [SerializeField] Button lineUpButton;
    [SerializeField] Button inventoryButton;
    [SerializeField] Button shopButton;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text xpText;
    [SerializeField] TMP_Text lineUpText;

    void Start() {
        shopButton.onClick.AddListener(SwitchShop);
        lineUpButton.onClick.AddListener(OpenLineUp);
        inventoryButton.onClick.AddListener(OpenInventory);
    }

    public void UpdateLevelText(int level) {
        levelText.text = $"Lv.{level}";
    }

    public void UpdateXpText(int current, int next) {
        xpText.text = $"{current}/{next}";
    }
    
    public void UpdateLineUpText(int current, int max) {
        lineUpText.text = $"{current}/{max}";
        lineUpText.color = current >= max ? Color.red : Color.white;
    }

    void SwitchShop() {
        if (ArenaUIManager.Instance.Shop.gameObject.activeSelf) {
            ArenaUIManager.Instance.Shop.Close();
        }
        else {
            ArenaUIManager.Instance.Shop.Open();
        }
    }

    void OpenLineUp() {
        ArenaUIManager.Instance.LineUp.Open();
        ArenaUIManager.Instance.Inventory.Close();
    }

    void OpenInventory() {
        ArenaUIManager.Instance.LineUp.Close();
        ArenaUIManager.Instance.Inventory.Open();
    }
}