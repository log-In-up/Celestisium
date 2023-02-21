using Photon.Pun;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameInput = null;

    private const string USERNAME_KEY = "Username";

    private void Start()
    {
        if (PlayerPrefs.HasKey(USERNAME_KEY))
        {
            _usernameInput.text = PlayerPrefs.GetString(USERNAME_KEY);
            PhotonNetwork.NickName = PlayerPrefs.GetString(USERNAME_KEY);
        }
        else
        {
            _usernameInput.text = $"Player {Random.Range(0, 1000):0000}";
            OnUsernameEndEdit();
        }
    }

    public void OnUsernameEndEdit()
    {
        PhotonNetwork.NickName = _usernameInput.text;
        PlayerPrefs.SetString(USERNAME_KEY, _usernameInput.text);
    }
}
