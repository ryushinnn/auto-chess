using System;
using System.Collections.Generic;
using DG.Tweening;
using RExt.Core;
using RExt.Utils.ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;

public class HpTextSpawner : Singleton<HpTextSpawner> {
    [SerializeField] HpText hpTextPrefab;
    
    ObjectPool<HpText> pool;
    
    const float LEFT_POS = -0.5f;
    const float RIGHT_POS = 0.5f;
    const float POS_Y_GAP = 0.5f;
    
    protected override void OnAwake() {
        pool = ObjectPools.Instance.CreatePool(hpTextPrefab, 10, 20, 5);
    }

    public HpText SpawnHpTextAsDamage(Transform parent, Damage damage) {
        var hpText = SpawnHpText(parent, new Vector3(RIGHT_POS,0,0));
        hpText.transform.SetAsLastSibling();
        hpText.SetAsDamage(damage.value, damage.type, damage.crit);
        return hpText;
    }

    public HpText[] SpawnHpTextAsDamage(Transform parent, List<Damage> damages) {
        var hpTexts = new HpText[damages.Count];
        for (int i = 0; i < damages.Count; i++) {
            var hpText = SpawnHpText(parent, new Vector3(RIGHT_POS, POS_Y_GAP * i,0));
            hpText.transform.SetAsLastSibling();
            hpText.SetAsDamage(damages[i].value, damages[i].type, damages[i].crit);
            hpTexts[i] = hpText;
        }

        return hpTexts;
    }
    
    public HpText SpawnHpTextAsHeal(Transform parent, float amount) {
        var hpText = SpawnHpText(parent, new Vector3(LEFT_POS,0,0));
        hpText.transform.SetAsLastSibling();
        hpText.SetAsHeal(amount);
        return hpText;
    }

    HpText SpawnHpText(Transform parent, Vector3 pos) {
        var hpText = pool.Get();
        hpText.transform.SetParent(parent);
        hpText.transform.localPosition = pos;
        hpText.transform.localEulerAngles = Vector3.zero;
        DOVirtual.DelayedCall(1, () => {
            pool.Return(hpText);
        });
        return hpText;
    }
}