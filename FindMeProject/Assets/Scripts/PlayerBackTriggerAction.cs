using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerBackTriggerAction : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<PhotonView>().IsMine)
        {
            other.GetComponent<PhotonView>().RPC("OtherTriggerActive",RpcTarget.AllBuffered,true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<PhotonView>().IsMine)
        {
            other.GetComponent<PhotonView>().RPC("OtherTriggerActive", RpcTarget.AllBuffered,false);
        }
    }

}
