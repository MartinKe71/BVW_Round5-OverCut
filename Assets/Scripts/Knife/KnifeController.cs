using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Photon;
using Photon.Pun;
using Photon.Realtime;

public class KnifeController : MonoBehaviourPunCallbacks
{

    [Header("Mouse Position")]
    public float _verticalMovement;
    public float _horizontalMovement;
    public float _horizontalMul = 1f;
    public float _verticalMul = 1f;

    [Header("Mouse Delta Speed")]
    public float _verticalMovementDelta;
    public float _horizontalMovementDelta;

    [Header("Position Limit")]
    public Vector2 _verticalLimit = new Vector2(-9, 12f);
    public Vector2 _horizontalLimit = new Vector2(-16f, 16f);
    public Vector2 _yLimit = new Vector2(-4f, 15f);


    bool _canDown = true;
    KnifeCutFood _knifeCutFood;
    Vector3 _initialMouse;
    Transform _startTransform;
    Rigidbody _rb;

    GameObject _hand;
    GameObject _soundManager;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.SendRate = 40;
        PhotonNetwork.SerializationRate = 15;
        _startTransform = transform;
        _initialMouse = Input.mousePosition;
        _rb = GetComponent<Rigidbody>();
        _knifeCutFood = GetComponent<KnifeCutFood>();
        _soundManager = GameObject.Find("SoundManager");
    }

    // Update is called once per frame
    private void Update()
    {
        // MoveKnifeWithMouse();
        // MoveKnifeWithMouseSpeed();
    }

    void FixedUpdate()
    {
        // MoveKnifeWithMouse();

        photonView.RPC("MoveKnifeWithMouseSpeed", RpcTarget.AllViaServer);
        //MoveKnifeWithMouseSpeed();
    }

    private void MoveKnifeWithMouse()
    {
        Vector3 deltaMouse = Input.mousePosition - _initialMouse;
        float hOffset = deltaMouse.x / Screen.width * _horizontalMul * Time.deltaTime;
        float vOffset = deltaMouse.y / Screen.height * _verticalMul * Time.deltaTime;

        Vector3 offset = new Vector3(0, vOffset, -hOffset);
        if (!_canDown && vOffset < 0) return;
        transform.Translate(offset);
    }

    [PunRPC]
    private void MoveKnifeWithMouseSpeed()
    {
        if (photonView.IsMine)
        {
            float hOffset = _horizontalMovementDelta * Input.GetAxis("Mouse X");
            float vOffset = _verticalMovementDelta * Mathf.Clamp(Input.GetAxis("Mouse Y"), -0.7f, 0.7f);

            Vector3 offset = new Vector3(hOffset, vOffset, 0);
            //if (!_canDown && vOffset < 0) return;
            if (IsInBound(transform.position + offset)) transform.Translate(offset, Space.World);
            // else Debug.Log("Going to out of Bound");
        }
    }

    public bool IsInBound(Vector3 Pos)
    {
        return (Pos.x > _horizontalLimit.x && Pos.x < _horizontalLimit.y
            && Pos.y > _yLimit.x && Pos.y < _yLimit.y
            && Pos.z > _verticalLimit.x && Pos.z < _verticalLimit.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.transform.CompareTag("Board"))
        {
            photonView.RPC("StartCut", RpcTarget.AllViaServer);
            // Check if we are going to cut the hand
            if (_knifeCutFood._cuttingHand)
            {
                _hand = GameObject.Find("LeftHand(Clone)");
                if (photonView.IsMine)
                {
                    _hand.GetComponent<LeftHandController>().ChangeHandMesh();
                }
                _hand.GetComponent<LeftHandController>().OnCut();
                SoundManager.instance.PlayCutFingerClip();
            }
        }
    }

    [PunRPC]
    private void StartCut()
    {
        _canDown = false;
        if (_knifeCutFood._cuttingFood)
        {
            if (photonView.IsMine)
            {
                _knifeCutFood.CutFoodDone();                
                _knifeCutFood._curFood.GetComponent<PhotonView>().RPC("OnCut", RpcTarget.MasterClient, transform.position);
            }

            _hand = GameObject.Find("LeftHand(Clone)");
            photonView.RPC("MoveFoodToHand", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void MoveFoodToHand()
    {
        if (_hand != null && _knifeCutFood._curFood != null)
        {
            TheFood food = _knifeCutFood._curFood;
            Material material = _knifeCutFood._curFood._meshRenders[0].material;
            float maxDist = material.GetFloat("_FillMax") - material.GetFloat("_FillMin");
            float curDist = material.GetFloat("_FillMax") - (_hand.transform.position.x - food.transform.position.x);
            float offset = Mathf.Clamp((curDist / maxDist - 0.3f), 0, 1) * ScoreManager.instance._curFoodStat.moveDistance;
            food.transform.position += new Vector3(-offset, 0, 0);
        }
        else
        {
            Debug.LogError("No hand found!");
        }
    }

    //[PunRPC]
    //public void FoodCut()
    //{
    //    _knifeCutFood._curFood.OnCut(transform.position);
    //}


    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Board"))
        {
            _canDown = true;
        }
    }
}
