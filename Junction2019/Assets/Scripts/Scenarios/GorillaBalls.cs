using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorillaBalls : MonoBehaviour
{
    public TalkAndDo description;
    public TalkAndDo centeription;
    public TalkAndDo endcriptionCorrect;
    public TalkAndDo endcriptionWrong;

    public Transform ballPos1;
    public Transform ballPos2;
    public Transform ball;
    public AnimationCurve ballCurve;
    public float ballCurveMultiplier = 1.0f;
    int currentThrow;
    int throwAmount=10;

    public AnimateLocalPosition bear;

    public TextMesh counterMesh;
    public GameObject counter;
    public int count;
    private bool okPressed;

    void OnEnable()
    {
        counter.SetActive(false);
        count = 0;
        currentThrow = 0;
        counterMesh.text = count.ToString();
        ball.gameObject.SetActive(false);
        ball.position = ballPos1.position;
        description.Perfrom();
    }

    void RunTest()
    {
        ball.gameObject.SetActive(true);
        StartCoroutine("TestRoutine");

        Invoke("DoBear", 4.7f);
    }

    void DoBear()
    {
        bear.Play();
    }

    IEnumerator TestRoutine()
    {
        Debug.Log("Test routine");

        bool dir1to2 = true;
        while (currentThrow < throwAmount)
        {
            yield return new WaitForSeconds(1.0f);

            float throwVal = 0.0f;
            while (throwVal < 1.0f)
            {
                ball.position = dir1to2 ? Vector3.Lerp(ballPos1.position, ballPos2.position, throwVal) : Vector3.Lerp(ballPos2.position, ballPos1.position, throwVal);
                float height = ballCurve.Evaluate(throwVal) * ballCurveMultiplier;
                ball.position += Vector3.up * height;

                throwVal += Time.deltaTime;
                yield return null;
            }

            currentThrow++;
            dir1to2 = !dir1to2;
            yield return null;
        }

        centeription.Perfrom();
    }

    public void AddToCount()
    {
        if (okPressed)
        {
            return;
        }
        count++;
        counterMesh.text = count.ToString();
    }

    public void ReduceFromCount()
    {
        if (okPressed)
        {
            return;
        }
        count--;
        counterMesh.text = count.ToString();
    }

    void ShowCounter()
    {
        ball.gameObject.SetActive(false);
        counter.SetActive(true);
    }

    public void CountOk()
    {
        if(okPressed)
        {
            return;
        }
        okPressed = true;
        Invoke("CountOkDelayed", 0.5f);
    }

    private void CountOkDelayed() {
        if (count == throwAmount)
        {
            endcriptionCorrect.Perfrom();
        }
        else
        {
            endcriptionWrong.Perfrom();
        }
    }
}
