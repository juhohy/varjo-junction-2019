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
    public GameObject[] viveModels;
    public GameObject[] micModels;
    int timesBlinked;

    private void OnEnable()
    {
        timesBlinked = 0;
        video.SetActive(false);
        description.Perform();
    }

    public void ShowVideo()
    {
        video.SetActive(true);
        videoPlayer.Play();
        foreach(var go in viveModels) {
            go.SetActive(false);
        }
        foreach(var go in micModels) {
            go.SetActive(true);
        }
        Invoke("HideVideo", testTime);
    }

    public void HideVideo()
    {
        videoPlayer.Stop();
        video.SetActive(false);
        foreach (var go in viveModels)
        {
            go.SetActive(true);
        }
        foreach (var go in micModels)
        {
            go.SetActive(false);
        }
        if (timesBlinked <= 3)
        {
            endcriptionWin.Perform();
        } else
        {
            endcriptionLose.Perform();
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
            videoPlayer.time = 0.0;
        }
    }
}
