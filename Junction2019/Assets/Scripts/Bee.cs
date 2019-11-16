using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up + transform.right, 240.0f * Time.deltaTime, Space.World);
    }
}
