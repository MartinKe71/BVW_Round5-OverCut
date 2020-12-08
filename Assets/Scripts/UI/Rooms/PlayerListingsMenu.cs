using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListingsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private PlayerListing _playerListing;
    [SerializeField]
    private Text _readyUpText;

    [Header("Start Game Button")]
    public GameObject StartGameButton;

    private List<PlayerListing> _listings = new List<PlayerListing>();
    private RoomsCanvases _roomsCanvases;
    private bool _ready = false;

    public override void OnEnable()
    {
        base.OnEnable();
        SetReadyUp(false);
        GetCurrentRoomPlayers();
        StartGameButton.SetActive(false);   
    }

    public override void OnDisable()
    {
        base.OnDisable();
        for (int i = 0; i < _listings.Count; i++)
            Destroy(_listings[i].gameObject);

        _listings.Clear();
    }

    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    private void SetReadyUp(bool state)
    {
        _ready = state;
        if(_ready){
            _readyUpText.text = "Ready!";
        }
        else{
            _readyUpText.text = "Ready?";
        }
    }

/*
    public override void OnLeftRoom()
    {
        _content.DestroyChildren();
    }
*/

    private void GetCurrentRoomPlayers()
    {
        if(!PhotonNetwork.IsConnected) return;
        if(PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null) return;
        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playerInfo.Value); // getting player from the key
        }
        
    }

    private void AddPlayerListing(Player player)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            _listings[index].SetPlayerInfo(player);
        }
        else
        {
            PlayerListing listing = Instantiate(_playerListing, _content);
            if (listing != null)
            {
                listing.SetPlayerInfo(player);
                _listings.Add(listing);
            }
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _roomsCanvases.CurrentRoomCanvas.LeaveRoomMenu.OnClick_LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = _listings.FindIndex( x => x.Player == otherPlayer);
        if( index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    public void OnClick_StartGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
/*
            for(int i = 0; i < _listings.Count; i ++)
            {
                if(_listings[i].Player != PhotonNetwork.LocalPlayer)
                {
                    if(!_listings[i].Ready) return;
                }
            }
*/
            PhotonNetwork.CurrentRoom.IsOpen = false; // after start game, the room will not allow anyone to join
            PhotonNetwork.CurrentRoom.IsVisible = false; // after start game, the room will no longer shown in the room list
            PhotonNetwork.LoadLevel(1);
        }
    }

    public void OnClick_ReadyUp()
    {
        /*
        if(!PhotonNetwork.IsMasterClient)
        {
            SetReadyUp(!_ready);
            base.photonView.RPC("RPC_ChangeReadyState", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer,  _ready);
        }
        */
        SetReadyUp(!_ready);
        base.photonView.RPC("RPC_ChangeReadyState", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer,  _ready);

        if(PhotonNetwork.IsMasterClient){
            StartGameButton.SetActive(true);
//           base.photonView.RPC("RPC_SetStartButtonDeActive", RpcTarget.MasterClient);
        }    

        bool AllReady = true;       
        for(int i = 0; i < _listings.Count; i ++)
        {
            if(_listings[i].Player != PhotonNetwork.LocalPlayer)
            {
                if(!_listings[i].Ready) {
                    AllReady = false;
//                    Debug.LogError( i + " is not ready yet");
                }
            }
        }
        if(AllReady){
            base.photonView.RPC("RPC_SetStartButtonActive", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void RPC_ChangeReadyState(Player player, bool ready)
    {
        int index = _listings.FindIndex( x => x.Player == player);
        if( index != -1)
        {
            _listings[index].Ready = ready;
        }
    }

    [PunRPC]
    private void RPC_SetStartButtonActive()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            StartGameButton.GetComponent<Button>().interactable = true;
        }
    }

    [PunRPC]
    private void RPC_SetStartButtonDeActive()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            StartGameButton.GetComponent<Button>().interactable = false;
        }
    }
}
