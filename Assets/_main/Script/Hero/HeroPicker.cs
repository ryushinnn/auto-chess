using System;
using DG.Tweening;
using RExt.Utils;
using UnityEngine;

public class HeroPicker : MonoBehaviour {
    [SerializeField] LayerMask mapLayerMask;
    
    Hero hero;
    Tween tween;
    MapNode node;
    Vector3 offset;

    const float DRAG_POS_Y = 1;

    void Awake() {
        hero = GetComponent<Hero>();
    }

    void OnMouseDown() {
        if (hero.Side == TeamSide.Enemy) return;
        
        tween?.Kill();
        tween = hero.Model.DOLocalMoveY(DRAG_POS_Y, 0.2f);
        var ray = Utils.MainCamera().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, mapLayerMask) && hit.collider != null) {
            offset = new Vector3(hero.transform.position.x - hit.point.x, 0, hero.transform.position.z - hit.point.z);
        }
    }

    void OnMouseDrag() {
        if (hero.Side == TeamSide.Enemy) return;
        
        var ray = Utils.MainCamera().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, mapLayerMask) && hit.collider != null) {
            var pointXZ = new Vector3(hit.point.x, 0, hit.point.z);
            hero.transform.position = pointXZ + offset;
            node = Map.Instance.GetNode(pointXZ, (x, _) => x < Map.SIZE / 2);
            if (node.HasNone() || node.HasOnly(hero)) {
                MapVisual.Instance.Highlight(true, node);
                MapVisual.Instance.MarkAsNotAvailable(false);
            }
            else {
                MapVisual.Instance.Highlight(false);
                MapVisual.Instance.MarkAsNotAvailable(true, node);
            }
        }
    }

    void OnMouseUp() {
        if (hero.Side == TeamSide.Enemy) return;
        
        tween?.Kill();
        tween = hero.Model.DOLocalMoveY(0, 0.2f);
        if (node.HasNone() || node.HasOnly(hero)) {
            hero.SetNode(node);
            hero.ResetPosition();
            node = null;
            MapVisual.Instance.Highlight(false);
        }
        else {
            hero.ResetPosition();
            node = null;
            MapVisual.Instance.MarkAsNotAvailable(false);
        }
    }
}