using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField]GameObject FpsCamera;
    [SerializeField]GameObject Light;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            transform.GetComponent<MovementController>().enabled = true;
            FpsCamera.GetComponent<Camera>().enabled = true;
            Light.SetActive(true);
        }
        else
        {
            transform.GetComponent<MovementController>().enabled = false;
            FpsCamera.GetComponent<Camera>().enabled = false;
            Light.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
