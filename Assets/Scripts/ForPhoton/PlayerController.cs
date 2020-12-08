using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class PlayerController : MonoBehaviourPun
    {
        public float speed;

        private Rigidbody rb;
        private int count;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            count = 0;
        }

        void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            rb.AddForce(movement * speed);
        }
    }
}
