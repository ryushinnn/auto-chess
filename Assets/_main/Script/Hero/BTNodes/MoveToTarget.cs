public class MoveToTarget : BTNode {
    Hero hero;
    MapNode targetMapNode;
    
    public MoveToTarget(Hero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        if (hero.Target == null) {
            hero.GetAbility<HeroMovement>().StopMove(true);
            targetMapNode = null;
            State = NodeState.Failure;
            return State;
        }

        
        if (hero.Target.MapNode != targetMapNode) {
            var destination = Map.Instance.GetNearestAdjacentNode(hero.MapNode, hero.Target.MapNode, 1);
            hero.GetAbility<HeroMovement>().StartMove(destination.Position);
            targetMapNode = hero.Target.MapNode;
        }
        
        State = hero.MapNode == targetMapNode ? NodeState.Success : NodeState.Running;
        return State;
    }
}