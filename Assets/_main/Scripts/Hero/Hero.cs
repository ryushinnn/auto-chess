using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Pathfinding;
using RExt.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public enum HeroState {
    Preparation,
    ReadyToFight,
    InBattle
}

public enum HeroRank {
    B,
    A,
    S
}

public class Hero : MonoBehaviour {
    [SerializeField] public bool debug;
    
    [SerializeField] Transform model;
    [SerializeField] Transform abilitiesContainer;
    [SerializeField] Image rankIcon;

    public HeroTrait Trait => trait;
    public TeamSide Side => side;
    public HeroRank Rank => rank;
    public Transform Model => model;
    public Mecanim Mecanim => mecanim;
    public Hero Target => target;

    HeroBT bt;
    HeroPicker picker;

    [SerializeField, ReadOnly] HeroTrait trait;
    [SerializeField, ReadOnly] TeamSide side;
    [SerializeField, ReadOnly] HeroRank rank;
    [SerializeField, ReadOnly] HeroState state;
    [SerializeField, ReadOnly] Hero target;
    
    Mecanim mecanim;
    List<HeroAbility> abilities = new();
    Dictionary<Type, HeroAbility> cachedAbilities = new();
    Tween snapTween;

    public Vector2 dev_mapNode;
    public Vector2 dev_targetNode;
    public Vector2 dev_destinationNode;

    void Update() {
        PreProcess();
        Process();
        PostProcess();
    }

    public void Initialize(HeroTrait trait, TeamSide side) {
        this.trait = trait;
        this.side = side;
        rank = HeroRank.B;
        name = $"({rank}){trait.id}";
        rankIcon.sprite = AssetDB.Instance.GetRankIcon(rank);
        FindAbilities();
        FindComponents();
        SetUpModel();
        abilities.ForEach(x=>x.Initialize(this));
        if (!name.Contains(HeroId._Dummy_)) {
            bt.Initialize();
        }
        Switch(HeroState.Preparation);
    }

    public void Upgrade() {
        rank = rank.Next();
        name = $"({rank}){trait.id}";
        rankIcon.sprite = AssetDB.Instance.GetRankIcon(rank);
        Debug.Log($"{trait.id} upgraded to {rank}");
    }

    public void Switch(HeroState state) {
        this.state = state;
        switch (this.state) {
            case HeroState.Preparation:
                bt.Switch(false);
                picker.enabled = true;
                break;
            
            case HeroState.ReadyToFight:
                abilities.ForEach(x => x.ResetAll());
                picker.enabled = false;
                break;
            
            case HeroState.InBattle:
                bt.Switch(true);
                break;
        }
    }

    public T GetAbility<T>() where T : HeroAbility {
        if (!cachedAbilities.ContainsKey(typeof(T))) {
            cachedAbilities.Add(typeof(T), abilities.Find(ab => ab is T));
        }
        return cachedAbilities[typeof(T)] as T;
    }

    public void UpdatePosition(Node node, bool skipAnim = false) {
        snapTween?.Kill();
        if (skipAnim) {
            transform.position = node.WorldPosition;
        }
        else {
            snapTween = transform.DOMove(node.WorldPosition, 0.1f).SetEase(Ease.Linear);
        }
    }
    
    public void FindTarget() {
        target = GameManager.Instance.BattleField.GetNearestOpponent(this);
    }

    void FindAbilities() {
        foreach (Transform child in abilitiesContainer) {
            if (child.TryGetComponent(out HeroAbility ab)) {
                abilities.Add(ab);
            }
        }
    }
    
    void FindComponents() {
        bt = GetComponent<HeroBT>();
        picker = GetComponent<HeroPicker>();
    }

    void SetUpModel() {
        foreach (Transform child in model) {
            if (child.TryGetComponent(out Mecanim _)) {
                Destroy(child.gameObject);
            }
        }
        mecanim = Instantiate(trait.mecanim, model);
    }

    void PreProcess() {
        foreach (var ab in abilities) {
            if (ab.IsActive) {
                ab.PreProcess();
            }
        }
    }
    
    void Process() {
        foreach (var ab in abilities) {
            if (ab.IsActive) {
                ab.Process();
            }
        }
    }
    
    void PostProcess() {
        foreach (var ab in abilities) {
            if (ab.IsActive) {
                ab.PostProcess();
            }
        }
    }

    public MapNode GetNearestNode() {
        return Map.Instance.GetNearestNode(transform.position);
    }

    public void dev_Debug(object o) {
        if (!debug) return;
        Debug.Log(o);
    }
    
    [Button]
    void Dev_ChangeState(HeroState state) {
        Switch(state);
    }
}