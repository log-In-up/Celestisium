using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _container = null;
    [SerializeField] private GameObject _scoreboardItemPrefab = null;

    private Dictionary<Player, ScoreboardItem> _scoreboardItems = null;

    private void Start()
    {
        _scoreboardItems = new Dictionary<Player, ScoreboardItem>();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _container.gameObject.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            _container.gameObject.SetActive(false);
        }
    }

    private void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(_scoreboardItemPrefab, _container).GetComponent<ScoreboardItem>();
        item.Initialize(player);
        _scoreboardItems[player] = item;
    }

    private void RemoveScoreboardItem(Player player)
    {
        Destroy(_scoreboardItems[player].gameObject);
        _scoreboardItems.Remove(player);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }
}