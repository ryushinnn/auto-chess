using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Hero : MonoBehaviour {
    [SerializeField] HeroTrait trait;
    [SerializeField] Transform model;
    [SerializeField] Transform abilitiesContainer;

    public HeroTrait Trait => trait;
    public Seeker Seeker => seeker;
    public Transform Model => model;
    public Mecanim Mecanim => mecanim;

    Seeker seeker;
    Mecanim mecanim;
    List<HeroAbility> abilities = new();
    Dictionary<Type, HeroAbility> cachedAbilities = new();

    void Awake() {
        FindAbilities();
        seeker = GetComponent<Seeker>();
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