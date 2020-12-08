using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestConnect : MonoBehaviourPunCallbacks
{

    // Start is called before the first frame update
    void Start()
    {
        print("Connecting to server.");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us"; // set fix region
        //PhotonNetwork.ConnectUsingSettings();
    }

     public override void OnConnectedToMaster()
     {
         print("Connected to Server");
         print("My nickname is " + PhotonNetwork.LocalPlayer.NickName);
         if (PhotonNetwork.InLobby)
         {
             PhotonNetwork.JoinLobby();
         }

         PhotonNetwork.JoinLobby();
     }

     public override void OnDisconnected(DisconnectCause cause)
     {
         print("Disconnected from server for reason " + cause.ToString());
     }

     public override void OnJoinedLobby()
     {
         print("Joined lobby");
     }

}
