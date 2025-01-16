using System;
using System.Collections.Generic;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class Hero : MonoBehaviour, INodeObject {
    [SerializeField] HeroTrait trait;
    [SerializeField] Transform model;
    [SerializeField] Transform abilitiesContainer;
    [SerializeField] Seeker seeker;

    public HeroTrait Trait => trait;
    public Seeker Seeker => seeker;
    public Transform Model => model;
    public Mecanim Mecanim => mecanim;

    Mecanim mecanim;
    List<HeroAbility> abilities = new();
    Dictionary<Type, HeroAbility> cachedAbilities = new();
    [SerializeField, ReadOnly] Node node;
    
    void Awake() {
        FindAbilities();
        mecanim = model.GetComponentInChildren<Mecanim>();
    }

    void Update() {
        PreProcess();
        Process();
        PostProcess();
    }

    public T GetAbility<T>() where T : HeroAbility {
        if (!cachedAbilities.ContainsKey(typeof(T))) {
            cachedAbilities.Add(typeof(T), abilities.Find(ab => ab is T));
        }
        return cachedAbilities[typeof(T)] as T;
    }

    public void SetNode(Node node) {
        if (this.node != null) {
            this.node.obj = null;
        }
        this.node = node;
        this.node.obj = this;
    }

    public void ResetPosition() {
        transform.position = node.Position;
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
}