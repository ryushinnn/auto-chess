public class MoveToTarget : BTNode {
    Hero hero;
    MapNode target;
    MapNode destination;
    
    public MoveToTarget(Hero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        if (hero.Target == null) {
            hero.GetAbility<HeroMovement>().StopMove(true);
            target = null;
            State = NodeState.Failure;
            return State;
        }

        
        if (hero.Target.MapNode != target) {
            target = hero.Target.MapNode;
            destination = Map.Instance.GetNearestAdjacentNode(hero.MapNode, target, hero.GetAbility<HeroAttack>().AttackRange);
            hero.GetAbility<HeroMovement>().StartMove(destination.Position);
        }
        
        State = hero.MapNode == destination ? NodeState.Success : NodeState.Running;
        return State;
    }
}