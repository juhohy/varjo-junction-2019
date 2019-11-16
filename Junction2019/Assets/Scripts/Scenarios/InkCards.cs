using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkCards : MonoBehaviour
{
    public TalkAndDo description;
    public TalkAndDo endcription;
    public GameObject cards;
    public float time = 20.0f;

    private void OnEnable()
    {
        cards.SetActive(false);
        description.Perform();
    }

    public void ShowCards()
    {
        cards.SetActive(true);
        Invoke("HideCards", time);
    }

    public void HideCards()
    {
        cards.SetActive(false);
        InkCard longestGazeCard = GetCardWithLongestGaze();
        longestGazeCard.transform.SetParent(null, true);
        longestGazeCard.gameObject.SetActive(true);
        Debug.LogFormat("Card with longest gaze: {0}", GetCardWithLongestGaze()?.name);
        endcription.Perform();
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
