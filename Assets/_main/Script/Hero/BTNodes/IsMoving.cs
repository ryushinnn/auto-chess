public class IsMoving : BTNode {
    Hero hero;
    
    public IsMoving(Hero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        State = hero.GetAbility<HeroMovement>().IsMoving ? NodeState.Success : NodeState.Failure;
        return State;
    }
}