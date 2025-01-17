using UnityEngine;

public class UseSkill : BTNode {
    Hero hero;

    float cooldown = 5;
    float lastSuccessTime = Mathf.NegativeInfinity;
    
    public UseSkill(Hero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        if (Time.time >= lastSuccessTime + cooldown) {
            lastSuccessTime = Time.time;
            Debug.Log("use skill");
            State = NodeState.Success;
            return State;
        }
        
        State = NodeState.Failure;
        return State;
    }
}