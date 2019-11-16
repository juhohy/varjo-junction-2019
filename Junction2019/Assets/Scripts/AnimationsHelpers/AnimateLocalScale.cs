using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateLocalScale : MonoBehaviour
{
    public bool autoStart = false;
    public float duration = 1.0f;
    public bool loop = false;
    public bool ignoreStartValue = false;
    public AnimationCurve X = new AnimationCurve();
    public AnimationCurve Y = new AnimationCurve();
    public AnimationCurve Z = new AnimationCurve();

    float timer = 0.0f;
    Vector3 startValue = new Vector3();

    void Start()
    {
        if (ignoreStartValue)
        {
            startValue = Vector3.one;
        }
        else
        {
            startValue = transform.localScale;
        }
        if (X.length == 0) { X.AddKey(0, 1); }
        if (Y.length == 0) { Y.AddKey(0, 1); }
        if (Z.length == 0) { Z.AddKey(0, 1); }
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
            transform.localScale = new Vector3(startValue.x * X.Evaluate(timer), startValue.y * Y.Evaluate(timer), startValue.z * Z.Evaluate(timer));
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

        Update();
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
        transform.localScale = startValue;
        enabled = false;
    }
}
