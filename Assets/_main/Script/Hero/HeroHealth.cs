using System;
using UnityEngine;

public class HeroHealth : HeroAbility {
    [SerializeField] ProgressBar healthBar;
    [SerializeField] ProgressBar energyBar;

    void Start() {
        Initialize();   
    }

    void Initialize() {
        healthBar.UpdateAmount(1, true);
        energyBar.UpdateAmount(1, true);
    }
}