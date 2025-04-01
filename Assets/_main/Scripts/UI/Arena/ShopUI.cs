using System;
using System.Collections.Generic;
using RExt.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopUI : BaseUI {
    [SerializeField] Shop_Hero[] heroes;
    [SerializeField] Button refreshButton;
    [SerializeField] TMP_Text refreshCostText;
    [SerializeField] GameObject lockedMark;
    [SerializeField] Button lockButton;

    void Start() {
        refreshButton.onClick.AddListener(Refresh);
        lockButton.onClick.AddListener(Lock);
        refreshCostText.text = GameConfigs.REFRESH_COST.ToString();
        GameManager.Instance.Shop.onRefresh += UpdateHeroes;
        lockedMark.SetActive(false);
    }

    void UpdateHeroes(HeroTrait[] traits) {
        for (int i = 0; i < heroes.Length; i++) {
            heroes[i].Initialize(traits[i]);
        }
    }

    void Refresh() {
        GameManager.Instance.Shop.Refresh();
        GameManager.Instance.Shop.SwitchLock(false);
        lockedMark.SetActive(false);
    }

    void Lock() {
        GameManager.Instance.Shop.SwitchLock();
        lockedMark.SetActive(!lockedMark.activeSelf);
    }
}