using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class TimerStartGame : MonoBehaviourPun
{
    private TextMeshProUGUI txtTimerStart;
    private float timeStartRace = 5.0f;

    private void Awake()
    {
        txtTimerStart = FindMeGameManager.instance.txtTimerStart;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeStartRace >= 0.0f)
            {                
                timeStartRace -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeStartRace);
            }
            else if (timeStartRace < 0.0f)
            {
                ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() { { "WaitingPlayer", "false" } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
                photonView.RPC("StartGame", RpcTarget.AllBuffered);
                PhotonNetwork.RemoveRPCs(photonView);
            }
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        Debug.Log(PhotonNetwork.NickName + " Set Time");
        if (time > 0.0f)
        {
            txtTimerStart.text = time.ToString("F1");
        }
        else
        {
            txtTimerStart.text = "";
        }
    }

    [PunRPC]
    public void StartGame()
    {
        FindMeGameManager.instance.PanelTimeStart.SetActive(false);
        GetComponent<MovementController>().controlEnable = true;
        this.enabled = false;
    }
}
