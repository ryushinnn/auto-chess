public class FindTarget : BTNode {
    Hero hero;
    
    public FindTarget(Hero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        hero.FindTarget();
        State = NodeState.Success;
        return State;
    }
}