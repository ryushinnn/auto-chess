using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArenaUI : BaseUI {
    [SerializeField] Button lineUpButton;
    [SerializeField] Button inventoryButton;
    [SerializeField] Button shopButton;
    [SerializeField] TMP_Text coinsText;
    [SerializeField] Button buyXpButton;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text xpText;
    [SerializeField] TMP_Text lineUpText;
    [SerializeField] Image timeLeftImage;
    [SerializeField] TMP_Text roundAndWaveText;
    [SerializeField] TMP_Text timeLeftText;

    void Start() {
        shopButton.onClick.AddListener(SwitchShop);
        lineUpButton.onClick.AddListener(OpenLineUp);
        inventoryButton.onClick.AddListener(OpenInventory);
        buyXpButton.onClick.AddListener(BuyXp);
    }
    
    public void UpdateCoinsText(int coins) {
        coinsText.text = $"{coins}<sprite name=coin>";
    }

    public void UpdateLevelText(int level) {
        levelText.text = $"Lv.{level}";
    }

    public void UpdateXpText(int current, int next, bool maxLevel) {
        xpText.text = maxLevel ? "MAX" : $"{current}/{next}";
    }
    
    public void UpdateLineUpText(int current, int max) {
        lineUpText.text = $"{current}/{max}";
        lineUpText.color = current >= max ? Color.red : Color.white;
    }

    public void UpdateRoundAndWave(int round, int wave) {
        roundAndWaveText.text = $"{round + 1}-{wave + 1}";
    }

    public void UpdateTimeLeft(float current, float total) {
        timeLeftImage.fillAmount = current / total;
        timeLeftText.text = $"{Mathf.CeilToInt(current)}";
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

    void BuyXp() {
        if (GameManager.Instance.Inventory.Coins < GameConfigs.XP_GAIN_PER_PURCHASE) return;
        if (GameManager.Instance.Level.GainXp(GameConfigs.XP_GAIN_PER_PURCHASE)) {
            GameManager.Instance.Inventory.SpendCoins(GameConfigs.XP_GAIN_PER_PURCHASE);
        }
    }
}