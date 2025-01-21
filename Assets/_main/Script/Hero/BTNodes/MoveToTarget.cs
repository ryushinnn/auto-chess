using UnityEngine;

public class MoveToTarget : BTNode {
    Hero hero;
    
    public MoveToTarget(Hero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        if (hero.Target == null) {
            hero.GetAbility<HeroMovement>().StopMove(true);
            State = NodeState.Failure;
            return State;
        }
        
        hero.GetAbility<HeroMovement>().StartMove();
        State = hero.GetAbility<HeroMovement>().DestinationReached ? NodeState.Success : NodeState.Running;
        return State;
    }
}