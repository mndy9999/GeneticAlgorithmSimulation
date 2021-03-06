﻿using UnityEngine;
using FiniteStateMachine;
using System.Collections;

public class AI_Impress : State<AI>
{
    State<AI> bestState;
    private static AI_Impress _instance;
    private AI_Impress()
    {
        if (_instance != null)
            return;
        _instance = this;
    }
    //get instance of the state
    public static AI_Impress instance
    {
        get
        {
            //if there is no insteance, create one
            if (_instance == null)
                new AI_Impress();     //create one
            return _instance;
        }
    }

    public override float GetWeight(AI _owner) { return _owner.critter.critterTraitsDict[Trait.Beauty]; }

    public override void EnterState(AI _owner)
    {
        //Debug.Log("Entering Impress State");
        _owner.animator.Play("ShowOff");      //play animation when entering state
        _owner.seek.Target.GetComponent<Critter>().isCourted = true;
    }

    public override void ExitState(AI _owner)
    {
        //Debug.Log("Exiting Impress State");
        _owner.seek.LastKnownPotentialMate.GetComponent<Critter>().isCourted = false;
        _owner.critter.ResetBreed();
        _owner.StopAllCoroutines();
    }

    public override void UpdateState(AI _owner)
    {
        _owner.StartCoroutine(WaitForAnimation(_owner));

    }

    IEnumerator WaitForAnimation(AI _owner)
    {

        yield return new WaitForSeconds(2);
        if (_owner.IsDead() && _owner.critter.availableBehaviours.Contains(AI_Dead.instance)) { _owner.stateMachine.ChangeState(AI_Dead.instance); }
        else if (_owner.critter.IsAttacked && _owner.critter.availableBehaviours.Contains(AI_Attack.instance)) { _owner.stateMachine.ChangeState(AI_Attack.instance); }
        if (Random.Range(0, 10) < _owner.critter.critterTraitsDict[Trait.Beauty])
        {
            _owner.seek.LastKnownPotentialMate.GetComponent<Seek>().Mate = _owner.gameObject;
            _owner.seek.Mate = _owner.seek.LastKnownPotentialMate;
            _owner.stateMachine.ChangeState(AI_Breed.instance);
        }
        yield return new WaitForSeconds(2f);
        if (_owner.CanSeeTarget() && _owner.critter.availableBehaviours.Contains(AI_Chase.instance)) { _owner.stateMachine.ChangeState(AI_Chase.instance); }
        if (_owner.critter.availableBehaviours.Contains(AI_Wander.instance)) { _owner.stateMachine.ChangeState(AI_Wander.instance); }
        if (_owner.critter.availableBehaviours.Contains(AI_Idle.instance)) { _owner.stateMachine.ChangeState(AI_Idle.instance); }
    }
}