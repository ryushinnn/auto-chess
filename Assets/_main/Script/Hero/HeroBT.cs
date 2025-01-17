using System;
using UnityEngine;

public class HeroBT : MonoBehaviour {
    BTNode root;

    void Start() {
        Initialize();
    }

    void Update() {
        root.Evaluate();
    }

    void Initialize() {
        var hero = GetComponent<Hero>();
        
        // root (>) -----> isDead
        //         |-----> moveAndAction (>) -----> findTarget
        //                                  |-----> moveToTarget
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
        var action = new SelectorNode();
        moveAndAction.AddChild(findTarget);
        moveAndAction.AddChild(moveToTarget);
        moveAndAction.AddChild(action);
        var useSkill = new UseSkill(hero);
        var attack = new Attack(hero);
        action.AddChild(useSkill);
        action.AddChild(attack);
    }
}