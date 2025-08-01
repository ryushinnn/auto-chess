﻿using UnityEngine;

public class MoveToTarget : BTNode {
    Hero hero;
    
    public MoveToTarget(Hero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        var movement = hero.GetAbility<HeroMovement>();
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