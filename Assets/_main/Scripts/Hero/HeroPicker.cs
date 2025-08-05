using System;
using DG.Tweening;
using RExt.Extensions;
using RExt.Utils;
using UnityEngine;

public class HeroPicker : MonoBehaviour {
    LayerMask mapLayerMask;
    LineUpHero hero;
    Tween tween;
    Node currentNode;
    Node nextNode;
    Vector3 offset;

    Vector3 originPos;
    float mouseDownTime;
    bool dragged;

    const float DRAG_THRESHOLD = 0.1f;
    const float HOLD_DURATION_THRESHOLD = 0.5f;
    const float DRAG_POS_Y = 1;

    void Awake() {
        hero = GetComponent<LineUpHero>();
        mapLayerMask = 1 << MapVisual.Instance.Layer;
    }

    void OnMouseDown() {
        tween?.Kill();
        tween = hero.Model.DOLocalMoveY(DRAG_POS_Y, 0.2f);
        var ray = Utils.MainCamera().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, mapLayerMask) && hit.collider != null) {
            offset = new Vector3(hero.transform.position.x - hit.point.x, 0, hero.transform.position.z - hit.point.z);
        }
        currentNode = GameManager.Instance.LineUp.FindNodeOfHero(hero);
        
        dragged = false;
        originPos = hero.transform.position;
        mouseDownTime = Time.unscaledTime;
    }

    void OnMouseDrag() {
        var ray = Utils.MainCamera().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, mapLayerMask) && hit.collider != null) {
            var point = hit.point.ToZeroY();
            hero.transform.position = point + offset;
            var selectedDeckNode = Deck.Instance.GetNearestNode(point);
            if (selectedDeckNode != null) {
                if (selectedDeckNode.IsEmpty() || selectedDeckNode == currentNode) {
                    MapVisual.Instance.Highlight(true, selectedDeckNode);
                    MapVisual.Instance.MarkAsNotAvailable(false);
                }
                else {
                    MapVisual.Instance.Highlight(false);
                    MapVisual.Instance.MarkAsNotAvailable(true, selectedDeckNode);
                }
                nextNode = selectedDeckNode;
                return;
            }
            
            var selectedMapNode = Map.Instance.GetNearestNode(point, n => n.X < Map.SIZE / 2);
            if (selectedMapNode != null) {
                if (selectedMapNode.IsEmpty() || selectedMapNode == currentNode) {
                    MapVisual.Instance.Highlight(true, selectedMapNode);
                    MapVisual.Instance.MarkAsNotAvailable(false);
                }
                else {
                    MapVisual.Instance.Highlight(false);
                    MapVisual.Instance.MarkAsNotAvailable(true, selectedMapNode);
                }
                nextNode = selectedMapNode;
                return;
            }

            MapVisual.Instance.Highlight(false);
            MapVisual.Instance.MarkAsNotAvailable(false);
            nextNode = null;
        }
        
        if ((hero.transform.position - originPos).sqrMagnitude > DRAG_THRESHOLD * DRAG_THRESHOLD) {
            dragged = true;
        }
    }

    void OnMouseUp() {
        tween?.Kill();
        tween = hero.Model.DOLocalMoveY(0, 0.2f);
        if (nextNode == null) {
            MapVisual.Instance.Highlight(false);
            MapVisual.Instance.MarkAsNotAvailable(false);
            return;
        }
        
        if (nextNode is DeckNode) {
            // case 0: move to self
            if (nextNode == currentNode) {
                GameManager.Instance.LineUp.UpdateHeroNode(hero, currentNode);
                nextNode = null;
                MapVisual.Instance.Highlight(false);
            }
            // case 1: move map/deck -> deck (not self)
            else if (nextNode.IsEmpty()) {
                GameManager.Instance.LineUp.UpdateHeroNode(hero, nextNode);
                nextNode = null;
                MapVisual.Instance.Highlight(false);
                GameManager.Instance.LineUp.RecalculateHeroesOnMap();
            }
            // case 2: swap
            else {
                var other = GameManager.Instance.LineUp.FindHeroOnNode(nextNode);
                // invalid, back to current node
                if (other == null) {
                    Debug.Log($"Something is wrong. node {nextNode} is occupied but no hero found");
                    GameManager.Instance.LineUp.UpdateHeroNode(hero, currentNode);
                }
                // valid, swap
                else {
                    GameManager.Instance.LineUp.SwapHeroNodes(hero, other);
                }
                nextNode = null;
                MapVisual.Instance.MarkAsNotAvailable(false);
            }
        }
        else if (nextNode is MapNode) {
            // case 0: move to self
            if (nextNode == currentNode) {
                GameManager.Instance.LineUp.UpdateHeroNode(hero, currentNode);
                nextNode = null;
                MapVisual.Instance.Highlight(false);
            }
            // case 1: move map -> map (not self)
            else if (currentNode is MapNode && nextNode.IsEmpty()) {
                GameManager.Instance.LineUp.UpdateHeroNode(hero, nextNode);
                nextNode = null;
                MapVisual.Instance.Highlight(false);
            }
            // case 2: move deck -> map
            else if (currentNode is DeckNode && nextNode.IsEmpty()) {
                // invalid, back to current node
                if (GameManager.Instance.LineUp.Full) {
                    GameManager.Instance.LineUp.UpdateHeroNode(hero, currentNode);
                    nextNode = null;
                    MapVisual.Instance.Highlight(false);
                }
                // valid, update node
                else {
                    GameManager.Instance.LineUp.UpdateHeroNode(hero, nextNode);
                    nextNode = null;
                    MapVisual.Instance.Highlight(false);
                    GameManager.Instance.LineUp.RecalculateHeroesOnMap();
                }
            }
            // case 3: swap
            else {
                var other = GameManager.Instance.LineUp.FindHeroOnNode(nextNode);
                // invalid, back to current node
                if (other == null) {
                    Debug.Log($"Something is wrong. node {nextNode} is occupied but no hero found");
                    GameManager.Instance.LineUp.UpdateHeroNode(hero, currentNode);
                }
                // valid, swap
                else {
                    GameManager.Instance.LineUp.SwapHeroNodes(hero, other);
                }
                nextNode = null;
                MapVisual.Instance.MarkAsNotAvailable(false);
            }
        }

        if (!dragged && Time.unscaledTime - mouseDownTime < HOLD_DURATION_THRESHOLD) {
            ArenaUIManager.Instance.HeroInfo.Open(hero);
        }
    }
}