using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    public TalkAndDo endSpeech;

    void OnEnable()
    {
        endSpeech.Perform();
    }

}
