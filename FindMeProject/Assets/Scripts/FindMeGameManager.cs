using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class FindMeGameManager : MonoBehaviourPunCallbacks
{
    public static FindMeGameManager instance = null;

    [Header("Player Spawn")]
    public GameObject PlayerPrefab;
    public List<SpawnPoint> listSpawnPoint;
    private List<SpawnPoint> tmpSpawnPoint;

    [System.Serializable]
    public class SpawnPoint
    {
        public int NoListSpawnPoint;
        public Transform transSpawnPoint;
    }

    [Header("Panel Timer Start")]
    public GameObject PanelTimeStart;
    public TextMeshProUGUI txtTimerStart;

    [Header("Room Score")]
    public TextMeshProUGUI txtPlayerName;
    public TextMeshProUGUI txtPlayerScore;
    public int PlayerScore = 0;
    public GameObject PanelListRoomScore;
    public GameObject PrefabPanelRoomListScore;
    public Transform transContentPanelRoomListScore;

    [Header("Action")]
    public GameObject ImageAction;

    private Dictionary<int, GameObject> playerListGameObject = new Dictionary<int, GameObject>();

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
        PanelTimeStart.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        tmpSpawnPoint = listSpawnPoint;
        if (PhotonNetwork.IsConnectedAndReady)
        {
            txtPlayerName.text = PhotonNetwork.LocalPlayer.NickName;
            txtPlayerScore.text = PlayerScore.ToString(); 

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("WaitingPlayer"))
            {
                object WaitMaster;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("WaitingPlayer", out WaitMaster))
                {
                    if (WaitMaster.ToString() == "true")
                    {
                        object ObjPlayerSpawnPoint;
                        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerGame.PLAYER_SPAWN_POINT, out ObjPlayerSpawnPoint))
                        {
                            Debug.Log((int)ObjPlayerSpawnPoint);

                            PhotonNetwork.Instantiate(PlayerPrefab.name, listSpawnPoint[(int)ObjPlayerSpawnPoint].transSpawnPoint.position, Quaternion.identity);
                        }
                        else
                        {
                            foreach (Player player in PhotonNetwork.PlayerList)
                            {
                                if (player.CustomProperties.TryGetValue(MultiplayerGame.PLAYER_SPAWN_POINT, out ObjPlayerSpawnPoint))
                                {
                                    tmpSpawnPoint.Remove(tmpSpawnPoint[(int)ObjPlayerSpawnPoint]);
                                }
                            }
                            int randomPoint = Random.Range(0, tmpSpawnPoint.Count - 1);
                            PhotonNetwork.Instantiate(PlayerPrefab.name, tmpSpawnPoint[randomPoint].transSpawnPoint.position, Quaternion.identity);
                            ExitGames.Client.Photon.Hashtable playerSpawnPoint = new ExitGames.Client.Photon.Hashtable() { { MultiplayerGame.PLAYER_SPAWN_POINT, randomPoint } };
                            PhotonNetwork.LocalPlayer.SetCustomProperties(playerSpawnPoint);
                        }
                    }
                    else
                    {
                        int randompoint = Random.Range(0, listSpawnPoint.Count - 1);
                        GameObject player = PhotonNetwork.Instantiate(PlayerPrefab.name, listSpawnPoint[randompoint].transSpawnPoint.position, Quaternion.identity);
                        player.GetComponent<TimerStartGame>().StartGame();
                    }
                }
            }

            if (playerListGameObject == null)
                playerListGameObject = new Dictionary<int, GameObject>();

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                Debug.Log(player.NickName);
                GameObject prefab = Instantiate(PrefabPanelRoomListScore, transContentPanelRoomListScore);
                prefab.GetComponent<ListRoomScore>().Initialize(player.ActorNumber, player.NickName);

                object score;
                if (player.CustomProperties.TryGetValue(MultiplayerGame.PLAYER_SCORE_POINT, out score))
                {
                    prefab.GetComponent<ListRoomScore>().SetPlayerScore((int)score);
                }

                if (!playerListGameObject.ContainsKey(player.ActorNumber))
                    playerListGameObject.Add(player.ActorNumber, prefab);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Photon Call Back Method

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " joined to + " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject prefab = Instantiate(PrefabPanelRoomListScore, transContentPanelRoomListScore);
        prefab.GetComponent<ListRoomScore>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListGameObject.Add(newPlayer.ActorNumber, prefab);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListGameObject[otherPlayer.ActorNumber].gameObject);
        playerListGameObject.Remove(otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        foreach (GameObject playerlistobject in playerListGameObject.Values)
        {
            Destroy(playerlistobject);
        }

        playerListGameObject.Clear();
        playerListGameObject = null;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject prefabPanel;
        if (playerListGameObject.TryGetValue(targetPlayer.ActorNumber, out prefabPanel))
        {
            object Score;
            if (changedProps.TryGetValue(MultiplayerGame.PLAYER_SCORE_POINT, out Score))
            {
                prefabPanel.GetComponent<ListRoomScore>().SetPlayerScore((int)Score);
            }
        }
    }

    #endregion

    #region Public Methond

    public void AddScore()
    {
        PlayerScore += 1;
        txtPlayerScore.text = PlayerScore.ToString();

        ExitGames.Client.Photon.Hashtable intialProp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerGame.PLAYER_SCORE_POINT, PlayerScore } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(intialProp);
    }

    public void ButtonRoomScore()
    {
        PanelListRoomScore.SetActive(!PanelListRoomScore.activeSelf);
    }

    #endregion
}
