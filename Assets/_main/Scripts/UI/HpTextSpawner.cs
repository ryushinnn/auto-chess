using System;
using DG.Tweening;
using RExt.Core;
using RExt.Utils.ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;

public class HpTextSpawner : Singleton<HpTextSpawner> {
    [SerializeField] HpText hpTextPrefab;
    
    ObjectPool<HpText> pool;
    
    const float RANDOM_X_MIN = -0.5f;
    const float RANDOM_X_MAX = 0.5f;
    
    protected override void OnAwake() {
        pool = ObjectPools.Instance.CreatePool(hpTextPrefab, 10, 20, 5);
        Debug.Assert(pool != null, "pool != null");
    }

    public HpText SpawnHpTextAsDamage(Transform parent, float damage, DamageType type, bool crit) {
        var hpText = SpawnHpText(parent);
        hpText.SetAsDamage(damage, type, crit);
        return hpText;
    }
    
    public HpText SpawnHpTextAsHeal(Transform parent, float amount) {
        var hpText = SpawnHpText(parent);
        hpText.SetAsHeal(amount);
        return hpText;
    }

    HpText SpawnHpText(Transform parent) {
        var hpText = pool.Get();
        hpText.transform.SetParent(parent);
        hpText.transform.localPosition = new Vector3(Random.Range(RANDOM_X_MIN, RANDOM_X_MAX), 0, 0);
        hpText.transform.localEulerAngles = Vector3.zero;
        DOVirtual.DelayedCall(1, () => {
            pool.Return(hpText);
        });
        return hpText;
    }
}