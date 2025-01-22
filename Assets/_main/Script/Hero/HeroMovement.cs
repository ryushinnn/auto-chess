using System;
using DG.Tweening;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroMovement : HeroAbility {
    public bool IsMoving => isMoving;
    public bool DestinationReached => destinationReached;
    
    Sequence moveSequence;
    bool isMoving;
    bool destinationReached;
    MapNode targetNode;
    MapNode destinationNode;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        hero.Mecanim.Idle();
    }

    public override void Process() {
        if (isMoving) {
            hero.SetNode(Map.Instance.GetNode(hero.transform.position));
        }
        destinationReached = hero.MapNode == destinationNode;
    }

    public void StartMove() {
        if (targetNode == hero.Target.MapNode) return;
        
        targetNode = hero.Target.MapNode;
        var newDestinationNode = Map.Instance.GetNearestAdjacentNode(hero.MapNode, targetNode, hero.Trait.attackRange);
        if (newDestinationNode == destinationNode) return;
        
        destinationNode = newDestinationNode;
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

            moveSequence.AppendCallback(()=> {
                hero.GetAbility<HeroRotation>().Rotate(targetNode.Position - hero.transform.position);
                StopMove();
                Debug.Log($"{hero.name} reached ({destinationNode.X}, {destinationNode.Y})");
            });
        });
    }

    public void StopMove(bool resetPosition = false) {
        isMoving = false;
        hero.Mecanim.Idle();
        moveSequence?.Kill();
        if (resetPosition) {
            hero.ResetPosition();
        }
    }

    [Button]
    void Dev_StopMove() {
        StopMove(true);
    }
}