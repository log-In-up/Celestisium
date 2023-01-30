using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserInterface;

namespace GameNetworking
{
    [DisallowMultipleComponent]
    public sealed class NetworkCore : MonoBehaviourPunCallbacks
    {
        #region Editor fields
        [Header("Main components")]
        [SerializeField] private UICore _uiCore = null;

        [Header("Room menu components")]
        [SerializeField] private Button _joinBlueTeam = null;
        [SerializeField] private Button _joinRedTeam = null;
        [SerializeField] private GameObject _playerListItemPrefab = null;
        [SerializeField] private GameObject _startGameButton = null;
        [SerializeField] private TextMeshProUGUI _roomName = null;
        [SerializeField] private TextMeshProUGUI _spectators = null;
        [SerializeField] private Transform _blueTeamListContent = null;
        [SerializeField] private Transform _redTeamListContent = null;

        [Header("Error menu components")]
        [SerializeField] private TextMeshProUGUI _errorText = null;

        [Header("Find room menu components")]
        [SerializeField] private GameObject _roomListItemPrefab = null;
        [SerializeField] private Transform _roomListContent = null;
        #endregion

        #region Fields
        private Player[] _players = null;
        private PhotonTeamsManager _teamsManager = null;

        private const byte SPECTATOR_TEAM = 0, BLUE_TEAM = 1, RED_TEAM = 2;
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            _teamsManager = PhotonTeamsManager.Instance;
        }

        public override void OnEnable()
        {
            _joinBlueTeam.onClick.AddListener(OnClickJoinBlueTeam);
            _joinRedTeam.onClick.AddListener(OnClickJoinRedTeam);
            PhotonTeamsManager.PlayerJoinedTeam += PlayerJoinedTeam;
            PhotonTeamsManager.PlayerLeftTeam += PlayerLeftTeam;

            base.OnEnable();
        }

        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
#if UNITY_EDITOR
                Debug.Log("Connecting to Master...");
#endif
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnDisable()
        {
            _joinBlueTeam.onClick.RemoveListener(OnClickJoinBlueTeam);
            _joinRedTeam.onClick.RemoveListener(OnClickJoinRedTeam);
            PhotonTeamsManager.PlayerJoinedTeam -= PlayerJoinedTeam;
            PhotonTeamsManager.PlayerLeftTeam -= PlayerLeftTeam;

            base.OnDisable();
        }
        #endregion

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;

#if UNITY_EDITOR
            Debug.Log("Connected to Master. Joining the lobby...");
#endif
        }

        public override void OnJoinedLobby()
        {
            PhotonNetwork.LocalPlayer.JoinTeam(SPECTATOR_TEAM);
            _uiCore.OpenScreen(UIScreen.Title);
#if UNITY_EDITOR
            Debug.Log("Joined Lobby.");
#endif
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
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

                RoomListItem roomListItem = Instantiate(_roomListItemPrefab, _roomListContent).GetComponent<RoomListItem>();
                roomListItem.SetUp(roomList[index], _uiCore);
            }
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            string errorMessage = $"Room creation failed (error code {returnCode}): {message}";

            _errorText.text = errorMessage;
#if UNITY_EDITOR
            Debug.LogError(errorMessage);
#endif

            _uiCore.OpenScreen(UIScreen.Error);
        }

        #region Room callbacks
        public override void OnJoinedRoom()
        {
            _players = PhotonNetwork.PlayerList;

            _roomName.text = PhotonNetwork.CurrentRoom.Name;

            UpdateSpectators();
            UpdateListOfPlayers(BLUE_TEAM, _blueTeamListContent);
            UpdateListOfPlayers(RED_TEAM, _redTeamListContent);

            _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
            _uiCore.OpenScreen(UIScreen.Room);
        }

        public override void OnLeftRoom()
        {
            _uiCore.OpenScreen(UIScreen.Loading);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            newPlayer.JoinTeam(SPECTATOR_TEAM);
            UpdateSpectators();

            _players.AddToArray(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            otherPlayer.LeaveCurrentTeam();
            _players.RemoveFromArray(otherPlayer);
        }
        #endregion

        #region Methods
        private void UpdateSpectators()
        {
            _teamsManager.TryGetTeamMembers(SPECTATOR_TEAM, out Player[] members);

            List<string> spectators = new List<string>();

            for (int index = 0; index < members.Length; index++)
            {
                spectators.Add(members[index].NickName);
            }

            spectators.Sort();

            _spectators.text = string.Join(", ", spectators);
        }

        private void UpdateListOfPlayers(byte code, Transform parent)
        {
            _teamsManager.TryGetTeamMembers(code, out Player[] members);

            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }

            foreach (Player teamMate in members)
            {
                PlayerListItem roomListItem = Instantiate(_playerListItemPrefab, parent).GetComponent<PlayerListItem>();
                roomListItem.SetUp(teamMate);
            }
        }
        #endregion

        #region Event handlers        
        private void OnClickJoinBlueTeam()
        {
            if (!_teamsManager.TryGetTeamByCode(BLUE_TEAM, out PhotonTeam blueTeam)) return;

            PhotonNetwork.LocalPlayer.SwitchTeam(blueTeam);
        }

        private void OnClickJoinRedTeam()
        {
            if (!_teamsManager.TryGetTeamByCode(RED_TEAM, out PhotonTeam redTeam)) return;

            PhotonNetwork.LocalPlayer.SwitchTeam(redTeam);
        }

        private void PlayerLeftTeam(Player player, PhotonTeam team)
        {
            UpdateSpectators();
            UpdateListOfPlayers(BLUE_TEAM, _blueTeamListContent);
            UpdateListOfPlayers(RED_TEAM, _redTeamListContent);
        }

        private void PlayerJoinedTeam(Player player, PhotonTeam team)
        {
            UpdateSpectators();
            UpdateListOfPlayers(BLUE_TEAM, _blueTeamListContent);
            UpdateListOfPlayers(RED_TEAM, _redTeamListContent);
        }
        #endregion
    }
}