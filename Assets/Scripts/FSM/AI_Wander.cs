﻿using UnityEngine;
using FiniteStateMachine;

public class AI_Wander : State<AI>
{
    private static AI_Wander _instance;
    private AI_Wander()
    {
        if (_instance != null)
            return;
        _instance = this;
    }
    //get the instance of the state
    public static AI_Wander instance
    {
        get
        {
            //if there is no instance
            if (_instance == null)
                new AI_Wander();      //create one
            return _instance;
        }
    }
    public override void EnterState(AI _owner)
    {
        _owner.critter.speed = _owner.critter.walkSpeed;
        Debug.Log("Entering Wander State");
        _owner.animator.Play("Wander");  //start playing the animation when entering state
        _owner.genWaypoint();
        
    }

    public override void ExitState(AI _owner)
    {
        Debug.Log("Exiting Wander State");
    }

    public override void UpdateState(AI _owner)
    {
        if (_owner.IsDead()) { _owner.stateMachine.ChangeState(AI_Dead.instance); }
        if (_owner.critter.IsAttacked) { _owner.stateMachine.ChangeState(AI_Attack.instance); }
        if (_owner.CanSeeEnemy()) { _owner.stateMachine.ChangeState(AI_Evade.instance); }
        if (_owner.CanSeeTarget()) { _owner.stateMachine.ChangeState(AI_Chase.instance); }
        else if (!_owner.switchState) { _owner.stateMachine.ChangeState(AI_Idle.instance); }

        //calculate the direction and rotation, and start moving towards the target
        var direction = _owner.waypoint - _owner.transform.position;
        _owner.transform.rotation = Quaternion.Slerp(_owner.transform.rotation,
                                    Quaternion.LookRotation(direction),
                                    _owner.critter.speed * Time.deltaTime);
        _owner.transform.Translate(0, 0, Time.deltaTime * _owner.critter.speed);
    }


}
