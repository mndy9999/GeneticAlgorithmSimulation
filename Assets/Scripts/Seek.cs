﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Seek : MonoBehaviour {

    public string enemyType = "Carnivore";
    public string defaultEnemyType;

    public float viewRadius;        //how much can it see
    public float viewAngle;         //how far can it see

    //different types of tagets
    [SerializeField]
    GameObject target = null;
    [SerializeField]
    GameObject challenger = null;
    [SerializeField]
    GameObject food = null;
    [SerializeField]
    GameObject enemy = null;   
    [SerializeField]
    GameObject potentialMate = null;
    [SerializeField]
    GameObject mate = null;
    [SerializeField]
    GameObject courter = null;
    [SerializeField]
    GameObject opponent = null;

    //keep track of the last known targets
    GameObject lastKnownTarget = null;
    GameObject lastKnownFood = null;
    GameObject lastKnownEnemy= null;
    GameObject lastKnownMate = null;
    GameObject lastKnownPotentialMate = null;
    GameObject lastKnownCourter = null;
    GameObject lastKnownOpponent = null;
    GameObject lastKnownChallenger = null;

    GameObject temp = null;

    public List<GameObject> visibleTargets = new List<GameObject>();    //all the gameobjects in the field of view
    public List<string> availableTargetsType;       //food types the critter will look after

    [SerializeField] GameObject[] potentialTargets;     //holds the closest available targets of each type that are in the field of view

    Critter critter;

    public GameObject water;    //this is to check if it can see water

    private void Start()
    {
        potentialTargets = new GameObject[7];
        defaultEnemyType = enemyType;       //save the default enemy type
        critter = GetComponent<Critter>();
        availableTargetsType = critter.availableTargetTypes;

        //get view radius and view angle from the critter component
        viewRadius = critter.critterTraitsDict[Trait.ViewRadius];
        viewAngle = critter.viewAngle;

        //look for all the colliders in the field of view
        //FindVisibleTargets();

        //find specific targets that are in the field of view
        food = GetFood();
        enemy = GetEnemy();
        potentialMate = GetPotantialMate();
        opponent = GetOpponent();

        //setup the array with all the possible targets, sorted based on their importance
        potentialTargets[0] = enemy;
        potentialTargets[1] = challenger;
        potentialTargets[2] = mate;
        potentialTargets[3] = courter;       
        potentialTargets[4] = food;
        potentialTargets[5] = potentialMate;
        potentialTargets[6] = opponent;

        //set the current target to the most important potential target
        target = null;
        for (int i = 0; i < potentialTargets.Length; i++)
        {
            if (potentialTargets[i] != null) { target = potentialTargets[i]; break; }
        }

        //save informaion about the last known targets
        if (target) { lastKnownTarget = target; }
        if (food) { lastKnownFood = food; }
        if (enemy) { lastKnownEnemy = enemy; }
        if (mate) { lastKnownMate = mate; }
        if (potentialMate) { lastKnownPotentialMate = mate; }
        if (courter) { lastKnownCourter = mate; }
        if (opponent) { lastKnownOpponent = opponent; }

        StartCoroutine("FindTargetsWithDelay", .2f);

    }
    private void Update()
    {
        viewAngle = critter.viewAngle; //update view angle from the critter component

        //look for all the colliders in the field of view
        //FindVisibleTargets();

        //find specific targets that are in the field of view
        //food = GetFood();
        //enemy = GetEnemy();
        //potentialMate = GetPotantialMate();
        //opponent = GetOpponent();

        //setup the array with all the possible targets, sorted based on their importance
        potentialTargets[0] = enemy;
        potentialTargets[1] = challenger;
        potentialTargets[2] = courter;
        potentialTargets[3] = mate;
        potentialTargets[4] = food;
        potentialTargets[5] = potentialMate;
        potentialTargets[6] = opponent;

        //set the current target to the most important potential target
        target = null;
        for (int i = 0; i < potentialTargets.Length; i++)
        {
            if(potentialTargets[i] != null) { target = potentialTargets[i]; break; }
        }

        //save informaion about the last known targets
        if (target) { lastKnownTarget = target; }
        if (food) { lastKnownFood = food; }
        if (enemy) { lastKnownEnemy = enemy; }
        if (mate) { lastKnownMate = mate; }
        if (potentialMate) { lastKnownPotentialMate = potentialMate; }
        if (courter) { lastKnownCourter = courter; }
        if (opponent) { lastKnownOpponent = opponent; }

    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();

            food = GetFood();
            enemy = GetEnemy();
            potentialMate = GetPotantialMate();
            opponent = GetOpponent();

        }
    }


    void FindVisibleTargets()
    {
        water = null;       //reset the water each time it looks for new targets
        visibleTargets.Clear();     //reset the visible targets to add new ones
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius);      //create a sphere around the critter and put all the colliders in it into an array
        for (int i = 0; i < targetInViewRadius.Length; i++) //loop through every gabe object in the view radius
        {            
            GameObject target2 = targetInViewRadius[i].gameObject;  
            Vector3 direction = (target2.transform.position - transform.position).normalized;
            if (target2.transform.root.GetComponent<Critter>())
            {
                //if the game object is visibile and is in the field of view OR is close to the critter
                if ((Vector3.Angle(transform.forward, direction) < viewAngle / 2 || Vector3.Distance(transform.position, target2.transform.position) < 5.0f) 
                    && target2.transform.root.gameObject.GetComponent<Critter>().isVisible)
                {
                    visibleTargets.Add(target2.transform.root.gameObject);      //add it to the visible targets
                }
            }
            if(target2.layer == LayerMask.NameToLayer("Water")) { water = target2; }    //if the colliders is water, keep track of it
        }
    }

    GameObject GetFood()
    {
        temp = null;
        float dist = Mathf.Infinity;
        for (int i = 0; i < visibleTargets.Count; i++)  //loop through all the visible targets
        {
            float d = Vector3.Distance(transform.position, visibleTargets[i].transform.position);
            //then find the closest food type critter
            if(d < dist && availableTargetsType.Contains(visibleTargets[i].GetComponent<Critter>().critterType)) 
            {
                dist = d;
                temp = visibleTargets[i];
            }
        }
        return temp;
    }
    GameObject GetEnemy()
    {
        temp = null;
        float dist = Mathf.Infinity;
        for (int i = 0; i < visibleTargets.Count; i++)      //loop through all the visible targets
        {
            float d = Vector3.Distance(transform.position, visibleTargets[i].transform.position);
            //find the closest enemy type critter
            if (d < dist && visibleTargets[i].GetComponent<Critter>().critterType == enemyType && visibleTargets[i].GetComponent<Critter>().IsAlive)
            {
                dist = d;
                temp = visibleTargets[i];
            }
        }
        return temp;
    }
    GameObject GetPotantialMate()
    {
        temp = null;
        float dist = Mathf.Infinity;
        for (int i = 0; i < visibleTargets.Count; i++)      //loop through all the visible targets
        {
            float d = Vector3.Distance(transform.position, visibleTargets[i].transform.position);
            //find the closest critter from the same specied, different gender and can breed
            if (d < dist && critter.critterType == visibleTargets[i].GetComponent<Critter>().critterType && critter.gender != visibleTargets[i].GetComponent<Critter>().gender && critter.canBreed && visibleTargets[i].GetComponent<Critter>().canBreed && visibleTargets[i].GetComponent<Critter>().IsAlive)
            {
                dist = d;
                temp = visibleTargets[i];
            }
        }
        return temp;
    }
    GameObject GetOpponent()
    {
        temp = null;
        float dist = Mathf.Infinity;
        for (int i = 0; i < visibleTargets.Count; i++)      //loop through all the visible targets
        {
            float d = Vector3.Distance(transform.position, visibleTargets[i].transform.position);
            //find the closest critter of the same species, same gender and can be challenged
            if (d < dist && critter.critterType == visibleTargets[i].GetComponent<Critter>().critterType && critter.gender == visibleTargets[i].GetComponent<Critter>().gender && critter.canChallenge && visibleTargets[i].GetComponent<Critter>().canChallenge && critter.gameObject != visibleTargets[i].gameObject && !visibleTargets[i].GetComponent<Critter>().IsAlarmed && !critter.IsAlarmed && visibleTargets[i].GetComponent<Critter>().IsAlive)
            {
                dist = d;
                temp = visibleTargets[i];
            }
        }
        return temp;
    }

    //getters and setters
    public GameObject Target
    {
        get { return target; }
        set { target = value; }
    }
    public GameObject Food
    {
        get { return food; }
        set { food = value; }
    }
    public GameObject Enemy
    {
        get { return enemy; }
        set { enemy = value; }
    }
    public GameObject PotentialMate
    {
        get { return potentialMate; }
        set { potentialMate = value; }
    }
    public GameObject Mate
    {
        get { return mate; }
        set { mate = value; }
    }
    public GameObject Courter
    {
        get { return  courter; }
        set { courter = value; }
    }
    public GameObject Opponent
    {
        get { return opponent; }
        set { opponent = value; }
    }
    public GameObject Challenger
    {
        get { return challenger; }
        set { challenger = value; }
    }


    public GameObject LastKnownTarget
    {
        get { return lastKnownTarget; }
        set { lastKnownTarget = value; }
    }
    public GameObject LastKnownfood
    {
        get { return lastKnownFood; }
        set { lastKnownFood = value; }
    }
    public GameObject LastKnownEnemy
    {
        get { return lastKnownEnemy; }
        set { lastKnownEnemy = value; }
    }
    public GameObject LastKnownPotentialMate
    {
        get { return lastKnownPotentialMate; }
        set { lastKnownPotentialMate = value; }
    }
    public GameObject LastKnownMate
    {
        get { return lastKnownMate; }
        set { lastKnownMate = value; }
    }
    public GameObject LastKnownCourter
    {
        get { return lastKnownCourter; }
        set { lastKnownCourter = value; }
    }
    public GameObject LastKnownOpponent
    {
        get { return lastKnownOpponent; }
        set { lastKnownOpponent = value; }
    }
    public GameObject LastKnownChallenger
    {
        get { return lastKnownChallenger; }
        set { lastKnownChallenger = value; }
    }


    //calculate direction from angle
    Vector3 DirFromAngle(float angleDegrees, bool isGlobal)
    {
        if (!isGlobal) { angleDegrees += transform.eulerAngles.y; }
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmos()
    {
        if (GetComponent<Critter>().critterType != "Vegetable")
        {
            Gizmos.DrawWireSphere(transform.position, viewRadius);

            Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
            Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

            Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
            Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
        }
    }


}
