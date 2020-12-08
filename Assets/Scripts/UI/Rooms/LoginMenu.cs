using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LoginMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _playerName;

    [Header("Login Canvas")]
    public GameObject LoginCanvas;
    public Text EnterNameMessage;
    [Header("Create Or Join Room Canvas")]
    public GameObject CreateOrJoinRoomCanvas;

    private RoomsCanvases _roomCanvases;

    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomCanvases = canvases;
    }

    public void OnClick_Login()
    {
 //       if(!PhotonNetwork.IsConnected) return;

        string playerName = _playerName.text;
        if (!playerName.Equals(""))
        {
            /*
            PhotonNetwork.LocalPlayer.NickName = playerName;
 //           PhotonNetwork.ConnectUsingSettings();
            Debug.LogError("Player's Nickname is " + PhotonNetwork.LocalPlayer.NickName);
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us"; // set fix region
            PhotonNetwork.ConnectUsingSettings();
            */
            StartCoroutine(SayHiAndLogin());
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
            EnterNameMessage.text = "Please Enter Your Name!";
        }
    }

    // Call Backs
    public override void OnConnectedToMaster()
    {
        print("Connected to Server");
        print("My nickname is " + PhotonNetwork.LocalPlayer.NickName);
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        PhotonNetwork.JoinLobby();
        
        LoginCanvas.SetActive(false);
        CreateOrJoinRoomCanvas.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from server for reason " + cause.ToString());
    }

    public override void OnJoinedLobby()
    {
        print("Joined lobby");
    }

    private IEnumerator SayHiAndLogin()
    {
        EnterNameMessage.text = "Hi, " + _playerName.text + "!";
        yield return new WaitForSeconds (1.5f);
        PhotonNetwork.LocalPlayer.NickName = _playerName.text;
        Debug.LogError("Player's Nickname is " + PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "usw"; // set fix region
        PhotonNetwork.ConnectUsingSettings();
    }
}
