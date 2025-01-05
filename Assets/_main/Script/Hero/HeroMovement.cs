using DG.Tweening;
using Pathfinding;
using UnityEngine;

public class HeroMovement : HeroAbility {
    Sequence moveSequence;
    [SerializeField] float moveSpeed;

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
                moveSequence.AppendCallback(() => {
                        hero.GetAbility<HeroRotation>().SetDirection(wp - hero.transform.position);
                    })
                    .Append(hero.transform.DOMove(wp, 1 / moveSpeed).SetEase(Ease.Linear));
            }

            moveSequence.AppendCallback(StopMove);
        });
    }

    public void StopMove() {
        hero.Mecanim.ChangeState(Mecanim.State.Idle);
        moveSequence?.Kill();
    }
}