using System;
using System.Collections.Generic;
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

public class Stages : MonoBehaviour {
    public event Action<MatchPhase> OnChangePhase;
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
        OnChangePhase += p => {
            Debug.Log(p);
        };
    }

    void Update() {
        CountDown();
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
                }

                ArenaUIManager.Instance.Arena.UpdateRoundAndWave(currentStage, currentMatch);
                phase = nextPhase;
                timeLeft = totalTime = GameConfigs.MATCH_PHASE_DURATIONS[phase];
                break;
            
            case MatchPhase.Transition:
                phase = nextPhase;
                timeLeft = totalTime = GameConfigs.MATCH_PHASE_DURATIONS[phase];
                break;
            
            case MatchPhase.Battle:
                phase = nextPhase;
                timeLeft = totalTime = GameConfigs.MATCH_PHASE_DURATIONS[phase];
                break;
            
            case MatchPhase.Summary:
                OnEndMatch?.Invoke(MatchResult.Lose);
                phase = nextPhase;
                timeLeft = totalTime = GameConfigs.MATCH_PHASE_DURATIONS[phase];
                break;
        }
        
        OnChangePhase?.Invoke(phase);
    }

    public Enemy[] GetEnemies() {
        return stages[currentStage].matches[currentMatch].enemies;
    }

    public void EndBattlePhase(MatchResult result) {
        if (phase != MatchPhase.Battle) return;
        OnEndMatch?.Invoke(result);
        phase = MatchPhase.Summary;
        timeLeft = totalTime = GameConfigs.MATCH_PHASE_DURATIONS[phase];
        OnChangePhase?.Invoke(phase);
    }
}