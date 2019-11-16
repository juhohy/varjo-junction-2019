using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moose : MonoBehaviour
{
    public Animator animator;
    public Collider col;

    private void Update()
    {
        bool lookingAt = GazeManager.instance.gazeHitTarget == col;
        float target = animator.GetFloat("Blend");
        target = Mathf.Lerp(target, lookingAt ? 1.0f : 0.0f, Time.deltaTime * 10.0f);
        animator.SetFloat("Blend", target);
    }
}
