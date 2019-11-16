using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorillaBalls : MonoBehaviour
{
    public TalkAndDo description;
    public TalkAndDo endcription;

    public Transform ballPos1;
    public Transform ballPos2;
    public Transform ball;
    public AnimationCurve ballCurve;
    public float ballCurveMultiplier = 1.0f;
    int currentThrow;
    int throwAmount;


    void OnEnable()
    {
        currentThrow = 0;
        ball.gameObject.SetActive(false);
        ball.position = ballPos1.position;
        description.Perfrom();
    }

    void RunTest()
    {
        StartCoroutine("TestRoutine");
    }

    void EndTest()
    {
        endcription.Perfrom();
    }

    IEnumerator TestRoutine()
    {
        bool dir1to2 = true;
        while (currentThrow < throwAmount)
        {
            yield return new  WaitForSeconds(1.0f);

            float throwVal = 0.0f;
            while(throwVal < 1.0f)
            {
                ball.position = dir1to2 ? Vector3.Lerp(ballPos1.position, ballPos2.position, throwVal) : Vector3.Lerp(ballPos2.position, ballPos1.position, throwVal);
                float height = ballCurve.Evaluate(throwVal) * ballCurveMultiplier;
                ball.position += Vector3.up * height;

                throwVal += Time.deltaTime;
                yield return null;
            }

            throwAmount++;
            dir1to2 = !dir1to2;
            yield return null;
        }

    }
}
