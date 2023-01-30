using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public sealed class CreateRoomScreenObserver : ScreenObserver
    {
        #region Editor fields
        [SerializeField] private Button _back = null;
        [SerializeField] private Button _createRoom = null;
        [SerializeField] private Slider _countOfPlayers = null;
        [SerializeField] private TMP_InputField _roomNameInputField = null;
        [SerializeField] private TextMeshProUGUI _maxPlayers = null;
        [SerializeField] private Toggle _isOpen = null;
        #endregion

        #region Properties
        public override UIScreen Screen => UIScreen.CreateRoom;
        #endregion

        #region Overridden methods
        public override void Activate()
        {
            _back.onClick.AddListener(OnClickBack);
            _createRoom.onClick.AddListener(OnClickCreateRoom);
            _countOfPlayers.onValueChanged.AddListener(OnChangeCountOfPlayers);

            base.Activate();
        }

        public override void Deactivate()
        {
            _back.onClick.RemoveListener(OnClickBack);
            _createRoom.onClick.RemoveListener(OnClickCreateRoom);
            _countOfPlayers.onValueChanged.RemoveListener(OnChangeCountOfPlayers);

            base.Deactivate();
        }
        #endregion

        #region Event handlers
        private void OnClickBack()
        {
            UICore.OpenScreen(UIScreen.Title);
        }

        private void OnClickCreateRoom()
        {
            if (string.IsNullOrEmpty(_roomNameInputField.text)) return;

            RoomOptions roomOptions = new RoomOptions()
            {
                IsOpen = _isOpen.isOn,
                MaxPlayers = (byte)_countOfPlayers.value
            };

            PhotonNetwork.CreateRoom(_roomNameInputField.text, roomOptions);
            
            UICore.OpenScreen(UIScreen.Loading);
        }

        private void OnChangeCountOfPlayers(float countOfPlayers)
        {
            _maxPlayers.text = countOfPlayers.ToString();
        }
        #endregion
    }
}