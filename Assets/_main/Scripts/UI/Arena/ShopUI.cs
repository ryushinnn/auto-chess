using System;
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

    void Start() {
        refreshButton.onClick.AddListener(Refresh);
        refreshCostText.text = GameConfigs.REFRESH_COST.ToString();
    }

    [Button]
    public void Refresh() {
        foreach (var hero in heroes) {
            var levelConfig = GameConfigs.LEVEL_CONFIGS[GameManager.Instance.Level - 1];
            var price = GetRandomPrices(levelConfig.rates);
            var matchedHeroes = HeroTraitDB.Instance.FindAll(e => e.price == price && !e.summoned);
            var randomHero = matchedHeroes[Random.Range(0, matchedHeroes.Count)];
            hero.Initialize(randomHero);
        }
    }
    
    int GetRandomPrices(int[] rates) {
        Debug.Log(rates.ToString());
        var random = Random.value;
        Debug.Log(random);
        var totalRate = 0f;
        for (int i = 0; i < rates.Length; i++) {
            totalRate += rates[i] / 100f;
            if (random <= totalRate) {
                Debug.Log(i);
                return GameConfigs.HERO_PRICES[i];
            }
        }
        return GameConfigs.HERO_PRICES[0];
    }
}