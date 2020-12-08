using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

using Photon;
using Photon.Pun;
using Photon.Realtime;

public enum FoodType
{
    UnitedSteaks,
    Carrot,
    Potato,
    Onion,
    BabyCorn,
    Garlic
}

[Serializable]
public struct FoodStat
{
    public String prefabName;
    public int maxTime;
    public int pieceCount;
    public float minWidth;
    public float moveDistance;
    public Vector3 position;
    public Vector3 rotation;
    public bool isLast;
    //[ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
    //public Color surfaceColor;
    //[ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
    //public Color cutColor;
}

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager instance;

    public int PieceCounter = 0;
    public Text PieceCounterText;

    public List<FoodStat> _foodStats;
    public FoodStat _curFoodStat;
    public int _curFoodIdx = 0;

    public PlayableDirector _winTimeline;
    public PlayableDirector _loseTimeline;
    public ShowFinger _showFinger;

    GameObject curFood;

    private List<float> CutFoodLength = new List<float>();
    private float FoodLeftLength;
    private float curFoodLength = 0;
    private float shouldCutAvg;
    private int shouldCutPiece;
    private bool canReset = true;
    private bool canTimeUp = true;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTheFood()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_curFoodIdx >= 0 && _curFoodIdx < _foodStats.Count)
            {
                Debug.LogError("Setting the new food");
                _curFoodStat = _foodStats[_curFoodIdx];
                string _prefab = _curFoodStat.prefabName;
                Vector3 position_food = _curFoodStat.position;
                Quaternion rotation_food = Quaternion.Euler(_curFoodStat.rotation);
                curFood = PhotonNetwork.InstantiateRoomObject(_prefab, position_food, rotation_food, 0);
                photonView.RPC("SetTimer", RpcTarget.AllViaServer, _curFoodStat.maxTime);
                //photonView.RPC("ResetPieceCounter", RpcTarget.AllViaServer);
                //GetComponent<TimerController>().SetTimer(_curFoodStat.maxTime);
                ResetPieceCounter();
                _curFoodIdx++;
            }
            else
            {
                photonView.RPC("PlayWin", RpcTarget.AllViaServer);
                Debug.LogError("Food index out of bound: " + _curFoodIdx + ";" + _foodStats.Count);
            }
        }
    }

    public void ResetTheFood()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Master received reset message");
            if (_curFoodIdx > 0 && canReset)
            {
                // Destory old food
                curFood.GetComponent<TheFood>().SelfDestory();

                // Generate New Food
                _curFoodIdx--;
                _curFoodStat = _foodStats[_curFoodIdx];
                string _prefab = _curFoodStat.prefabName;
                Vector3 position_food = _curFoodStat.position;
                Quaternion rotation_food = Quaternion.Euler(_curFoodStat.rotation);
                curFood = PhotonNetwork.InstantiateRoomObject(_prefab, position_food, rotation_food, 0);
                ResetPieceCounter();
                _curFoodIdx++;
            }
        }
    }

    [PunRPC]
    public void PlayWin()
    {
        SoundManager.instance._tick.Pause();
        _winTimeline.Play();
        //GetComponent<RestartManager>().GetComponent<PhotonView>().RPC("CountFinalScore", RpcTarget.AllViaServer);
        GetComponent<RestartManager>().CountFinalScore();
        //GetComponent<RestartManager>().GetComponent<PhotonView>().RPC("SetFinalScoreUI", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void PlayLose()
    {
        SoundManager.instance._tick.Pause();
        _loseTimeline.Play();
    }

    public void AddToPieceCounter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PieceCounter += 1;
            int cutLeft = _curFoodStat.pieceCount - PieceCounter;
            if (cutLeft < 0) cutLeft = 0;
            photonView.RPC("SetCounterText", RpcTarget.AllViaServer, cutLeft);
            Debug.LogError("CanTimeUp is: " + canTimeUp);
            if (PieceCounter >= _curFoodStat.pieceCount && canTimeUp)
            {
                Debug.LogError("PieceCounter is: " + PieceCounter.ToString());
                Debug.LogError("target count is:" + _curFoodStat.pieceCount);
                canTimeUp = false;
                if (PhotonNetwork.IsMasterClient)
                {
                    TimeUp();
                }
                GetComponent<RestartManager>().GetComponent<PhotonView>().RPC("SetGradeComplete", RpcTarget.AllViaServer, (_curFoodIdx - 1));
            }                     
        }       
    }

    IEnumerator GenerateNewFood()
    {
        // Make sure there's no more food cutting for the current food
        curFood.GetComponent<BoxCollider>().enabled = false;
        Debug.LogError(photonView.ViewID);
        yield return new WaitForSeconds(5f);
        SetTheFood();
        ResetPieceCounter();
        canReset = true;
        canTimeUp = true;
    }    

    //[PunRPC]
    public void ResetPieceCounter()
    {
        PieceCounter = 0;
        photonView.RPC("SetCounterText", RpcTarget.AllViaServer, _curFoodStat.pieceCount);
        //Debug.LogError("Reset the Counter!" + _curFoodStat.pieceCount);
    }

    [PunRPC]
    public void SetCounterText(int piece)
    {
        PieceCounterText.text = piece.ToString("0");
    }

    public void TimeUp()
    {
        Debug.LogError("I'm in TimeUP!!! " + canTimeUp);
        canReset = false;
        curFood.GetComponent<BoxCollider>().enabled = false;
        Debug.LogError("CurFood is: " + curFood.name);
        Debug.LogError("CurFoodList length is: " + curFood.GetComponent<TheFood>().CutFoodList.Count);
        curFood.GetComponent<PhotonView>().RPC("MovePlateToPot", RpcTarget.AllViaServer);
        StartCoroutine(GenerateNewFood());        
    }
}
