using UnityEngine;

public class MoveToTarget : BTNode {
    Hero hero;
    
    public MoveToTarget(Hero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        var movement = hero.GetAbility<HeroMovement>();
        if (hero.Target == null) {
            movement.StopMove(true);
            State = NodeState.Failure;
            return State;
        }
        
        hero.GetAbility<HeroMovement>().StartMove();
        State = !movement.IsMoving ? NodeState.Success : NodeState.Running;
        return State;
    }
}