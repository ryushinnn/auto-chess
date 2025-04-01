using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Pathfinding;
using RExt.Extension;
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

public class Hero : MonoBehaviour, IMapNodeObject {
    [SerializeField] public bool debug;
    
    [SerializeField] Transform model;
    [SerializeField] Transform abilitiesContainer;
    [SerializeField] Image rankIcon;

    public string ID => id;
    public HeroTrait Trait => trait;
    public TeamSide Side => side;
    public HeroRank Rank => rank;
    public Seeker Seeker => seeker;
    public Transform Model => model;
    public Mecanim Mecanim => mecanim;
    public MapNode MNode => node as MapNode;
    public DeckNode DNode => node as DeckNode;
    public Hero Target => target;

    Seeker seeker;
    HeroBT bt;
    HeroPicker picker;

    [SerializeField, ReadOnly] string id;
    [SerializeField, ReadOnly] HeroTrait trait;
    [SerializeField, ReadOnly] TeamSide side;
    [SerializeField, ReadOnly] HeroRank rank;
    [SerializeField, ReadOnly] HeroState state;
    Mecanim mecanim;
    List<HeroAbility> abilities = new();
    Dictionary<Type, HeroAbility> cachedAbilities = new();
    Node node;
    [SerializeField, ReadOnly] Hero target;
    Tween snapTween;

    public Vector2 dev_mapNode;
    public Vector2 dev_targetNode;
    public Vector2 dev_destinationNode;

    void Update() {
        PreProcess();
        Process();
        PostProcess();
        
        dev_mapNode = MNode != null ? new Vector2(MNode.X, MNode.Y) : new Vector2(-1, -1);
        dev_targetNode = target?.MNode != null ? new Vector2(target.MNode.X, target.MNode.Y) : new Vector2(-1, -1);
    }

    void LateUpdate() {
        RemoveNodeOnDead();
    }

    public void Initialize(HeroTrait trait, TeamSide side) {
        id = Guid.NewGuid().ToString();
        this.trait = trait;
        this.side = side;
        rank = HeroRank.B;
        name = $"({rank}){trait.id} ID:{id}";
        rankIcon.sprite = AssetDB.Instance.GetRankIcon(rank);
        FindAbilities();
        FindComponents();
        SetUpModel();
        abilities.ForEach(x=>x.Initialize(this));
        bt.Initialize();
        Switch(HeroState.Preparation);
    }

    public void Upgrade() {
        rank = rank.Next();
        name = $"({rank}){trait.id} ID:{id}";
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

    public void SetNode(Node node) {
        this.node?.Remove(this);
        this.node = node;
        this.node.Add(this);
    }

    public void SwapNode(Hero hero) {
        var currentNode = node;
        var newNode = (Node)hero.DNode ?? hero.MNode;
        SetNode(newNode);
        ResetPosition();
        hero.SetNode(currentNode);
        hero.ResetPosition();
    }

    public void ResetPosition(bool skipAnimation = false) {
        snapTween?.Kill();
        if (skipAnimation) {
            transform.position = node.Position;
        }
        else {
            snapTween = transform.DOMove(node.Position, 0.1f);
        }
    }
    
    public void FindTarget() {
        Func<IMapNodeObject, bool> condition = x => x is Hero h && h.side != side;
        target = Map.Instance.GetNearestNode(MNode, node => node.Any(condition))?.Get<Hero>(condition);
    }

    void FindAbilities() {
        foreach (Transform child in abilitiesContainer) {
            if (child.TryGetComponent(out HeroAbility ab)) {
                abilities.Add(ab);
            }
        }
    }
    
    void FindComponents() {
        seeker = GetComponent<Seeker>();
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

    void RemoveNodeOnDead() {
        if (state == HeroState.InBattle
            && !GetAbility<HeroAttributes>().IsAlive && node != null) {
            
            node.Remove(this);
            node = null;
        }
    }

    public void dev_Debug(object o) {
        if (!debug) return;
        Debug.Log(o);
    }

    [Button]
    void Dev_ChangeNode(int x, int y) {
        SetNode(Map.Instance.GetNode(x,y));
        ResetPosition(true);
    }
    
    [Button]
    void Dev_ChangeState(HeroState state) {
        Switch(state);
    }

    [Button]
    void Dev_CheckNullMapNode() {
        Debug.Log("mapNode is null: " + (MNode == null));
    }
}