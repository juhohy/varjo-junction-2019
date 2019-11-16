using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    public float startDelay = 10.0f;

    int currentScenario = 0;
    public List<GameObject> scenarios = new List<GameObject>();
    public List<AudioClip> scenarioMusics = new List<AudioClip>();
    public List<float> scenarioMusicVolumes = new List<float>();
    AudioSource musicSource;

    void Awake()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        Invoke("LoadCurrentScenario", startDelay);
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

        if(scenarioMusics.Count > currentScenario)
        {
            if(scenarioMusics[currentScenario] != null)
            { 
                Debug.Log("Playing scenario music: " + scenarioMusics[currentScenario].name);
                musicSource.clip = scenarioMusics[currentScenario];
                musicSource.volume = scenarioMusicVolumes[currentScenario];
                musicSource.Play();
            }
            else
            {
                musicSource.Stop();
            }
        }
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
