using System;
using DG.Tweening;
using RExt.Utils;
using UnityEngine;

public class HeroPicker : MonoBehaviour {
    [SerializeField] LayerMask mapLayerMask;
    
    Hero hero;
    Tween tween;
    Node node;
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
            var deckNode = Deck.Instance.GetNode(pointXZ);
            if (deckNode != null) {
                if (deckNode.HasNone() || deckNode.Has(hero)) {
                    MapVisual.Instance.Highlight(true, deckNode);
                    MapVisual.Instance.MarkAsNotAvailable(false);
                }
                else {
                    MapVisual.Instance.Highlight(false);
                    MapVisual.Instance.MarkAsNotAvailable(true, deckNode);
                }
                node = deckNode;
                return;
            }
            
            var mapNode = Map.Instance.GetNode(pointXZ, (x, _) => x < Map.SIZE / 2);
            if (mapNode != null) {
                if (mapNode.HasNone() || mapNode.HasOnly(hero)) {
                    MapVisual.Instance.Highlight(true, mapNode);
                    MapVisual.Instance.MarkAsNotAvailable(false);
                }
                else {
                    MapVisual.Instance.Highlight(false);
                    MapVisual.Instance.MarkAsNotAvailable(true, mapNode);
                }
                node = mapNode;
                return;
            }

            MapVisual.Instance.Highlight(false);
            MapVisual.Instance.MarkAsNotAvailable(false);
            node = null;
        }
    }

    void OnMouseUp() {
        if (hero.Side == TeamSide.Enemy) return;
        
        tween?.Kill();
        tween = hero.Model.DOLocalMoveY(0, 0.2f);
        if (node == null) {
            MapVisual.Instance.Highlight(false);
            MapVisual.Instance.MarkAsNotAvailable(false);
            return;
        }
        
        if ((node is DeckNode dNode && (dNode.HasNone() || dNode.Has(hero)))
            || (node is MapNode mNode && (mNode.HasNone() || mNode.HasOnly(hero)))) {
            
            hero.SetNode(node);
            hero.ResetPosition();
            node = null;
            MapVisual.Instance.Highlight(false);
        }
        else {
            var otherHero = node.Get<Hero>();
            if (otherHero == null) {
                hero.ResetPosition();
            }
            else {
                hero.SwapNode(otherHero);
            }
            node = null;
            MapVisual.Instance.MarkAsNotAvailable(false);
        }
    }
}