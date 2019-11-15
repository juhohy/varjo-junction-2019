using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkAndDo : MonoBehaviour
{
    public AudioClip talkClip;
    public GameObject targetObject;
    public string targetAction;

    // Start is called before the first frame update
    public void Perfrom()
    {
        AudioManager.Instance.PlaySound(talkClip, transform.position);
        Invoke("Do", talkClip.length);
    }

    void Do()
    {
        targetObject.SendMessage(targetAction);
    }

}
