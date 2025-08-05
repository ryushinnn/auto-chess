using DG.Tweening;
using RExt.Extensions;
using UnityEngine;

public class LineUpHero : Hero {
    Tween snapTween;
    
    public void SetData(HeroTrait trait, HeroRank rank) {
        this.trait = trait;
        this.rank = rank;
        name = $"({rank}){trait.id}";
        rankIcon.sprite = AssetDB.Instance.GetRankIcon(rank);
        SetUpModel();
        abilities.ForEach(x=>x.ResetAll());
    }
    
    public void Upgrade() {
        rank = rank.Next();
        name = $"({rank}){trait.id}";
        rankIcon.sprite = AssetDB.Instance.GetRankIcon(rank);
        Debug.Log($"{trait.id} upgraded to {rank}");
    }
    
    public void UpdatePosition(Node node, bool skipAnim = false) {
        snapTween?.Kill();
        if (skipAnim) {
            WorldPosition = node.WorldPosition;
        }
        else {
            snapTween = transform.DOMove(node.WorldPosition, 0.1f).SetEase(Ease.Linear);
        }
    }
}