using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPun
{
    public GameObject PlayerCamera;
    public GameObject PlayerLight;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Player";
        if (photonView.IsMine)
        {
            transform.GetComponent<MovementController>().enabled = true;
            PlayerCamera.GetComponent<Camera>().enabled = true;
            gameObject.name = PhotonNetwork.NickName;
            PlayerLight.SetActive(true);
        }
        else
        {
            transform.GetComponent<MovementController>().enabled = false;
            PlayerCamera.GetComponent<Camera>().enabled = false;
            PlayerLight.SetActive(false);
        }
    }
}
