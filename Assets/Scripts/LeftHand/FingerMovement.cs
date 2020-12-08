using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerMovement : MonoBehaviour
{
    float curTime = 0.5f;
    bool startFloat = false;
    public Vector3 initialPosition;

    private void OnEnable()
    {
        startFloat = true;
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (startFloat)
        {
            float yOffset = Mathf.Sin(2 * Time.time) / curTime;
            transform.position = initialPosition + new Vector3(0, yOffset, 0);
            curTime += Time.deltaTime;
        }
    }
}
