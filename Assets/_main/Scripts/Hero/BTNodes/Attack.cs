using UnityEngine;

public class Attack : BTNode {
    Hero hero;
    
    public Attack(Hero hero) {
        this.hero = hero;
    }

    public override NodeState Evaluate() {
        State = hero.GetAbility<HeroAttack>().Attack() ? NodeState.Success : NodeState.Failure;
        return State;
    }
}