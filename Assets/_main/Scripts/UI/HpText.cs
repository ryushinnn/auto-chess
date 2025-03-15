using DG.Tweening;
using RExt.Utils.ObjectPool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpText : MonoBehaviour, IPoolable{
    [SerializeField] TMP_Text text;
    [SerializeField] Image critMark;
    [SerializeField] CanvasGroup cg;
    [SerializeField] Color physicalDamageColor;
    [SerializeField] Color magicalDamageColor;
    [SerializeField] Color trueDamageColor;
    [SerializeField] Color healColor;

    Sequence seq;
    
    const float NORMAL_SIZE = 0.4f;
    const float CRIT_SIZE = 0.5f;
    

    public void SetAsDamage(float damage, DamageType type, bool crit) {
        text.SetText(((int)damage).ToString());
        text.fontSize = crit ? CRIT_SIZE : NORMAL_SIZE;
        var color = type switch {
            DamageType.Physical => physicalDamageColor,
            DamageType.Magical => magicalDamageColor,
            DamageType.True => trueDamageColor,
        };
        text.color = color;
        critMark.color = color;
        critMark.gameObject.SetActive(crit);
    }
    
    public void SetAsHeal(float amount) {
        text.SetText($"+{((int)amount).ToString()}");
        text.fontSize = NORMAL_SIZE;
        text.color = healColor;
        critMark.gameObject.SetActive(false);
    }
    
    public void Activate() {
        gameObject.SetActive(true);
        
        seq?.Kill();
        seq = DOTween.Sequence();
        transform.localScale = 2f * Vector3.one;
        cg.alpha = 1f;
        var targetY = 1f;
        seq.Append(transform.DOScale(1, 0.25f))
            .Insert(0.1f, transform.DOLocalMoveY(targetY, 0.5f))
            .Insert(0.4f,cg.DOFade(0, 0.2f));
    }
    public void Deactivate() {
        gameObject.SetActive(false);
    }
}