using DG.Tweening;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroMovement : HeroAbility {
    float moveSpeed;
    Sequence moveSequence;

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        moveSpeed = this.hero.Trait.movementSpeed;
    }

    public void StartMove(Vector3 dest) {
        hero.Seeker.StartPath(hero.transform.position, dest, path => {
            hero.Mecanim.ChangeState(Mecanim.State.Run);
            moveSequence?.Kill();
            moveSequence = DOTween.Sequence();

            for (int i = 1; i < path.vectorPath.Count; i++) {
                var wp = path.vectorPath[i];
                var rotateInstantly = i == 1;
                moveSequence.AppendCallback(() => {
                        hero.GetAbility<HeroRotation>().Rotate(wp - hero.transform.position, rotateInstantly);
                    })
                    .Append(hero.transform.DOMove(wp, 1 / moveSpeed).SetEase(Ease.Linear))
                    .AppendCallback(() => {
                        hero.SetNode(Map.Instance.GetNode(hero.transform.position));
                    });
            }

            moveSequence.AppendCallback(()=>StopMove());
        });
    }

    public void StopMove(bool resetPosition = false) {
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