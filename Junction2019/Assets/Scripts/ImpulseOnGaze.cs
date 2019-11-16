using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ImpulseOnGaze : MonoBehaviour
{
    public Vector3 velocity = Vector3.forward;
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
            GetComponent<Rigidbody>().AddForce(transform.TransformDirection(velocity), ForceMode.VelocityChange);
            done = true;
        }
    }
}
