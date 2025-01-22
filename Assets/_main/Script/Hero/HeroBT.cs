using System;
using UnityEngine;

public class HeroBT : MonoBehaviour {
    BTNode root;

    void Update() {
        root?.Evaluate();
    }

    public void Initialize() {
        var hero = GetComponent<Hero>();
        
        // root (>) -----> isAlive
        //         |-----> moveAndAction (>) -----> findTarget
        //                                  |-----> moveToTarget
        //                                  |-----> (!) isMoving
        //                                  |-----> action (?) -----> attack
        //                                                    |-----> useSkill
        
        var mainLoop = new SequenceNode();
        root = mainLoop;
        var isAlive = new IsAlive(hero);
        var moveAndAction = new SequenceNode();
        mainLoop.AddChild(isAlive);
        mainLoop.AddChild(moveAndAction);
        var findTarget = new FindTarget(hero);
        var moveToTarget = new MoveToTarget(hero);
        var notMoving = new Inverter(new IsMoving(hero));
        var action = new SelectorNode();
        moveAndAction.AddChild(findTarget);
        moveAndAction.AddChild(moveToTarget);
        moveAndAction.AddChild(notMoving);
        moveAndAction.AddChild(action);
        var useSkill = new UseSkill(hero);
        var attack = new Attack(hero);
        action.AddChild(useSkill);
        action.AddChild(attack);
    }
}