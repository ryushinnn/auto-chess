using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class Hero : MonoBehaviour, IMapNodeObject {
    [SerializeField] HeroTrait trait;
    [SerializeField] Transform model;
    [SerializeField] Transform abilitiesContainer;
    [SerializeField] Seeker seeker;

    public HeroTrait Trait => trait;
    public Seeker Seeker => seeker;
    public Transform Model => model;
    public Mecanim Mecanim => mecanim;
    public MapNode MapNode => mapNode;
    public Hero Target => target;

    Mecanim mecanim;
    List<HeroAbility> abilities = new();
    Dictionary<Type, HeroAbility> cachedAbilities = new();
    MapNode mapNode;
    [SerializeField, ReadOnly] Hero target;

    public Vector2 dev_mapNode;
    
    void Awake() {
        FindAbilities();
        mecanim = model.GetComponentInChildren<Mecanim>();
    }

    void Update() {
        PreProcess();
        Process();
        PostProcess();

        dev_mapNode = mapNode != null ? new Vector2(mapNode.X, mapNode.Y) : new Vector2(-1, -1);
    }

    public T GetAbility<T>() where T : HeroAbility {
        if (!cachedAbilities.ContainsKey(typeof(T))) {
            cachedAbilities.Add(typeof(T), abilities.Find(ab => ab is T));
        }
        return cachedAbilities[typeof(T)] as T;
    }

    public void SetNode(MapNode mapNode) {
        this.mapNode?.objects.Remove(this);
        this.mapNode = mapNode;
        this.mapNode?.objects.Add(this);
    }

    public void ResetPosition() {
        transform.position = mapNode.Position;
    }
    
    public void FindTarget() {
        target = Map.Instance.GetNearestNonEmptyNode<Hero>(mapNode)?.objects.First(x => x is Hero) as Hero;
    }

    void FindAbilities() {
        abilities ??= new List<HeroAbility>();
        foreach (Transform child in abilitiesContainer) {
            if (child.TryGetComponent(out HeroAbility ab)) {
                ab.Initialize(this);
                abilities.Add(ab);
            }
        }
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

    [Button]
    void Dev_ChangeNode(int x, int y) {
        SetNode(Map.Instance.GetNode(x,y));
        ResetPosition();
    }
}