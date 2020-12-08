using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Video;

public class UITutorialManager : MonoBehaviourPunCallbacks
{
    public GameObject Background;
    public GameObject StartButton;
    public GameObject ReplayButton;
    public GameObject SkipButton;

    void Start()
    {
        Background.SetActive(false);
        VideoPlayer videoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();
        videoPlayer.Play();
        videoPlayer.loopPointReached += EndReached;

        if(PhotonNetwork.IsMasterClient)
        {
            SkipButton.SetActive(true);
        }
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        StartGame();
    }

    public void StartGame()
    {
        Background.SetActive(true);
        if(PhotonNetwork.IsMasterClient)
        {
            StartButton.SetActive(true);
            ReplayButton.SetActive(true);
        }
    }

    public void OnClickStartButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(4);
        }

    }
    public void OnClickReplayButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(3);
        }

    }
}
