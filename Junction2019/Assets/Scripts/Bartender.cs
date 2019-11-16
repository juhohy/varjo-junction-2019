using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bartender : MonoBehaviour
{
    public static Bartender Instance;
    public Animator animator;
    public bool talk = false;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        float target = animator.GetFloat("Blend");
        target = Mathf.Lerp(target, talk ? 1.0f : 0.0f, Time.deltaTime * 5.0f);
        animator.SetFloat("Blend", target);
    }
}
