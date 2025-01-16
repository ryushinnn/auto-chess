using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class HeroRotation : HeroAbility {
    float rotateTime = 0.25f;
    Tween rotateTween;

    public void Rotate(Vector3 direction, bool instantly = false) {
        rotateTween?.Kill();
        var rot = Quaternion.LookRotation(direction);
        if (instantly) {
            hero.Model.rotation = rot;
            return;
        }

        rotateTween = hero.Model.DORotateQuaternion(rot, rotateTime);
    }
}