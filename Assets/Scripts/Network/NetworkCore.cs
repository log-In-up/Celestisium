using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserInterface;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
        private Dictionary<string, Player[]> _teamsAndPlayers = null;

        private const string TEAM = "Team", SPECTATOR = "Spectator", RED_TEAM = "Red_team", BLUE_TEAM = "Blue_team";
        #endregion

        #region MonoBehaviour API
        public override void OnEnable()
        {
            _joinBlueTeam.onClick.AddListener(OnClickJoinBlueTeam);
            _joinRedTeam.onClick.AddListener(OnClickJoinRedTeam);

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

            base.OnDisable();
        }
        #endregion

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;

            Hashtable hashtable = new Hashtable() { { TEAM, SPECTATOR } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);

#if UNITY_EDITOR
            Debug.Log("Connected to Master. Joining the lobby...");
#endif
        }

        public override void OnJoinedLobby()
        {
            _uiCore.OpenScreen(UIScreen.Title);
#if UNITY_EDITOR
            Debug.Log("Joined Lobby.");
#endif
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
                roomListItem.SetUp(roomList[index]);
            }
        }

        #region Room callbacks
        public override void OnJoinedRoom()
        {
            _roomName.text = PhotonNetwork.CurrentRoom.Name;

            InitializePlayerLists();

            UpdateTeamLists(_blueTeamListContent, BLUE_TEAM);
            UpdateTeamLists(_redTeamListContent, RED_TEAM);
            UpdateSpectators();

            _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
            _uiCore.OpenScreen(UIScreen.Room);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            UpdateTeamLists(_blueTeamListContent, BLUE_TEAM);
            UpdateTeamLists(_redTeamListContent, RED_TEAM);
            UpdateSpectators();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Hashtable hashtable = new Hashtable()
            {
                {TEAM, SPECTATOR}
            };
            newPlayer.SetCustomProperties(hashtable);

            _players.ToList().Add(newPlayer);

            _teamsAndPlayers[SPECTATOR].ToList().Add(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            _players.ToList().Remove(otherPlayer);

            string key = otherPlayer.CustomProperties[TEAM].ToString();

            _teamsAndPlayers[key].ToList().Remove(otherPlayer);
        }

        public override void OnLeftRoom()
        {
            _uiCore.OpenScreen(UIScreen.Title);
        }
        #endregion

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            string errorMessage = $"Room creation failed (error code {returnCode}): {message}";

            _errorText.text = errorMessage;
#if UNITY_EDITOR
            Debug.LogError(errorMessage);
#endif

            _uiCore.OpenScreen(UIScreen.Error);
        }

        #region Methods
        private void InitializePlayerLists()
        {
            _teamsAndPlayers = new Dictionary<string, Player[]>()
            {
                [SPECTATOR] = new Player[0],
                [RED_TEAM] = new Player[0],
                [BLUE_TEAM] = new Player[0]
            };
            _players = PhotonNetwork.PlayerList;

            foreach (Player player in _players)
            {
                string key = player.CustomProperties[TEAM].ToString();
                _teamsAndPlayers[key].ToList().Add(player);
            }

            Hashtable currentRoomHashtable = new Hashtable()
            {
                {SPECTATOR, _teamsAndPlayers[SPECTATOR] },
                {RED_TEAM, _teamsAndPlayers[RED_TEAM] },
                {BLUE_TEAM, _teamsAndPlayers[BLUE_TEAM] }
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(currentRoomHashtable);
        }

        private void UpdateTeamLists(Transform parentThatHoldsPlayers, string team)
        {
            foreach (Transform child in parentThatHoldsPlayers)
            {
                Destroy(child.gameObject);
            }

            for (int index = 0; index < _teamsAndPlayers[team].Length; index++)
            {
                PlayerListItem roomListItem = Instantiate(_playerListItemPrefab, parentThatHoldsPlayers).GetComponent<PlayerListItem>();
                roomListItem.SetUp(_teamsAndPlayers[team][index]);
            }
        }

        private void UpdateSpectators()
        {
            List<string> spectators = new List<string>();

            for (int index = 0; index < _players.Length; index++)
            {
                if (_players[index].CustomProperties[TEAM].Equals(SPECTATOR))
                {
                    spectators.Add(_players[index].NickName);
                }
            }

            spectators.Sort();

            _spectators.text = string.Join(", ", spectators);
        }

        private void OnClickJoinBlueTeam()
        {
            Hashtable hashtable = new Hashtable() { { TEAM, BLUE_TEAM } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);

            if (!_teamsAndPlayers[BLUE_TEAM].Contains(PhotonNetwork.LocalPlayer))
            {
                _teamsAndPlayers[BLUE_TEAM] = _teamsAndPlayers[BLUE_TEAM].AddToArray(PhotonNetwork.LocalPlayer);
            }
            if (_teamsAndPlayers[RED_TEAM].Contains(PhotonNetwork.LocalPlayer))
            {
                _teamsAndPlayers[RED_TEAM] = _teamsAndPlayers[RED_TEAM].RemoveFromArray(PhotonNetwork.LocalPlayer);
            }

            Hashtable currentRoomHashtable = new Hashtable()
            {
                {SPECTATOR, _teamsAndPlayers[SPECTATOR] },
                {RED_TEAM, _teamsAndPlayers[RED_TEAM] },
                {BLUE_TEAM, _teamsAndPlayers[BLUE_TEAM] }
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(currentRoomHashtable);
        }

        private void OnClickJoinRedTeam()
        {
            Hashtable hashtable = new Hashtable() { { TEAM, RED_TEAM } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);

            if (!_teamsAndPlayers[RED_TEAM].Contains(PhotonNetwork.LocalPlayer))
            {
                _teamsAndPlayers[RED_TEAM] = _teamsAndPlayers[RED_TEAM].AddToArray(PhotonNetwork.LocalPlayer);
            }
            if (_teamsAndPlayers[BLUE_TEAM].Contains(PhotonNetwork.LocalPlayer))
            {
                _teamsAndPlayers[BLUE_TEAM] = _teamsAndPlayers[BLUE_TEAM].RemoveFromArray(PhotonNetwork.LocalPlayer);
            }

            Hashtable currentRoomHashtable = new Hashtable()
            {
                {SPECTATOR, _teamsAndPlayers[SPECTATOR] },
                {RED_TEAM, _teamsAndPlayers[RED_TEAM] },
                {BLUE_TEAM, _teamsAndPlayers[BLUE_TEAM] }
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(currentRoomHashtable);
        }
        #endregion
    }
}