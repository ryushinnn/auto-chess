﻿using System;
using UnityEngine;

public class HeroBT : MonoBehaviour {
    Hero hero;
    BTNode root;
    SequenceNode mainLoop;

    void Update() {
        root?.Evaluate();
    }

    public void Initialize() {
        hero ??= GetComponent<Hero>();
        
        // root (>) -----> isAlive
        //         |-----> moveAndAction (>) -----> findTarget
        //                                  |-----> moveToTarget
        //                                  |-----> action (?) -----> attack
        //                                                    |-----> useSkill
        
        mainLoop = new SequenceNode();
        var isAlive = new IsAlive(hero);
        var moveAndAction = new SequenceNode();
        mainLoop.AddChild(isAlive);
        mainLoop.AddChild(moveAndAction);
        var findTarget = new FindTarget(hero);
        var moveToTarget = new MoveToTarget(hero);
        var action = new SelectorNode();
        moveAndAction.AddChild(findTarget);
        moveAndAction.AddChild(moveToTarget);
        moveAndAction.AddChild(action);
        var useSkill = new UseSkill(hero);
        var attack = new Attack(hero);
        action.AddChild(useSkill);
        action.AddChild(attack);
    }

    public void Switch(bool active) {
        root = active ? mainLoop : null;
    }
}