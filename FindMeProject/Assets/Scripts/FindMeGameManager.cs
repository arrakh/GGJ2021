using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class FindMeGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject PlayerPrefab;
    public List<RespawnPoint> listRespawnPoint;

    [System.Serializable]
    public class RespawnPoint
    {
        public Transform transPoint;
        public bool isPointOccupation;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PlayerPrefab != null)
            {
                int randompoint = Random.Range(-10, 10);
                PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(randompoint, 0, randompoint), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " joined to + " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
}
