using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserInterface;

namespace GameNetworking
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class RoomListItem : MonoBehaviour
    {
        #region Editor field
        [SerializeField] private TextMeshProUGUI _roomName = null;
        #endregion

        #region Fields
        private RoomInfo _roomInfo = null;
        private Button _button = null;
        private UICore _uiCore= null;
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }
        #endregion

        #region Methods
        private void OnClick()
        {
            PhotonNetwork.JoinRoom(_roomInfo.Name);
            _uiCore.OpenScreen(UIScreen.Loading);
        }
        #endregion

        #region Public methods
        public void SetUp(RoomInfo roomInfo, UICore uiCore)
        {
            _roomInfo = roomInfo;
            _roomName.text = _roomInfo.Name;

            _uiCore = uiCore;
        }
        #endregion
    }
}