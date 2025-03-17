using System;
using DG.Tweening;
using Pathfinding;
using RExt.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class HeroMovement : HeroAbility {
    public bool IsMoving => destination != null;

    HeroRotation rotation;
    HeroAttributes attributes;
    
    Sequence moveSequence;
    MapNode destination;
    DestinationMark mark;

    public override void ResetAll() {
        moveSequence?.Kill();
        destination = null;
        mark = null;
        hero.Mecanim.Idle();
    }

    public override void Process() {
        if (destination != null) {
            hero.SetNode(Map.Instance.GetNode(hero.transform.position));
        }
        
        hero.dev_destinationNode = destination != null ? new Vector2(destination.X, destination.Y) : new Vector2(-1, -1);
    }

    protected override void FindReferences() {
        rotation = hero.GetAbility<HeroRotation>();
        attributes = hero.GetAbility<HeroAttributes>();
    }

    public void StartMove() {
        if (Map.Instance.CheckAdjacency(hero.Target.MNode, hero.MNode, hero.Trait.attackRange)
            && hero.MNode.HasAtFirst(hero)) {

            if (destination != null) {
                Debug.Log($"{hero.name} already has target in range, stop at ({destination.X}, {destination.Y})");
                destination.Remove(mark);
                StopMove(true);
            }
            return;
        }

        Func<MapNode, bool> destCondition = x => x.HasNone() || x == destination;
        var newDest = Map.Instance.GetNearestAdjacentNode(hero.Target.MNode, hero.MNode, hero.Trait.attackRange, destCondition) 
                      ??
                      Map.Instance.GetNearestNode(hero.Target.MNode, destCondition);

        // known issue: new dest and current dest has same distance to target
        // sometimes hero move between 2 nodes repeatedly
        
        if (newDest == destination) return;

        destination?.Remove(mark);
        destination = newDest;
        destination.Add(mark = new DestinationMark(hero));

        Debug.Log($"{hero.name} change destination to ({destination.X}, {destination.Y})");
        hero.Mecanim.Run();
        
        hero.Seeker.StartPath(hero.transform.position, destination.Position, path => {
            moveSequence?.Kill();
            moveSequence = DOTween.Sequence();

            for (int i = 1; i < path.vectorPath.Count; i++) {
                var wp = path.vectorPath[i];
                moveSequence.AppendCallback(() => {
                        rotation.Rotate(wp - hero.transform.position);
                    })
                    .Append(hero.transform.DOMove(wp, 1 / attributes.MovementSpeed).SetEase(Ease.Linear));
            }

            moveSequence.AppendCallback(() => {
                Debug.Log($"{hero.name} reached ({destination.X}, {destination.Y})");
                rotation.Rotate(hero.Target.MNode.Position - hero.transform.position);
                destination.Remove(mark);
                StopMove();
            });
        });
    }

    public void StopMove(bool resetPosition = false) {
        destination = null;
        hero.Mecanim.Idle();
        moveSequence?.Kill();
        if (resetPosition) {
            // known issue: null ref exception here??? maybe because of current node is null
            var emptyNode = Map.Instance.GetNearestNode(hero.MNode, x => x.HasAtFirst(hero) || x.HasNone());
            hero.SetNode(emptyNode);
            hero.ResetPosition();
        }
    }

    [Button]
    void Dev_StopMove() {
        StopMove(true);
    }

    void OnDrawGizmos() {
        if (hero != null 
            && hero.MNode != null 
            && destination != null 
            && hero.Target?.MNode != null) {
            
            var offset = new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
            Gizmos.color = Color.red;
            Utils.DrawArrow(hero.MNode.Position, destination.Position + offset);
            Gizmos.color = Color.green;
            Utils.DrawArrow(destination.Position + offset, hero.Target.MNode.Position);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hero.MNode.Position + new Vector3(0,5,0), 0.25f);
        }
    }
}