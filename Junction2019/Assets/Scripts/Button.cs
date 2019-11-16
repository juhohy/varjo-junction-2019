using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject target;
    public string message;

    void Awake()
    {
        transform.tag = "Button";
    }

    public void Press()
    {
        target.SendMessage(message);
    }
}
