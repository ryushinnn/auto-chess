using Sirenix.OdinInspector;
using UnityEngine;

public class BattleHero : Hero {
    public TeamSide Side => side;
    public BattleHero Target => target;
    
    [SerializeField, ReadOnly] TeamSide side;
    [SerializeField, ReadOnly] BattleHero target;
    HeroBT bt;

    public override void Activate() {
        gameObject.SetActive(true);

        if (!initialized) {
            FindAbilities();
            abilities.ForEach(x=>x.Initialize(this));
            initialized = true;
            GameManager.Instance.Progress.OnChangePhase += OnChangePhase;
        }
    }

    public void SetData(HeroTrait trait, HeroRank rank, TeamSide side) {
        this.trait = trait;
        this.side = side;
        this.rank = rank;
        name = $"({rank}){trait.id}";
        rankIcon.sprite = AssetDB.Instance.GetRankIcon(rank);
        SetUpModel();
        abilities.ForEach(x=>x.ResetAll());
        bt = new HeroBT(this);
    }
    
    public void FindTarget() {
        target = GameManager.Instance.BattleField.GetNearestOpponent(this);
    }

    protected override void Process() {
        base.Process();
        bt?.Evaluate();
    }

    void OnChangePhase(MatchPhase phase) {
        switch (phase) {
            case MatchPhase.Summary:
                bt = null;
                break;
        }
    }
}