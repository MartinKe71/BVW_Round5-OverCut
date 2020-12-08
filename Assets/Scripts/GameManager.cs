using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Video;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int PieceCounter;
    public Text PieceCounterText;
    public GameObject UICanvas;

    void Start()
    {
        UICanvas.SetActive(false);
        PieceCounter = 0;

        Cursor.visible = false;

        //VideoPlayer videoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();
        //videoPlayer.Play();
        //videoPlayer.loopPointReached += SetupGame;

        SetupGame();

        // Player[] players = PhotonNetwork.PlayerList;
        // Left hand should be master.
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    // Create Hand
        //    Vector3 position_hand = new Vector3(-5.31f, -2.44f, -0.11f);
        //    Quaternion rotation_hand = Quaternion.Euler(0.0f, -151.8f, 0.0f);
        //    PhotonNetwork.Instantiate("LeftHand", position_hand, rotation_hand, 0);
        //    //photonView.RPC("RPCInitializeHand", RpcTarget.All);

        //    // Create Carrot
        //    //Vector3 position_food = new Vector3(-5.62f, -3.41f, 4.43f);
        //    //Quaternion rotation_food = Quaternion.Euler(0.0f, 0.0f, -90.0f);
        //    //PhotonNetwork.InstantiateRoomObject("Carrot", position_food, rotation_food, 0);
        //    ScoreManager.instance.SetTheFood();

        //    // Create Plate
        //    Vector3 position_plate = new Vector3(0.5f, -4.41f, 17.2f);
        //    Quaternion rotation_plate = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        //    PhotonNetwork.InstantiateRoomObject("Plate", position_plate, rotation_plate, 0);
        //}

        //else
        //{
        //    Vector3 position_knife = new Vector3(7.43f, 1f, 3.3f);
        //    Quaternion rotation_knife = Quaternion.Euler(0.0f, -90f, 0.0f);
        //    PhotonNetwork.Instantiate("Knife", position_knife, rotation_knife, 0);
        //}
    }

    //public void SetupGame(UnityEngine.Video.VideoPlayer vp)
    public void SetupGame()
    {
        UICanvas.SetActive(true);
        int guest = 0;
        //vp.enabled = false;
        if (PhotonNetwork.IsMasterClient)
        {
            // Create Hand
            Vector3 position_hand = new Vector3(-5.31f, -2.44f, -0.11f);
            Quaternion rotation_hand = Quaternion.Euler(0.0f, -151.8f, 0.0f);
            PhotonNetwork.Instantiate("LeftHand", position_hand, rotation_hand, 0);
            //photonView.RPC("RPCInitializeHand", RpcTarget.All);

            // Create Plate
            //Vector3 position_plate = new Vector3(0.5f, -4.41f, 17.2f);
            //Quaternion rotation_plate = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            //PhotonNetwork.InstantiateRoomObject("Plate", position_plate, rotation_plate, 0);

            // Create Carrot
            //Vector3 position_food = new Vector3(-5.62f, -3.41f, 4.43f);
            //Quaternion rotation_food = Quaternion.Euler(0.0f, 0.0f, -90.0f);
            //PhotonNetwork.InstantiateRoomObject("Carrot", position_food, rotation_food, 0);
            ScoreManager.instance.SetTheFood();
        }

        else
        {
            Vector3 position_knife = new Vector3(7.43f, 3f, 3.3f);
            Quaternion rotation_knife = Quaternion.Euler(0.0f, -90f, 0.0f);
            PhotonNetwork.Instantiate("Knife", position_knife, rotation_knife, 0);
        }
    }

    public void AddToPieceCounter()
    {
        PieceCounter += 1;
        PieceCounterText.text = PieceCounter.ToString("0");
    }
    public void ResetPieceCounter()
    {
        PieceCounter = 0;
    }

    public void RecoverCursor()
    {
        Cursor.visible = true;
    }

    [PunRPC]
    private void RPCInitializeHand()
    {
        Vector3 position = new Vector3(-5.31f, -2.44f, -0.11f);
        Quaternion rotation = Quaternion.Euler(0.0f, -151.8f, 0.0f);

        PhotonNetwork.Instantiate("LeftHand", position, rotation, 0);
    }


    #region Photon Callbacks


    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>

    /*
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }


    #endregion


    #region Public Methods


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    */

    #endregion
}

