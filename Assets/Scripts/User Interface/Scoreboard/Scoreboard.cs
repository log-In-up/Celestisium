using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Scoreboard : MonoBehaviourPunCallbacks
{
    #region Editor fields
    [SerializeField] private Transform _container = null;
    [SerializeField] private GameObject _scoreboardItemPrefab = null;
    #endregion

    #region Fields
    private bool _scoreboardCanBeOpened;
    private Dictionary<Player, ScoreboardItem> _scoreboardItems = null;
    #endregion

    #region Properties
    public bool ScoreboardCanBeOpened { get => _scoreboardCanBeOpened; set => _scoreboardCanBeOpened = value; }
    #endregion

    #region MonoBehaviour API
    private void Awake()
    {
        _scoreboardItems = new Dictionary<Player, ScoreboardItem>();
        _scoreboardCanBeOpened = true;
    }

    private void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
        }
    }

    private void Update()
    {
        if (!_scoreboardCanBeOpened)
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                _container.gameObject.SetActive(false);
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _container.gameObject.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            _container.gameObject.SetActive(false);
        }
    }
    #endregion

    #region PunCallbacks
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }
    #endregion

    #region Methods
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
    #endregion    
}