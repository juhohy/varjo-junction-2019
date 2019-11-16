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
        Bartender.Instance.talk = true;
        AudioManager.Instance.PlaySound(talkClip, Bartender.Instance.transform.position + Vector3.up * 1.8f);
        Invoke("Do", talkClip.length + waitAfter);
    }

    void Do()
    {
        Bartender.Instance.talk = false;
        Debug.Log("Do: " + targetAction, gameObject);
        targetObject.SendMessage(targetAction);
    }
}
