using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI _playerName = null;

    private Player _player = null;

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(_player.Equals(otherPlayer))
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

    public void SetUp(Player player)
    {
        _player = player;
        _playerName.text = player.NickName;
    }
}