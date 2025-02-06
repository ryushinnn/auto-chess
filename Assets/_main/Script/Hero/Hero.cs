using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class Hero : MonoBehaviour, IMapNodeObject {
    [SerializeField] Transform model;
    [SerializeField] Transform abilitiesContainer;

    public HeroTrait Trait => trait;
    public TeamSide Side => side;
    public Seeker Seeker => seeker;
    public Transform Model => model;
    public Mecanim Mecanim => mecanim;
    public MapNode MapNode => mapNode;
    public Hero Target => target;

    Seeker seeker;
    HeroBT bt;
    [SerializeField, ReadOnly] HeroTrait trait;
    [SerializeField, ReadOnly] TeamSide side;
    Mecanim mecanim;
    List<HeroAbility> abilities = new();
    Dictionary<Type, HeroAbility> cachedAbilities = new();
    MapNode mapNode;
    [SerializeField, ReadOnly] Hero target;
    Tween snapTween;

    public Vector2 dev_mapNode;
    public Vector2 dev_targetNode;
    public Vector2 dev_destinationNode;
    
    void Awake() {
        FindAbilities();
        FindComponents();
    }

    void Update() {
        PreProcess();
        Process();
        PostProcess();

        dev_mapNode = mapNode != null ? new Vector2(mapNode.X, mapNode.Y) : new Vector2(-1, -1);
        dev_targetNode = target?.MapNode != null ? new Vector2(target.mapNode.X, target.mapNode.Y) : new Vector2(-1, -1);
    }

    void LateUpdate() {
        RemoveNodeOnDead();
    }

    public void Initialize(HeroTrait trait, TeamSide side) {
        this.trait = trait;
        this.side = side;
        foreach (Transform child in model) {
            if (child.TryGetComponent(out Mecanim _)) {
                Destroy(child.gameObject);
            }
        }
        mecanim = Instantiate(trait.mecanim, model);
        abilities.ForEach(x=>x.Initialize(this));
        bt.Initialize();
    }

    public T GetAbility<T>() where T : HeroAbility {
        if (!cachedAbilities.ContainsKey(typeof(T))) {
            cachedAbilities.Add(typeof(T), abilities.Find(ab => ab is T));
        }
        return cachedAbilities[typeof(T)] as T;
    }

    public void SetNode(MapNode mapNode) {
        this.mapNode?.Remove(this);
        this.mapNode = mapNode;
        this.mapNode.Add(this);
    }

    public void ResetPosition(bool skipAnimation = false) {
        snapTween?.Kill();
        if (skipAnimation) {
            transform.position = mapNode.Position;
        }
        else {
            snapTween = transform.DOMove(mapNode.Position, 0.1f);
        }
    }
    
    public void FindTarget() {
        Func<IMapNodeObject, bool> condition = x => x is Hero h && h.side != side;
        target = Map.Instance.GetNearestNode(mapNode, node => node.Any(condition))?.Get<Hero>(condition);
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
        if (!GetAbility<HeroAttributes>().IsAlive && mapNode != null) {
            mapNode.Remove(this);
            mapNode = null;
        }
    }

    [Button]
    void Dev_ChangeNode(int x, int y) {
        SetNode(Map.Instance.GetNode(x,y));
        ResetPosition(true);
    }
}