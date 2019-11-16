using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class StareTest : MonoBehaviour
{
    public TalkAndDo description;
    public TalkAndDo endcriptionWin;
    public TalkAndDo endcriptionLose;
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
        Invoke("HideVideo", testTime);
    }

    public void HideVideo()
    {
        videoPlayer.Stop();
        video.SetActive(false);
        if (timesBlinked <= 3)
        {
            endcriptionWin.Perfrom();
        } else
        {
            endcriptionLose.Perfrom();
        }
    }

    void Update()
    {
        bool isTracking = GazeManager.instance.BothEyesTracking();
        if(videoPlayer.isPlaying && !isTracking)
        {
            videoPlayer.Stop();
            timesBlinked++;
        } else if(!videoPlayer.isPlaying && isTracking)
        {
            videoPlayer.Play();
            videoPlayer.time = 7.0;
        }
    }
}
