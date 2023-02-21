using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Players
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Scoreboard))]
    public sealed class PlayerCanvasCore : MonoBehaviour
    {
        #region Editor field
        [SerializeField] private Button _leftFromRoomButton = null;
        [SerializeField] private GameObject _crosshair = null;
        [SerializeField] private GameObject _menu = null;
        [SerializeField] private GameObject _hpBar = null;
        [SerializeField] private PlayerController _playerController = null;
        #endregion

        #region
        private Scoreboard _scoreboard = null;
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            _scoreboard = GetComponent<Scoreboard>();
        }

        private void OnEnable()
        {
            _leftFromRoomButton.onClick.AddListener(OnClickLeftFromRoom);
        }

        private void Start()
        {
            SetActiveMenuWindow(_scoreboard.ScoreboardCanBeOpened);
            SetCursorState(_scoreboard.ScoreboardCanBeOpened);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _scoreboard.ScoreboardCanBeOpened = !_scoreboard.ScoreboardCanBeOpened;

                SetActiveMenuWindow(_scoreboard.ScoreboardCanBeOpened);
                SetCursorState(_scoreboard.ScoreboardCanBeOpened);

                _playerController.IsInMenu = !_scoreboard.ScoreboardCanBeOpened;
            }
        }

        private void OnDisable()
        {
            _leftFromRoomButton.onClick.RemoveListener(OnClickLeftFromRoom);
        }
        #endregion

        #region Methods
        private void SetActiveMenuWindow(bool value)
        {
            _crosshair.SetActive(value);
            _hpBar.SetActive(value);
            _menu.SetActive(!value);
        }

        private void SetCursorState(bool value)
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !value;
        }
        #endregion

        #region Event Handlers
        private void OnClickLeftFromRoom()
        {            
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LoadLevel(0);
        }
        #endregion
    }
}