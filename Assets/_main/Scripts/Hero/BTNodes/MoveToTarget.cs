using UnityEngine;

public class MoveToTarget : BTNode {
    BattleHero hero;
    
    public MoveToTarget(BattleHero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        var movement = hero.GetAbility<HeroMovement>();
        if (!hero.Trait.moveable) return NodeState.Failure;
        
        if (hero.Target == null) {
            movement.StopMove();
            State = NodeState.Failure;
            return State;
        }
        
        movement.StartMove();
        State = movement.ReachedDestination ? NodeState.Success : NodeState.Running;
        return State;
    }
}