using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varjo;

public class GazeCal : MonoBehaviour
{
    public TalkAndDo description;
    public TalkAndDo endcription;
    public ScenarioManager scenarioManager;
    bool calibrated = false;

    void OnEnable()
    {
        calibrated = false;
        description.Perfrom();
    }

    void Update()
    {
        if(!calibrated)
        { 
            VarjoPlugin.GazeData data = VarjoPlugin.GetGaze();
            if(data.status == VarjoPlugin.GazeStatus.VALID)
            {
                calibrated = true;
                endcription.Perfrom();
            }
        }
    }

}
