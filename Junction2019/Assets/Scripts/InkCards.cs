using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkCards : MonoBehaviour
{
    public TalkAndDo description;
    public TalkAndDo endcription;
    public GameObject cards;

    private void OnEnable()
    {
        cards.SetActive(false);
        description.Perfrom();
    }

    public void ShowCards()
    {
        cards.SetActive(true);
        Invoke("HideCards", 10.0f);
    }

    public void HideCards()
    {
        cards.SetActive(false);
        endcription.Perfrom();
    }
}
