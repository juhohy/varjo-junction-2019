using System.Collections;
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

    public void NextScenario()
    {
        currentScenario++;
        LoadCurrentScenario();
    }

    void LoadCurrentScenario()
    {
        Debug.Log("Load scenario " + currentScenario.ToString() + ": " + scenarios[currentScenario].name);
        for (int i = 0; i < scenarios.Count; ++i)
        {
            scenarios[i].SetActive(i == currentScenario);
        }
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
