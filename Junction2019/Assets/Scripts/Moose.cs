using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moose : MonoBehaviour
{
    public Animator animator;
    public Collider col;
    private float lookingAt;
    public Eye[] eyes;

    private void Update()
    {
        bool lookedAt = GazeManager.instance.gazeHitTarget == col;
        if (lookedAt)
        {
            lookingAt = 1.5f;
        } else
        {
            lookingAt = Mathf.Max(0, lookingAt - Time.deltaTime);
        }
        float target = animator.GetFloat("Blend");
        target = Mathf.Lerp(target, Mathf.Min(1.0f, lookingAt), Time.deltaTime * 20.0f);
        animator.SetFloat("Blend", target);

        for(int i = 0; i < eyes.Length; ++i)
        {
            eyes[i].lookedAt = lookedAt;
        }
    }
}
