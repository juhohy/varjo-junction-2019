using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateLocalRotation : MonoBehaviour
{
    public bool autoStart = false;
    public float duration = 1.0f;
    public bool loop = false;
    public AnimationCurve X = new AnimationCurve();
    public AnimationCurve Y = new AnimationCurve();
    public AnimationCurve Z = new AnimationCurve();

    float timer = 0.0f;
    Vector3 startValue = new Vector3();

    void Start()
    {
        startValue = transform.localEulerAngles;
        if (X.length == 0) { X.AddKey(0, 0); }
        if (Y.length == 0) { Y.AddKey(0, 0); }
        if (Z.length == 0) { Z.AddKey(0, 0); }
        ResetValues();

        if (autoStart)
        {
            Play();
        }
    }

    void Update()
    {
        timer += Time.deltaTime / duration;
        if (timer < 1.0f)
        {
            transform.localEulerAngles = new Vector3(startValue.x + X.Evaluate(timer), startValue.y + Y.Evaluate(timer), startValue.z + Z.Evaluate(timer));
        }
        else
        {
            ResetValues();
            if (loop)
            {
                enabled = true;
            }
        }
    }

    public void Play()
    {
        if (enabled)
        {
            Restart();
        }

        enabled = true;
    }
    public void Pause()
    {
        enabled = false;
    }

    public void Restart()
    {
        timer = 0.0f;
        enabled = true;
    }

    public void ResetValues()
    {
        timer = 0.0f;
        transform.localEulerAngles = startValue;
        enabled = false;
    }
}
