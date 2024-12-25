using System;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    [SerializeField] Transform abilitiesContainer;

    List<HeroAbility> abilities = new();

    void Awake() {
        FindAbilities();
    }

    void Update() {
        PreProcess();
        Process();
        PostProcess();
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