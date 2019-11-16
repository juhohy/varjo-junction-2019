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
        Debug.LogFormat("Card with longest gaze: {0}", GetCardWithLongestGaze()?.name);
        endcription.Perfrom();
    }

    public InkCard GetCardWithLongestGaze()
    {
        InkCard mostWatched = null;
        foreach(InkCard card in cards.GetComponentsInChildren<InkCard>())
        {
            if (mostWatched == null || card.gazeSeconds > mostWatched.gazeSeconds)
            {
                mostWatched = card;
            }
        }
        return mostWatched;
    }
}
