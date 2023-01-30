using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public sealed class TitleScreenObserver : ScreenObserver
    {
        #region Editor fields
        [SerializeField] private Button _createRoom = null;
        [SerializeField] private Button _findRoom = null;
        [SerializeField] private Button _quitGame = null;
        [SerializeField] private TMP_InputField _usernameInput = null;
        #endregion

        #region Fields
        private const string USERNAME_KEY = "Username";
        #endregion

        #region Properties
        public override UIScreen Screen => UIScreen.Title;
        #endregion

        #region Overridden methods
        public override void Activate()
        {
            InitializeUsername();

            _createRoom.onClick.AddListener(OnClickCreateRoom);
            _findRoom.onClick.AddListener(OnClickFindRoom);
            _quitGame.onClick.AddListener(OnClickQuitGame);
            _usernameInput.onEndEdit.AddListener(OnUsernameEndEdit);

            base.Activate();
        }        

        public override void Deactivate()
        {
            _createRoom.onClick.RemoveListener(OnClickCreateRoom);
            _findRoom.onClick.RemoveListener(OnClickFindRoom);
            _quitGame.onClick.RemoveListener(OnClickQuitGame);
            _usernameInput.onEndEdit.RemoveListener(OnUsernameEndEdit);

            base.Deactivate();
        }
        #endregion

        #region Methods
        private void InitializeUsername()
        {
            if (PlayerPrefs.HasKey(USERNAME_KEY))
            {
                _usernameInput.text = PlayerPrefs.GetString(USERNAME_KEY);
                PhotonNetwork.NickName = PlayerPrefs.GetString(USERNAME_KEY);
            }
            else
            {
                string randomNickName = $"Player {Random.Range(0, 1000):0000}";

                _usernameInput.text = randomNickName;

                PhotonNetwork.NickName = randomNickName;
                PlayerPrefs.SetString(USERNAME_KEY, randomNickName);
            }
        }
        #endregion

        #region Event handlers
        private void OnClickCreateRoom()
        {
            UICore.OpenScreen(UIScreen.CreateRoom);
        }

        private void OnClickFindRoom()
        {
            UICore.OpenScreen(UIScreen.FindRoom);
        }

        private void OnClickQuitGame() 
        {
            PhotonNetwork.LeaveLobby();
            Application.Quit();
        }
                
        private void OnUsernameEndEdit(string nickname)
        {
            PhotonNetwork.NickName = nickname;
            PlayerPrefs.SetString(USERNAME_KEY, nickname);
        }
        #endregion
    }
}