using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    public bool lookedAt = false;

    void LateUpdate()
    {
        if (!lookedAt)
        {
            transform.LookAt(Varjo.VarjoManager.Instance.HeadTransform);
        }
    }
}
