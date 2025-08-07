using Sirenix.OdinInspector;
using UnityEngine;

public class BattleHero : Hero {
    public TeamSide Side => side;
    public BattleHero Target => target;
    
    [SerializeField, ReadOnly] TeamSide side;
    [SerializeField, ReadOnly] BattleHero target;
    HeroBT bt;

    public void SetData(HeroTrait trait, HeroRank rank, TeamSide side) {
        this.trait = trait;
        this.side = side;
        this.rank = rank;
        name = $"({rank}){trait.id}";
        rankIcon.sprite = AssetDB.Instance.GetRankIcon(rank);
        SetUpModel();
        abilities.ForEach(x=>x.ResetAll());
    }
    
    public void FindTarget() {
        target = GameManager.Instance.BattleField.GetNearestOpponent(this);
    }

    public void SwitchBehaviour(bool value) {
        bt = value ? new HeroBT(this) : null;
    }

    protected override void Process() {
        base.Process();
        bt?.Evaluate();
    }
}