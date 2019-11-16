using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moose : MonoBehaviour
{
    public Animator animator;
    public Collider col;
    private float lookingAt;

    private void Update()
    {
        if(GazeManager.instance.gazeHitTarget == col)
        {
            lookingAt = 2.0f;
        } else
        {
            lookingAt = Mathf.Max(0, lookingAt - Time.deltaTime);
        }
        float target = animator.GetFloat("Blend");
        target = Mathf.Lerp(target, Mathf.Min(1.0f, lookingAt), Time.deltaTime * 20.0f);
        animator.SetFloat("Blend", target);
    }
}
