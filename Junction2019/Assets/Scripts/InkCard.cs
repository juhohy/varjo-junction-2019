using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkCard : MonoBehaviour
{
    public float gazeSeconds { get; private set; }
    private void Update()
    {
        if (GazeManager.instance.gazeHitTarget?.GetComponentInParent<InkCard>() == this)
        {
            gazeSeconds += Time.deltaTime;
        }
    }
}
