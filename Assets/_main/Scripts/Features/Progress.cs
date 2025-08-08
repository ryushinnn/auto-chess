using System;
using System.Collections.Generic;
using DG.Tweening;
using RExt.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

public enum MatchPhase {
    None,
    Preparation,
    Transition,
    Battle,
    Summary,
}

public enum MatchResult {
    Win,
    Lose,
}

public class Progress : MonoBehaviour {
    public MatchPhase Phase => phase;
    public event Action<MatchResult> OnEndMatch;
    
    [SerializeField] Stage[] stages;

    [SerializeField, ReadOnly] MatchPhase phase;
    float timeLeft;
    float totalTime;
    int currentStage;
    int currentMatch;

    Dictionary<MatchPhase, MatchPhase> nexts = new() {
        { MatchPhase.None, MatchPhase.Preparation},
        { MatchPhase.Preparation, MatchPhase.Transition },
        { MatchPhase.Transition, MatchPhase.Battle },
        { MatchPhase.Battle, MatchPhase.Summary },
        { MatchPhase.Summary, MatchPhase.Preparation }
    };

    void Awake() {
        OnEndMatch = r => {
            Debug.Log(r);
        };
    }

    void Update() {
        CountDown();
        
        //test
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (phase == MatchPhase.Preparation || phase == MatchPhase.Battle) {
                timeLeft = 1;
            }
        }
    }
    
    public void Initialize() {
        ChangePhase();
    }

    void CountDown() {
        if (phase == MatchPhase.None) return;
        
        if (timeLeft > 0) {
            timeLeft -= Time.deltaTime;
            ArenaUIManager.Instance.Arena.UpdateTimeLeft(timeLeft, totalTime);
        }
        else {
            ChangePhase();
        }
    }

    void ChangePhase() {
        var nextPhase = nexts[phase];
        switch (nextPhase) {
            case MatchPhase.Preparation:
                if (phase != MatchPhase.None) {
                    currentMatch++;
                    if (currentMatch >= stages[currentStage].matches.Length) {
                        currentMatch = 0;
                        currentStage++;
                        if (currentStage >= stages.Length) {
                            phase = MatchPhase.None;
                            break;
                        }
                    }
                    GameManager.Instance.Level.GainXp(GameConfigs.XP_GAIN_PER_MATCH);
                }

                ArenaUIManager.Instance.Arena.UpdateRoundAndWave(currentStage, currentMatch);
                phase = nextPhase;
                timeLeft = totalTime = GameConfigs.MATCH_PHASE_DURATIONS[phase];
                GameManager.Instance.BattleField.RemoveHeroes();
                GameManager.Instance.LineUp.ShowHeroesOnMap();
                GameManager.Instance.LineUp.SetHeroesPickable(true);
                MapVisual.Instance.SetHalfMapVisibility(false);
                break;
            
            case MatchPhase.Transition:
                phase = nextPhase;
                timeLeft = totalTime = GameConfigs.MATCH_PHASE_DURATIONS[phase];
                GameManager.Instance.LineUp.SetHeroesPickable(false);
                GameManager.Instance.LineUp.FillHeroesOnMap();
                GameManager.Instance.LineUp.PlayHeroesDiveInAnimation();
                DOVirtual.DelayedCall(GameConfigs.SPAWN_ENEMIES_DELAY, () => {
                    GameManager.Instance.LineUp.HideHeroesOnMap();
                    GameManager.Instance.BattleField.SpawnHeroes();
                    GameManager.Instance.BattleField.PlayHeroesDiveOutAnimation();
                    MapVisual.Instance.SetHalfMapVisibility(true);
                });
                break;
            
            case MatchPhase.Battle:
                phase = nextPhase;
                timeLeft = totalTime = GameConfigs.MATCH_PHASE_DURATIONS[phase];
                GameManager.Instance.BattleField.ActivateHeroes();
                break;
            
            case MatchPhase.Summary:
                OnEndMatch?.Invoke(MatchResult.Lose);
                phase = nextPhase;
                timeLeft = totalTime = GameConfigs.MATCH_PHASE_DURATIONS[phase];
                GameManager.Instance.BattleField.DeactivateHeroes();
                break;
        }
    }

    public Enemy[] GetEnemies() {
        return stages[currentStage].matches[currentMatch].enemies;
    }

    public void EndBattlePhase(MatchResult result) {
        if (phase != MatchPhase.Battle) return;
        OnEndMatch?.Invoke(result);
        phase = MatchPhase.Summary;
        timeLeft = totalTime = GameConfigs.MATCH_PHASE_DURATIONS[phase];
        GameManager.Instance.BattleField.DeactivateHeroes();
    }
}