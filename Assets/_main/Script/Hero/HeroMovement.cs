using System;
using DG.Tweening;
using Pathfinding;
using RExt.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class HeroMovement : HeroAbility {
    public bool IsMoving => destination != null;
    
    Sequence moveSequence;
    MapNode destination;
    DestinationMark mark;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        hero.Mecanim.Idle();
    }

    public override void Process() {
        if (destination != null) {
            hero.SetNode(Map.Instance.GetNode(hero.transform.position));
        }
        
        hero.dev_destinationNode = destination != null ? new Vector2(destination.X, destination.Y) : new Vector2(-1, -1);
    }

    public void StartMove() {
        if (Map.Instance.CheckAdjacency(hero.Target.MapNode, hero.MapNode, hero.Trait.attackRange)
            && hero.MapNode.HasAtFirst(hero)) {

            if (destination != null) {
                Debug.Log($"{hero.name} already has target in range, stop at ({destination.X}, {destination.Y})");
                destination.Remove(mark);
                StopMove(true);
            }
            return;
        }

        Func<MapNode, bool> destCondition = x => x.HasNone() || x == destination;
        var newDest = Map.Instance.GetNearestAdjacentNode(hero.Target.MapNode, hero.MapNode, hero.Trait.attackRange, destCondition) 
                      ??
                      Map.Instance.GetNearestNode(hero.Target.MapNode, destCondition);

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
                        hero.GetAbility<HeroRotation>().Rotate(wp - hero.transform.position);
                    })
                    .Append(hero.transform.DOMove(wp, 1 / hero.GetAbility<HeroAttributes>().MovementSpeed).SetEase(Ease.Linear));
            }

            moveSequence.AppendCallback(() => {
                Debug.Log($"{hero.name} reached ({destination.X}, {destination.Y})");
                hero.GetAbility<HeroRotation>().Rotate(hero.Target.MapNode.Position - hero.transform.position);
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
            var emptyNode = Map.Instance.GetNearestNode(hero.MapNode, x => x.HasAtFirst(hero) || x.HasNone());
            hero.SetNode(emptyNode);
            hero.ResetPosition();
        }
    }

    [Button]
    void Dev_StopMove() {
        StopMove(true);
    }

    void OnDrawGizmos() {
        if (hero.MapNode != null && destination != null && hero.Target?.MapNode != null) {
            var offset = new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
            Gizmos.color = Color.red;
            Utils.DrawArrow(hero.MapNode.Position, destination.Position + offset);
            Gizmos.color = Color.green;
            Utils.DrawArrow(destination.Position + offset, hero.Target.MapNode.Position);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hero.MapNode.Position + new Vector3(0,5,0), 0.25f);
        }
    }
}