using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorillaBalls : MonoBehaviour
{
    public TalkAndDo description;
    public TalkAndDo endcription;

    void OnEnable()
    {
        description.Perfrom();
    }

    void RunTest()
    {
        Invoke("EndTest", 10.0f);
    }

    void EndTest()
    {
        endcription.Perfrom();
    }

}
