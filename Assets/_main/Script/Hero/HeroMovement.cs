using System;
using DG.Tweening;
using Pathfinding;
using RExt.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroMovement : HeroAbility {
    public bool IsMoving => isMoving;
    public bool DestinationReached => destinationReached;
    
    Sequence moveSequence;
    bool isMoving;
    bool destinationReached;
    MapNode destinationNode;
    DestinationMark mark;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        hero.Mecanim.Idle();
    }

    public override void Process() {
        if (isMoving) {
            hero.SetNode(Map.Instance.GetNode(hero.transform.position));
        }
        destinationReached = hero.MapNode == destinationNode;
        
        hero.dev_destinationNode = destinationNode != null ? new Vector2(destinationNode.X, destinationNode.Y) : new Vector2(-1, -1);
    }

    public void StartMove() {
        if (destinationNode != null 
            && destinationNode.Any(x=>x is Hero h && h == hero.Target) 
            && Map.Instance.CheckAdjacency(hero.Target.MapNode, destinationNode, hero.Trait.attackRange)) return;
        
        if (Map.Instance.CheckAdjacency(hero.Target.MapNode, hero.MapNode, hero.Trait.attackRange) && hero.MapNode.HasOnly(hero)) {
            destinationNode = hero.MapNode;
            OnReachEndOfPath();
            return;
        }
        
        if (isMoving) {
            destinationNode?.Remove(mark);
        }
        
        destinationNode = Map.Instance.GetNearestAdjacentNode(hero.MapNode, hero.Target.MapNode, hero.Trait.attackRange, 
                              x => x.HasNone()) 
                          ?? hero.MapNode;
        
        destinationNode.Add(mark = new DestinationMark());
        Debug.Log($"{hero.name} change destination to ({destinationNode.X}, {destinationNode.Y})");
        
        hero.Seeker.StartPath(hero.transform.position, destinationNode.Position, path => {
            isMoving = true;
            hero.Mecanim.Run();
            moveSequence?.Kill();
            moveSequence = DOTween.Sequence();

            for (int i = 1; i < path.vectorPath.Count; i++) {
                var wp = path.vectorPath[i];
                moveSequence.AppendCallback(() => {
                        hero.GetAbility<HeroRotation>().Rotate(wp - hero.transform.position);
                    })
                    .Append(hero.transform.DOMove(wp, 1 / hero.GetAbility<HeroAttributes>().MovementSpeed).SetEase(Ease.Linear));
            }

            moveSequence.AppendCallback(OnReachEndOfPath);
        });
    }

    public void StopMove(bool resetPosition = false) {
        isMoving = false;
        hero.Mecanim.Idle();
        moveSequence?.Kill();
        if (resetPosition) {
            var emptyNode = Map.Instance.GetNearestNode(hero.MapNode, x => x.HasOnly(hero) || x.HasNone());
            hero.SetNode(emptyNode);
            hero.ResetPosition();
        }
    }

    void OnReachEndOfPath() {
        Debug.Log($"{hero.name} reached ({destinationNode.X}, {destinationNode.Y})");
        hero.GetAbility<HeroRotation>().Rotate(hero.Target.MapNode.Position - hero.transform.position);
        StopMove();
        destinationNode.Remove(mark);
    }

    [Button]
    void Dev_StopMove() {
        StopMove(true);
    }

    void OnDrawGizmos() {
        if (destinationNode != null) {
            Gizmos.color = Color.red;
            Utils.DrawArrow(hero.MapNode.Position, destinationNode.Position);
            Gizmos.color = Color.green;
            Utils.DrawArrow(destinationNode.Position, hero.Target.MapNode.Position);
        }
    }
}