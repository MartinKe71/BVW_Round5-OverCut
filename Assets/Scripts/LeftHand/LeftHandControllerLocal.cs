using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

using Photon;
using Photon.Pun;
using Photon.Realtime;

public class LeftHandControllerLocal : MonoBehaviour
{
    [Header("Mouse Delta Speed")]
    public float _verticalMovementDelta;
    public float _horizontalMovementDelta;
    public float _yMovementDelta = 0.1f;

    [Header("Position Limit")]
    public Vector2 _verticalLimit = new Vector2(-9, 12f);
    public Vector2 _horizontalLimit = new Vector2(-16f, 16f);

    GameObject _food;
    bool _canGrab = true;
    bool _grabbing = false;

    // Start is called before the first frame update
    void Start()
    {
        _food = null;
    }

    // Update is called once per frame
    void Update()
    {
        GrabFood();
    }

    private void FixedUpdate()
    {
        MoveHandWithMouseSpeed();
    }

    private void MoveHandWithMouseSpeed()
    {
        float hOffset = _horizontalMovementDelta * Input.GetAxis("Mouse X");
        float vOffset = _verticalMovementDelta * Input.GetAxis("Mouse Y");
        float yOffset = _yMovementDelta * Input.mouseScrollDelta.y;

        Vector3 offset = new Vector3(hOffset, yOffset, vOffset);
        if (IsInBound(transform.position + offset))
        {
            transform.Translate(offset, Space.World);
            if (_food != null && _grabbing) _food.transform.Translate(offset, Space.World);
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

           // _food.transform.parent = transform;
            //_food.GetComponent<PhotonTransformView>().enabled = true;
            _grabbing = true;
        }

        if (_grabbing && Input.GetKeyUp(KeyCode.Space))
        {
            //_food.GetComponent<PhotonTransformView>().enabled = false;
            // _food.transform.parent = null;
            _grabbing = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            _canGrab = true;
            _food = other.gameObject;
        }
        Debug.Log(other.tag);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            _canGrab = false;
            //_food.GetComponent<PhotonTransformView>().enabled = false;
            // _food.transform.parent = null;
            _food = null;
        }
    }

}
