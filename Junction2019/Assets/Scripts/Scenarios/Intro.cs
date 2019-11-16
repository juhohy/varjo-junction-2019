using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    public TalkAndDo introTalk;

    void OnEnable()
    {
        introTalk.Perform();
    }
}
