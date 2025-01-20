public class IsAlive : BTNode {
    Hero hero;
    
    public IsAlive(Hero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        State = hero.GetAbility<HeroHealth>().IsAlive ? NodeState.Success : NodeState.Failure;
        return State;
    }
}