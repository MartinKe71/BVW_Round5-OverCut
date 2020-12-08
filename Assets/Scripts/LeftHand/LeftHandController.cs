using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

using Photon;
using Photon.Pun;
using Photon.Realtime;

public class LeftHandController : MonoBehaviourPunCallbacks
//public class LeftHandController : MonoBehaviour
{
    [Header("Mouse Delta Speed")]
    public float _verticalMovementDelta;
    public float _horizontalMovementDelta;
    public float _yMovementDelta = 0.1f;

    [Header("Position Limit")]
    public Vector2 _verticalLimit = new Vector2(-9, 12f);
    public Vector2 _horizontalLimit = new Vector2(-16f, 16f);

    // Put it in the controller for the moment, will move to lefthandstatusmanger later
    [Header("Status")]
    public bool _handInvincible = false;
    public float _invincibleTime = 5f;
    public Vector3 _emptyRotation = new Vector3(0.0f, -151.8f, 0.0f);
    public Vector3 _holdRotation = new Vector3(0.0f, -90.0f, 0.0f);
    public Vector3 _emptyToHoldOffset = new Vector3(-2.0f, 0.0f, 5f);

    [Header("Meshes")]
    public int handIdx = 0;
    public List<Mesh> emptyHands;
    public List<Mesh> holdHands;

    private GameObject _gameManager;
    private GameObject Hand;

    GameObject _food;
    bool _canGrab = true;
    bool _grabbing = false;
    bool _canPress = false;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.SendRate = 40;
        PhotonNetwork.SerializationRate = 15;
        _food = null;
        _gameManager = GameObject.Find("GameManager");
        Hand = GameObject.Find("LeftHand(Clone)");
    }

    // Update is called once per frame
    void Update()
    {
        GrabFood();
        PressBell();
    }

    private void FixedUpdate()
    {
        photonView.RPC("MoveHandWithMouseSpeed", RpcTarget.AllViaServer);
        //MoveHandWithMouseSpeed();
    }

    [PunRPC]
    private void MoveHandWithMouseSpeed()
    {
        if (photonView.IsMine)
        {
            float hOffset = _horizontalMovementDelta * Input.GetAxis("Mouse X");
            float vOffset = _verticalMovementDelta * Input.GetAxis("Mouse Y");
            float yOffset = _yMovementDelta * Input.mouseScrollDelta.y;

            Vector3 offset = new Vector3(hOffset, yOffset, vOffset);
            
            //if (_food != null && _grabbing)
            //{
            //    float distance = transform.position.x - _food.transform.position.x;
            //    float multiplier = 1 - distance / _food.GetComponent<TheFood>()._meshRenders[0].material.GetFloat("_FillMax");
            //    offset += new Vector3(Mathf.Sin(Time.time), 0, Mathf.Cos(Time.time)) * Time.deltaTime * multiplier;
            //}

            if (IsInBound(transform.position + offset))
            {
                transform.Translate(offset, Space.World);
                if (_food != null && _grabbing)
                {
                    _food.transform.Translate(offset, Space.World);
                }
            }
        }
    }

    public bool IsInBound(Vector3 Pos)
    {
        return (Pos.x > _horizontalLimit.x && Pos.x < _horizontalLimit.y
            && Pos.z > _verticalLimit.x && Pos.z < _verticalLimit.y);
    }

    private void GrabFood()
    {
        if (_canGrab && !_grabbing && Input.GetKeyDown(KeyCode.Space) && _food != null)
        {
            _grabbing = true;
            _food.GetComponent<TheFood>()._grabbed = true;
            //GetComponent<MeshFilter>().mesh = holdHands[handIdx];
            photonView.RPC("ChangeMeshToHold", RpcTarget.AllViaServer);
        }

        if (_grabbing && Input.GetKeyUp(KeyCode.Space))
        {
            _grabbing = false;
            _food.GetComponent<TheFood>()._grabbed = false;
            photonView.RPC("ChangeMeshToEmpty", RpcTarget.AllViaServer);
        }
    }

    private void PressBell()
    {
        if (_canPress && Input.GetKeyDown(KeyCode.Space))
        {
            // Reset _canPress
            //_canPress = false;
            // Call reset function in scoremanager
            ScoreManager.instance.ResetTheFood();
        }    
    }

    [PunRPC]
    private void ChangeMeshToHold()
    {
        transform.rotation = Quaternion.Euler(_holdRotation);
        transform.position += _emptyToHoldOffset;
        GetComponent<MeshFilter>().mesh = holdHands[handIdx];
    }

    [PunRPC]
    private void ChangeMeshToEmpty()
    {
        transform.rotation = Quaternion.Euler(_emptyRotation);
        transform.position -= _emptyToHoldOffset;
        GetComponent<MeshFilter>().mesh = emptyHands[handIdx];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            _canGrab = true;
            _food = other.gameObject;
        }
        else if (other.CompareTag("Bell"))
        {
            _canPress = true;
        }
        //Debug.Log(other.tag);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            _canGrab = false;
            _food.GetComponent<TheFood>()._grabbed = false;
            _grabbing = false;
            photonView.RPC("ChangeMeshToEmpty", RpcTarget.AllViaServer);
            //_food.GetComponent<PhotonTransformView>().enabled = false;
            // _food.transform.parent = null;
            _food = null;
        }
        else if (other.CompareTag("Bell"))
        {
            _canPress = false;
        }
    }

    public void OnCut()
    {
        StartCoroutine(HandRecover());
    }

    public void ChangeHandMesh()
    {
        if (handIdx < holdHands.Count)
        {
            photonView.RPC("CutFinger", RpcTarget.AllViaServer);
        }
    }

    IEnumerator HandRecover()
    {
        _handInvincible = true;
        float curTime = 0f;
        while (curTime < _invincibleTime)
        {
            transform.Translate(new Vector3(Mathf.PerlinNoise(0,Time.time * 20), 0, Mathf.PerlinNoise(0, Time.time*30)) * 3 *  Time.deltaTime);
            yield return new WaitForEndOfFrame();
            curTime += Time.deltaTime;
        }
        _handInvincible = false;
    }

    [PunRPC]
    void CutFinger()
    {

        if (GetComponent<MeshFilter>().mesh == holdHands[handIdx])
        {            
            GetComponent<MeshFilter>().mesh = holdHands[handIdx + 1];
            _gameManager.GetComponent<BloodController>().GetComponent<PhotonView>().RPC("SetHandUI", RpcTarget.AllViaServer, (handIdx+1));
            handIdx++;
        }
        else if (GetComponent<MeshFilter>().mesh == emptyHands[handIdx])
        {            
            GetComponent<MeshFilter>().mesh = emptyHands[handIdx + 1];
            _gameManager.GetComponent<BloodController>().GetComponent<PhotonView>().RPC("SetHandUI", RpcTarget.AllViaServer, (handIdx+1));
            handIdx++;
        }

        ScoreManager.instance._showFinger._num = handIdx;

        if (handIdx >= 5)
        {
            ScoreManager.instance.GetComponent<PhotonView>().RPC("PlayLose", RpcTarget.AllViaServer);
        }

        _gameManager.GetComponent<BloodController>().GetComponent<PhotonView>().RPC("OpenBloodStainUI", RpcTarget.AllViaServer);
        
        Hand = GameObject.Find("LeftHand(Clone)");
        Vector3 EffectPos =  Hand.transform.position + new Vector3(1.0f, 0.0f, 1.0f);
        EffectPos = EffectPos - new Vector3(0.5f * (handIdx-1), 0.0f, 0.0f) ;
        PhotonNetwork.InstantiateRoomObject("Effect_SplashBlood", EffectPos, Hand.transform.rotation, 0);
    }
}
