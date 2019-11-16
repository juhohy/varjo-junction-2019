using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Varjo.VarjoManager.Instance.HeadTransform);
    }
}
