using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviourPunCallbacks
{
    public GameObject Description;
    public GameObject StartButton;
    public GameObject SkipButton;

    void Start()
    {
        Description.SetActive(false);

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
        Description.SetActive(true);
        if(PhotonNetwork.IsMasterClient)
        {
            StartButton.SetActive(true);
//            Vector3 position = new Vector3(136f, -356f, 0.0f);
//            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
//            PhotonNetwork.Instantiate("GameStartButton", position, rotation, 0);
//            StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait(){
        yield return new WaitForSeconds (8);
        PhotonNetwork.LoadLevel(2);
    }

    public void OnClickStartButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(2);
        }
    }
}
