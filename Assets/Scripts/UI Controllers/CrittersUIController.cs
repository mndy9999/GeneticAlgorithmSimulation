﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CrittersUIController : MonoBehaviour
{
    public GameObject panel;
    public GameObject currentGO;
    public GameObject currentPanel;
    public GameObject ring;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Critters")))
            {
                
                if (hit.transform.GetComponent<Critter>())
                    currentGO = hit.transform.gameObject;
                panel.gameObject.SetActive(true);
                ring.SetActive(true);
            }
            else if (!EventSystem.current.IsPointerOverGameObject(-1)) { ring.gameObject.SetActive(false); panel.gameObject.SetActive(false); }
        }
        if (panel && currentGO) UpdateInfo();
        else
        {
            ring.gameObject.SetActive(false);
            panel.gameObject.SetActive(false);
        }
    }

    void UpdateInfo()
    {
        
        panel.transform.Find("Name").GetComponent<Text>().text = currentGO.GetComponent<Critter>().Name;
        panel.transform.Find("Age").GetComponent<Text>().text = currentGO.GetComponent<Critter>().age.ToString();
        panel.transform.Find("Gender").GetComponent<Text>().text = currentGO.GetComponent<Critter>().gender.ToString();
        panel.transform.Find("LifeStage").GetComponent<Text>().text = currentGO.GetComponent<Critter>().lifeStage.ToString();
        panel.transform.Find("State").GetComponent<Text>().text = currentGO.transform.root.GetComponent<AI>() ? currentGO.GetComponent<AI>().currentState : "N/A";
        if (panel.transform.GetChild(0).gameObject.activeSelf) { infoPanel(); }
        if (panel.transform.GetChild(1).gameObject.activeSelf) { statsPanel(); }
        if (panel.transform.GetChild(2).gameObject.activeSelf) { behavioursPanel(); }

        
        ring.transform.position = currentGO.transform.position;
    }

    void infoPanel()
    {
        currentPanel = panel.transform.GetChild(0).transform.GetChild(0).gameObject;
        currentPanel.transform.Find("Energy").GetComponent<Text>().text = currentGO.GetComponent<Critter>().Energy.ToString();
        currentPanel.transform.Find("Health").GetComponent<Text>().text = ((int)currentGO.GetComponent<Critter>().Health).ToString();
        currentPanel.transform.Find("Resources").GetComponent<Text>().text = currentGO.GetComponent<Critter>().Resource.ToString();
    }

    void statsPanel()
    {
        currentPanel = panel.transform.GetChild(1).transform.GetChild(0).gameObject;
        currentPanel.transform.Find("WalkSpeed").GetComponent<Text>().text = currentGO.GetComponent<Critter>().critterTraitsDict[Trait.WalkSpeed].ToString();
        currentPanel.transform.Find("RunSpeed").GetComponent<Text>().text = currentGO.GetComponent<Critter>().critterTraitsDict[Trait.RunSpeed].ToString();
        currentPanel.transform.Find("Speed").GetComponent<Text>().text = currentGO.GetComponent<AI>().agent.speed.ToString();
        currentPanel.transform.Find("Sight").GetComponent<Text>().text = currentGO.GetComponent<Critter>().critterTraitsDict[Trait.ViewRadius].ToString();
        currentPanel.transform.Find("SightAngle").GetComponent<Text>().text = currentGO.GetComponent<Critter>().critterTraitsDict[Trait.ViewAngle].ToString();
        //currentPanel.transform.Find("ThreatPoints").GetComponent<Text>().text = currentGO.GetComponent<Critter>().critterTraitsDict[Trait.Age].ToString();
        //currentPanel.transform.Find("Beauty").GetComponent<Text>().text = currentGO.GetComponent<Critter>().critterTraitsDict[Trait.Age].ToString();
    }

    void behavioursPanel()
    {        
        currentPanel = panel.transform.GetChild(2).transform.GetChild(0).gameObject;
        if (currentGO.GetComponent<Critter>().availableBehaviours.Count > 0)
        {
            currentPanel.GetComponent<Text>().text = "";
            for (int i = 0; i < currentGO.GetComponent<Critter>().availableBehaviours.Count-1; i+=2)
            {
                if (!currentPanel.GetComponent<Text>().text.Contains(currentGO.GetComponent<Critter>().availableBehaviours[i].ToString()))
                    currentPanel.GetComponent<Text>().text += currentGO.GetComponent<Critter>().availableBehaviours[i].ToString() + "   " + currentGO.GetComponent<Critter>().availableBehaviours[i+1].ToString() + "    " + currentGO.GetComponent<Critter>().availableBehaviours[i + 2].ToString() + "\n";
            }
        }
        else { currentPanel.GetComponent<Text>().text = "N/A"; }
    }

}
