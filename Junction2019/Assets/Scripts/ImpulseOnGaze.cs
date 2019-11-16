using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ImpulseOnGaze : MonoBehaviour
{
    public Vector3 velocity = Vector3.forward;
    public float randomRotationDeg = 0.0f;
    public float randomFactor = 1.0f;
    private bool done;

    private void Update()
    {
        Debug.Log(Varjo.VarjoPlugin.GetGaze().status);
        if (done)
        {
            return;
        }

        if (GazeManager.instance.gazeHitTarget?.GetComponentInParent<ImpulseOnGaze>() == this)
        {
            Vector3 dir = Quaternion.AngleAxis(
                Random.Range(-randomRotationDeg, +randomRotationDeg), 
                Vector3.up
            ) * velocity * Random.Range(1.0f, randomFactor);
            GetComponent<Rigidbody>().AddForce(transform.TransformDirection(dir), ForceMode.VelocityChange);
            done = true;
            AudioManager.Instance.PlaySound("glass_klonk", transform.position);
        }
    }
}
