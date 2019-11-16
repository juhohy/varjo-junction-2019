using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class StareTest : MonoBehaviour
{
    public TalkAndDo description;
    public TalkAndDo endcription;
    public GameObject video;
    public VideoPlayer videoPlayer;
    public float testTime = 240.0f;
    int timesBlinked;

    private void OnEnable()
    {
        timesBlinked = 0;
        video.SetActive(false);
        description.Perfrom();
    }

    public void ShowVideo()
    {
        video.SetActive(true);
        videoPlayer.Play();
        Invoke("HideCards", 240.0f);
    }

    public void HideVideo()
    {
        videoPlayer.Stop();
        video.SetActive(false);
        endcription.Perfrom();
    }

    void Update()
    {
        if(videoPlayer.isPlaying)
        {
            timesBlinked++;
            // TODO: If gaze lost, restart video
        }
    }
}
