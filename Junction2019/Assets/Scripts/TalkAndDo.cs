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
        Debug.Log("Talk: " + talkClip.name, gameObject);
        AudioManager.Instance.PlaySound(talkClip, transform.position);
        Invoke("Do", talkClip.length);
    }

    void Do()
    {
        Debug.Log("Do: " + targetAction, gameObject);
        targetObject.SendMessage(targetAction);
    }

}
