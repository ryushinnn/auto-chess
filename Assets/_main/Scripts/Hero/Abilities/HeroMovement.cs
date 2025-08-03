using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HeroMovement : HeroAbility {
    public bool ReachedDestination => reachedDestination;

    HeroRotation rotation;
    HeroAttributes attributes;
    
    Sequence moveSequence;
    MapNode currentDestination;
    MapNode currentTargetNode;
    List<MapNode> path;
    MapNode currentNode;
    MapNode nextNode;
    bool reachedDestination;
    
    public override void ResetAll() {
        moveSequence?.Kill();
        currentDestination = null;
        hero.Mecanim.Idle();
    }

    protected override void FindReferences() {
        rotation = hero.GetAbility<HeroRotation>();
        attributes = hero.GetAbility<HeroAttributes>();
    }
    
    public void StartMove() {
        var targetNode = hero.Target.GetNearestNode();
        
        // if target not move, no need to calculate new destination
        if (targetNode == currentTargetNode) return;
        
        currentTargetNode = targetNode;
        // if target in range, no need to calculate new destination
        if (nextNode != null && Map.Instance.CheckAdjacency(nextNode, currentTargetNode, hero.Trait.attackRange)) return;

        var myNode = hero.GetNearestNode();
        var destination = Map.Instance.GetNeighbors(currentTargetNode, hero.Trait.attackRange).Filter(x => x.IsEmpty()).GetNearestFrom(myNode)
                      ?? Map.Instance.GetNearestNode(currentTargetNode, x => x.IsEmpty());
        
        // if destination not changed, no need to calculate new path
        if (destination == currentDestination) return;

        // update destination, calculate new path
        currentDestination?.SetToEmpty();
        currentDestination = destination;
        currentDestination.ChangeState(NodeState.Targeted);
        
        hero.Mecanim.Run();

        reachedDestination = false;
        path = Map.Instance.FindPath(myNode, currentDestination);
        moveSequence?.Kill();
        moveSequence = DOTween.Sequence();
        for (int i = 1; i < path.Count; i++) {
            var node = path[i];
            moveSequence.AppendCallback(() => {
                nextNode = node;
                rotation.Rotate(nextNode.WorldPosition - hero.transform.position);
            })
            .Append(hero.transform.DOMove(node.WorldPosition, 1/attributes.MovementSpeed).SetEase(Ease.Linear))
            .AppendCallback(() => {
                currentNode = node;
                // check if target in range before reach destination, can early cancel move
                if (currentNode != currentDestination && Map.Instance.CheckAdjacency(hero.Target.GetNearestNode(), currentNode, hero.Trait.attackRange)) {
                    rotation.Rotate(hero.Target.transform.position - hero.transform.position);
                    currentNode.ChangeState(NodeState.Occupied);
                    // only reset it if it is targeted (by self)
                    if (currentDestination.State == NodeState.Targeted) {
                        currentDestination.SetToEmpty();
                    }
                    StopMove();
                    reachedDestination = true;
                }
            });
        }

        moveSequence.AppendCallback(() => {
            rotation.Rotate(hero.Target.transform.position - hero.transform.position);
            currentDestination.ChangeState(NodeState.Occupied);
            StopMove();
            reachedDestination = true;
        });
    }

    public void StopMove() {
        hero.Mecanim.Idle();
        moveSequence?.Kill();
    }

    void OnDrawGizmos() {
        if (path != null) {
            for (int i = 1; i < path.Count; i++) {
                Gizmos.DrawLine(path[i].WorldPosition, path[i - 1].WorldPosition);
            }
        }
    }
}