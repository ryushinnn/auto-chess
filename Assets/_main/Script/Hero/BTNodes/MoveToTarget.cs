using UnityEngine;

public class MoveToTarget : BTNode {
    Hero hero;
    MapNode targetNode;
    MapNode destinationNode;
    
    public MoveToTarget(Hero hero) {
        this.hero = hero;
    }
    
    public override NodeState Evaluate() {
        if (hero.Target == null) {
            hero.GetAbility<HeroMovement>().StopMove(true);
            targetNode = null;
            State = NodeState.Failure;
            return State;
        }

        
        if (hero.Target.MapNode != targetNode) {
            targetNode = hero.Target.MapNode;
            destinationNode = Map.Instance.GetNearestAdjacentNode(hero.MapNode, targetNode, hero.GetAbility<HeroAttack>().AttackRange);
            hero.GetAbility<HeroMovement>().StartMove(destinationNode.Position);
            // Debug.Log(hero.name + " move to " + destinationNode.X + ", " + destinationNode.Y);
        }
        
        State = hero.MapNode == destinationNode ? NodeState.Success : NodeState.Running;
        return State;
    }
}