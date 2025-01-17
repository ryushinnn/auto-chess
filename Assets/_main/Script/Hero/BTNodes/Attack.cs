using UnityEngine;

public class Attack : BTNode {
    Hero hero;

    float cooldown = 1;
    float lastSuccessTime = Mathf.NegativeInfinity;
    
    public Attack(Hero hero) {
        this.hero = hero;
    }

    public override NodeState Evaluate() {
        if (Time.time >= lastSuccessTime + cooldown) {
            lastSuccessTime = Time.time;
            Debug.Log("attack");
            State = NodeState.Success;
            return State;
        }
        
        State = NodeState.Failure;
        return State;
    }
}