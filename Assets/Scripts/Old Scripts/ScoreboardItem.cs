using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[DisallowMultipleComponent]
public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI _usernameText = null;
    [SerializeField] private TextMeshProUGUI _killsText = null;
    [SerializeField] private TextMeshProUGUI _deathsText = null;

    private Player _player = null;

    private const string KILLS_KEY = "Kills", DEATHS_KEY = "Deaths";

    public void Initialize(Player player)
    {
        _player = player;

        _usernameText.text = player.NickName;
        UpdateStats();
    }

    private void UpdateStats()
    {
        if (_player.CustomProperties.TryGetValue(KILLS_KEY, out object kills))
        {
            _killsText.text = kills.ToString();
        }
        if (_player.CustomProperties.TryGetValue(DEATHS_KEY, out object deaths))
        {
            _deathsText.text = deaths.ToString();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == _player)
        {
            if (changedProps.ContainsKey(KILLS_KEY) || changedProps.ContainsKey(DEATHS_KEY))
            {
                UpdateStats();
            }
        }
    }
}