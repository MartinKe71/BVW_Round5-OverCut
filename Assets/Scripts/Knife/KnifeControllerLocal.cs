using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Photon;
using Photon.Pun;
using Photon.Realtime;

public class KnifeControllerLocal : MonoBehaviour
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
    public Vector2 _yLimit = new Vector2(-3f, 15f);

    bool _canDown = true;
    KnifeCutFood _knifeCutFood;
    Vector3 _initialMouse;
    Transform _startTransform;
    Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _startTransform = transform;
        _initialMouse = Input.mousePosition;
        _rb = GetComponent<Rigidbody>();
        _knifeCutFood = GetComponent<KnifeCutFood>();
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

        //photonView.RPC("MoveKnifeWithMouseSpeed", RpcTarget.AllViaServer);
        MoveKnifeWithMouseSpeed();
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

    private void MoveKnifeWithMouseSpeed()
    {
        float hOffset = _horizontalMovementDelta * Input.GetAxis("Mouse X");
        float vOffset = _verticalMovementDelta * Mathf.Clamp(Input.GetAxis("Mouse Y"), -0.7f, 0.7f);

        Vector3 offset = new Vector3(hOffset, vOffset, 0);
        if (!_canDown && vOffset < 0) return;
        if (IsInBound(transform.position + offset))
        {
            transform.Translate(offset, Space.World);
        }
    }

    public bool IsInBound(Vector3 Pos)
    {
        return (Pos.x > _horizontalLimit.x && Pos.x < _horizontalLimit.y
            && Pos.z > _verticalLimit.x && Pos.z < _verticalLimit.y
            && Pos.y > _yLimit.x && Pos.y < _yLimit.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Board"))
        {
            _canDown = false;
            SoundManager.instance.PlayCutFoodClip();

            if (_knifeCutFood._cuttingFood)
            {
                //_knifeCutFood._curFood.OnCut(transform.position);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Board"))
        {
            _canDown = true;
        }
    }
}
