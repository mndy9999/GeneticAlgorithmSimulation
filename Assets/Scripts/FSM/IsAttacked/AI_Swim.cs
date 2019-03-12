﻿using UnityEngine;
using FiniteStateMachine;
using System.Collections;

public class AI_Swim : State<AI>
{
    private static AI_Swim _instance;
    private static string _name = "swim";
    private AI_Swim()
    {
        if (_instance != null)
            return;
        _instance = this;
    }
    //get instance of the state
    public static AI_Swim instance
    {
        get
        {
            //if there is no insteance, create one
            if (_instance == null)
                new AI_Swim();     //create one
            return _instance;
        }
    }

    public static string name
    {
        get { return _name; }
        set { _name = value; }
    }

    public override void EnterState(AI _owner)
    {
        Debug.Log("Entering Swim State");
        _owner.animator.Play("Run");      //play animation when entering state
        _owner.agent.SetDestination(_owner.seek.water.GetComponent<Collider>().bounds.center);
    }

    public override void ExitState(AI _owner)
    {
        Debug.Log("Exiting Swim State");
    }

    public override void UpdateState(AI _owner)
    {
        if (_owner.IsDead()) { _owner.stateMachine.ChangeState(AI_Dead.instance); }
        else if (_owner.critter.IsAttacked) { _owner.stateMachine.ChangeState(AI_Attack.instance); }
        else if (_owner.CanSeeEnemy()) if (_owner.seek.Enemy.GetComponent<CheckEnvironment>().InWater) { _owner.stateMachine.ChangeState(AI_Evade.instance); }
        else if(!_owner.CanSeeEnemy()) _owner.StartCoroutine(ChangeState(_owner));
       
    }

    IEnumerator ChangeState(AI _owner)
    {
        _owner.critter.IsAlarmed = false;
        yield return new WaitForSeconds(3);
        _owner.stateMachine.ChangeState(AI_Evade.instance);
    }
}