using DG.Tweening;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroMovement : HeroAbility {
    public bool IsMoving => isMoving;
    
    float moveSpeed;
    Sequence moveSequence;
    bool isMoving;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        moveSpeed = this.hero.Trait.movementSpeed;
    }

    public override void Process() {
        if (isMoving) {
            hero.SetNode(Map.Instance.GetNode(hero.transform.position));
        }
    }

    public void StartMove(Vector3 dest) {
        hero.Seeker.StartPath(hero.transform.position, dest, path => {
            isMoving = true;
            hero.Mecanim.ChangeState(Mecanim.State.Run);
            moveSequence?.Kill();
            moveSequence = DOTween.Sequence();

            for (int i = 1; i < path.vectorPath.Count; i++) {
                var wp = path.vectorPath[i];
                moveSequence.AppendCallback(() => {
                        hero.GetAbility<HeroRotation>().Rotate(wp - hero.transform.position);
                    })
                    .Append(hero.transform.DOMove(wp, 1 / moveSpeed).SetEase(Ease.Linear));
            }

            moveSequence.AppendCallback(()=>StopMove());
        });
    }

    public void StopMove(bool resetPosition = false) {
        isMoving = false;
        hero.Mecanim.ChangeState(Mecanim.State.Idle);
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