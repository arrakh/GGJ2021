using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject PanelEntry;

    public GameObject PanelInputName;
    public TMP_InputField infieldNickName;

    public GameObject PanelJoinOrCreateRoom;

    public GameObject PanelRoom;
    public TextMeshProUGUI txtRoomName;
    public Transform transListPlayer;
    public GameObject prefabUiPanelUserPlayer;
    private int NoIndexPlayer;
    public List<UserPlayer> listUserPlayer;

    [System.Serializable]
    public class UserPlayer
    {
        public int NoPanel;
        public GameObject PanelPlayer;
        public TextMeshProUGUI txtPlayerName;
        public TextMeshProUGUI txtPlayerStatus;
        public GameObject PanelButtonReady;
    }

    public GameObject PanelListRoom;
    public Transform transListRoom;
    public GameObject prefabPanelRoom;


    public GameObject PanelLoading;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PanelEntry.SetActive(true);
        PanelInputName.SetActive(false);
        PanelJoinOrCreateRoom.SetActive(false);
        PanelLoading.SetActive(false);
    }

    #region Public Method

    public void ButtonStartGame()
    {
        infieldNickName.text = "";
        PanelEntry.SetActive(false);
        PanelInputName.SetActive(true);
    }

    public void ButtonQuitApplication()
    {
        Application.Quit();
    }

    public void ButtonConfirmName()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void ButtonBackToEntryPanel()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            PanelEntry.SetActive(true);
            PanelJoinOrCreateRoom.SetActive(false);
        }
    }

    public void ButtonCreateRoom()
    {
        PanelJoinOrCreateRoom.SetActive(false);
        PanelLoading.SetActive(true);
        CreateAndJoinRoom();
    }

    public void ButtonJoinOrCreateRoom()
    {
        PanelJoinOrCreateRoom.SetActive(false);
        PanelLoading.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    #endregion

    #region Photon Callback

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " Connect to photon Master");
        PanelInputName.SetActive(false);
        PanelJoinOrCreateRoom.SetActive(true);

    }

    public override void OnConnected()
    {
        Debug.Log("Connect To Internet");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(PhotonNetwork.NickName + " Disconnect");
        PanelEntry.SetActive(true);
        PanelJoinOrCreateRoom.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log(message);
        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    #endregion

    void CreateAndJoinRoom()
    {
        string RoomName = "Room " + PhotonNetwork.NickName + " " + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;

        string[] roomProperties = {"WaitingPlayer"};

        ExitGames.Client.Photon.Hashtable waitingPlayer = new ExitGames.Client.Photon.Hashtable() { { "WaitingPlayer", "true" } };
        roomOptions.CustomRoomPropertiesForLobby = roomProperties;
        roomOptions.CustomRoomProperties = waitingPlayer;

        PhotonNetwork.CreateRoom(RoomName, roomOptions);
    }
}
