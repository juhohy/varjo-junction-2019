﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    int currentScenario = 0;
    public List<GameObject> scenarios = new List<GameObject>();

    void Start()
    {
        LoadCurrentScenario();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            NextScenario();
        }
    }

    void NextScenario()
    {
        currentScenario++;
        LoadCurrentScenario();
    }

    void LoadCurrentScenario()
    {
        for(int i = 0; i < scenarios.Count; ++i)
        {
            scenarios[i].SetActive(i == currentScenario);
        }
    }
}