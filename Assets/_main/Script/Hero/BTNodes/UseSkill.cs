using UnityEngine;

public class UseSkill : BTNode {
    Hero hero;
    
    public UseSkill(Hero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        State = hero.GetAbility<HeroSkill>().UseSkill() ? NodeState.Success : NodeState.Failure;
        return State;
    }
}