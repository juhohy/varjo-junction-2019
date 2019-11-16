using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalFan : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up * 30.0f * Time.deltaTime, Space.World);
    }
}
