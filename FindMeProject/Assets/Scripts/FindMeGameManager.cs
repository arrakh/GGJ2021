using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class FindMeGameManager : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    public List<SpawnPoint> listSpawnPoint;

    [System.Serializable]
    public class SpawnPoint
    {
        public int NoListSpawnPoint;
        public Transform transSpawnPoint;
    }

    public GameObject CanvasTimerStart;
    public TextMeshProUGUI txtTimerStart;

    public static FindMeGameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        CanvasTimerStart.SetActive(false);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SetSpawnPoint();
            }
            else
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("WaitingPlayer"))
                {
                    object WaitMaster;
                    if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("WaitingPlayer", out WaitMaster))
                    {
                        Debug.Log(WaitMaster.ToString());
                        if (WaitMaster.ToString() != "true")
                        {
                            int randompoint = Random.Range(0, listSpawnPoint.Count - 1);
                            GameObject player = PhotonNetwork.Instantiate(PlayerPrefab.name, listSpawnPoint[randompoint].transSpawnPoint.position, Quaternion.identity);
                            if (player.GetPhotonView().IsMine)
                            {
                                player.GetComponent<MovementController>().controlEnable = true;
                            }
                        }
                    }
                }
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

    void SetSpawnPoint()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int randomPoint = Random.Range(0, listSpawnPoint.Count - 1);
            if (player.IsMasterClient)
            {
                PhotonNetwork.Instantiate(PlayerPrefab.name, listSpawnPoint[randomPoint].transSpawnPoint.position, Quaternion.identity);
            }
            else
            {
                photonView.RPC("SetTargetSpawn", player, listSpawnPoint[randomPoint].NoListSpawnPoint);
            }
            listSpawnPoint.Remove(listSpawnPoint[randomPoint]);
        }
    }

    [PunRPC]
    public void SetTargetSpawn(int Target)
    {
        PhotonNetwork.Instantiate(PlayerPrefab.name, listSpawnPoint[Target].transSpawnPoint.position, Quaternion.identity);
    }
}
