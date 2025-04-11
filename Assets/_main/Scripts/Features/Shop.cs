using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shop : MonoBehaviour {
    public event Action<HeroTrait[]> onRefresh;
    
    [SerializeField, ReadOnly] HeroTrait[] heroes = new HeroTrait[SHOP_SLOTS_COUNT];
    [SerializeField, ReadOnly] bool lockAutoRefresh;
    
    const int SHOP_SLOTS_COUNT = 5;
    
    public void Refresh() {
        for (int i=0; i<SHOP_SLOTS_COUNT; i++) {
            var levelConfig = GameConfigs.LEVEL_CONFIGS[GameManager.Instance.Level - 1];
            var price = GetRandomPrices(levelConfig.rates);
            var matchedHeroes = HeroTraitDB.Instance.FindAll(e => e.price == price && !e.summoned);
            var randomHero = matchedHeroes[Random.Range(0, matchedHeroes.Count)];
            heroes[i] = randomHero;
        }
        onRefresh?.Invoke(heroes);
    }

    public void Purchase(HeroTrait hero, out bool success) {
        success = GameManager.Instance.LineUp.Add(hero);
    }

    public void SwitchLock(bool? value = null) {
        if (value.HasValue) {
            lockAutoRefresh = value.Value;
            return;
        }

        lockAutoRefresh = !lockAutoRefresh;
    }
    
    int GetRandomPrices(int[] rates) {
        var random = Random.value;
        var totalRate = 0f;
        for (int i = 0; i < rates.Length; i++) {
            totalRate += rates[i] / 100f;
            if (random <= totalRate) {
                return GameConfigs.HERO_PRICES[i];
            }
        }
        return GameConfigs.HERO_PRICES[0];
    }
}