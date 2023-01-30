using GameNetworking;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField _roomNameInputField = null;
    [SerializeField] private TextMeshProUGUI _errorText = null;
    [SerializeField] private TextMeshProUGUI _roomNameText = null;
    [SerializeField] private Transform _roomListContent = null;
    [SerializeField] private GameObject _roomListItemPrefab = null;
    [SerializeField] private Transform _playerListContent = null;
    [SerializeField] private GameObject _playerListItemPrefab = null;
    [SerializeField] private GameObject _startGameButton = null;

    private const string TITLE_MENU_NAME = "Title Menu", LOADING_MENU_NAME = "Loading Menu",
        ROOM_MENU_NAME = "Room Menu", ERROR_MENU_NAME = "Error Menu";

    public static Launcher Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu(TITLE_MENU_NAME);
        Debug.Log("Joined Lobby");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu(ROOM_MENU_NAME);
        _roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        foreach (Transform child in _playerListContent)
        {
            Destroy(child.gameObject);
        }

        Player[] players = PhotonNetwork.PlayerList;

        for (int index = 0; index < players.Count(); index++)
        {
            PlayerListItem roomListItem = Instantiate(_playerListItemPrefab, _playerListContent).GetComponent<PlayerListItem>();
            roomListItem.SetUp(players[index]);
        }
        
        _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        string errorMessage = $"Room creation failed (error code {returnCode}): {message}";

        _errorText.text = errorMessage;
        Debug.LogError(errorMessage);

        MenuManager.Instance.OpenMenu(ERROR_MENU_NAME);
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu(TITLE_MENU_NAME);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in _roomListContent)
        {
            Destroy(child.gameObject);
        }

        for (int index = 0; index < roomList.Count; index++)
        {
            if (roomList[index].RemovedFromList) continue;

            //RoomListItem roomListItem = Instantiate(_roomListItemPrefab, _roomListContent).GetComponent<RoomListItem>();
            //roomListItem.SetUp(roomList[index]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerListItem roomListItem = Instantiate(_playerListItemPrefab, _playerListContent).GetComponent<PlayerListItem>();
        roomListItem.SetUp(newPlayer);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(_roomNameInputField.text)) return;

        PhotonNetwork.CreateRoom(_roomNameInputField.text);
        MenuManager.Instance.OpenMenu(LOADING_MENU_NAME);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu(LOADING_MENU_NAME);
    }

    public void JoinRoom(RoomInfo roomInfo)
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);
        MenuManager.Instance.OpenMenu(LOADING_MENU_NAME);
    }
}
