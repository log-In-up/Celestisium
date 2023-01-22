using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public sealed class RoomScreenObserver : ScreenObserver
    {
        #region Editor fields
        [SerializeField] private Button _leaveRoom = null;
        
        [SerializeField] private Button _startGame = null;
        #endregion

        #region Properties
        public override UIScreen Screen => UIScreen.Room;
        #endregion

        #region Overridden methods
        public override void Activate()
        {
            _leaveRoom.onClick.AddListener(OnClickLeaveRoom);
            _startGame.onClick.AddListener(OnClickStartGame);

            base.Activate();
        }

        public override void Deactivate()
        {            
            _leaveRoom.onClick.RemoveListener(OnClickLeaveRoom);
            _startGame.onClick.RemoveListener(OnClickStartGame);

            base.Deactivate();
        }
        #endregion

        #region Event handlers
        private void OnClickLeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            UICore.OpenScreen(UIScreen.Loading);
        }

        private void OnClickStartGame()
        {
            PhotonNetwork.LoadLevel(1);
        }
        #endregion
    }
}