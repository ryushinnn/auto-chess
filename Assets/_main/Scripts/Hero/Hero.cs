using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Pathfinding;
using RExt.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public enum HeroRank {
    B,
    A,
    S
}

public class Hero : MonoBehaviour {
    public Vector3 WorldPosition {
        get => transform.position;
        set => transform.position = value.ToZeroY();
    }
    
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
    [SerializeField, ReadOnly] Hero target;
    
    bool initialized;
    
    Mecanim mecanim;
    List<HeroAbility> abilities = new();
    Dictionary<Type, HeroAbility> cachedAbilities = new();
    Tween snapTween;

    void Update() {
        Process();
    }

    public void Activate() {
        gameObject.SetActive(true);

        if (!initialized) {
            FindAbilities();
            FindComponents();
            abilities.ForEach(x=>x.Initialize(this));
            initialized = true;
        }
    }
    
    public void Deactivate() {
        gameObject.SetActive(false);
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
        if (name.Contains(HeroId.D_u_m_m_y)) {
            GetAbility<HeroMovement>().Disable();
        }
    }

    public void Upgrade() {
        rank = rank.Next();
        name = $"({rank}){trait.id}";
        rankIcon.sprite = AssetDB.Instance.GetRankIcon(rank);
        Debug.Log($"{trait.id} upgraded to {rank}");
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
            WorldPosition = node.WorldPosition;
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
    
    void Process() {
        foreach (var ab in abilities) {
            if (ab.IsActive) {
                ab.Process();
            }
        }
        
        bt?.Evaluate();
    }
}