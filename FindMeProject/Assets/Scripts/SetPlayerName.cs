using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SetPlayerName : MonoBehaviour
{
    public Button btnConfirmation;

    private void Start()
    {
        btnConfirmation.interactable = false;
    }

    public void CheckPlayerName(string playername)
    {
        if (string.IsNullOrEmpty(playername))
        {
            btnConfirmation.interactable = false;
            return;
        }

        btnConfirmation.interactable = true;

        PhotonNetwork.NickName = playername;
    }
}
