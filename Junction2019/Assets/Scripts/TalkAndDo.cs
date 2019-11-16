using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkAndDo : MonoBehaviour
{
    public AudioClip talkClip;
    public GameObject targetObject;
    public string targetAction;
    public float waitBefore = 0.0f;
    public float waitAfter = 0.0f;

    public void Perfrom()
    {
        Debug.Log("Talk: " + talkClip.name, gameObject);
        Invoke("PerformDelayed", waitBefore);
    }

    private void PerformDelayed()
    {
        AudioManager.Instance.PlaySound(talkClip, transform.position);
        Invoke("Do", talkClip.length + waitAfter);
    }

    void Do()
    {
        Debug.Log("Do: " + targetAction, gameObject);
        targetObject.SendMessage(targetAction);
    }
}
