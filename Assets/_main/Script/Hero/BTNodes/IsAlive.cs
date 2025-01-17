public class IsAlive : BTNode {
    Hero hero;
    
    public IsAlive(Hero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        State = hero.IsAlive() ? NodeState.Success : NodeState.Failure;
        return State;
    }
}