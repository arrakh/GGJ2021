using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class TimerStartGame : MonoBehaviourPun
{
    private GameObject canvasTimerStart;
    private TextMeshProUGUI txtTimerStart;
    private float timeStartRace = 5.0f;

    private void Awake()
    {
        canvasTimerStart = FindMeGameManager.instance.CanvasTimerStart;
        txtTimerStart = FindMeGameManager.instance.txtTimerStart;
    }

    private void Start()
    {
        canvasTimerStart.SetActive(false);
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeStartRace >= 0.0f)
            {                
                timeStartRace -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.All, timeStartRace);
            }
            else if (timeStartRace < 0.0f)
            {
                ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() { { "WaitingPlayer", "false" } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
                photonView.RPC("StartGame", RpcTarget.All);
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
            canvasTimerStart.SetActive(true);
        }
        else
        {
            txtTimerStart.text = "";
            canvasTimerStart.SetActive(false);
        }
    }

    [PunRPC]
    public void StartGame()
    {
        GetComponent<MovementController>().controlEnable = true;
    }
}
