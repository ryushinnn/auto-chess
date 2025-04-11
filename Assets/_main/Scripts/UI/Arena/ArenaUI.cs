using System;
using UnityEngine;
using UnityEngine.UI;

public class ArenaUI : BaseUI {
    [SerializeField] Button lineUpButton;
    [SerializeField] Button inventoryButton;
    [SerializeField] Button shopButton;

    void Start() {
        shopButton.onClick.AddListener(SwitchShop);
        lineUpButton.onClick.AddListener(OpenLineUp);
        inventoryButton.onClick.AddListener(OpenInventory);
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