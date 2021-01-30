using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ListRoomScore : MonoBehaviour
{
    [Header("Ui Reference")]
    public TextMeshProUGUI txtPlayerName;
    public TextMeshProUGUI txtPlayerScore;

    public void Initialize(int playerId, string PlayerName)
    {
        txtPlayerName.text = PlayerName;
        txtPlayerScore.text = "0";

        if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
        {
            ExitGames.Client.Photon.Hashtable intialProp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerGame.PLAYER_SCORE_POINT, 0 } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(intialProp);
        }
    }

    public void SetPlayerScore(int score)
    {
        txtPlayerScore.text = score.ToString();
    }
}
