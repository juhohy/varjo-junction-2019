using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkCard : MonoBehaviour
{
    public float gazeSeconds { get; private set; }
    public MeshRenderer texturePlane;

    private void Start()
    {
        texturePlane.material = new Material(texturePlane.material);
        var texture = Resources.Load<Texture>("Art" + name);
        texturePlane.material.SetTexture("_MainTex", texture);
    }
    private void Update()
    {
        if (GazeManager.instance.gazeHitTarget?.GetComponentInParent<InkCard>() == this)
        {
            gazeSeconds += Time.deltaTime;
        }
    }
}
