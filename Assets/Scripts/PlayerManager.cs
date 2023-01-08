using Photon.Pun;
using Photon.Realtime;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[DisallowMultipleComponent]
[RequireComponent(typeof(PhotonView))]
public class PlayerManager : MonoBehaviour
{
    private PhotonView _photonView;
    private GameObject _controller;

    private int _kills, _deaths;

    private const string PHOTON_PREFABS_FOLDER = "PhotonPrefabs", PLAYER_CONTROLLER_PREFAB_NAME = "Player Controller";
    private const string KILLS_KEY = "Kills", DEATHS_KEY = "Deaths";

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (_photonView.IsMine)
        {
            CreateController();
        }
    }

    private void CreateController()
    {
        string prefabName = Path.Combine(PHOTON_PREFABS_FOLDER, PLAYER_CONTROLLER_PREFAB_NAME);
        byte group = 0;
        object[] data = new object[] { _photonView.ViewID };

        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        _controller = PhotonNetwork.Instantiate(prefabName, spawnpoint.position, spawnpoint.rotation, group, data);
    }

    public void Die()
    {
        PhotonNetwork.Destroy(_controller);
        CreateController();

        _deaths++;

        Hashtable hash = new Hashtable
        {
            { DEATHS_KEY, _deaths }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill()
    {
        _photonView.RPC(nameof(RPC_GetKill), _photonView.Owner);
    }

    [PunRPC]
    void RPC_GetKill()
    {
        _kills++;

        Hashtable hash = new Hashtable
        {
            { KILLS_KEY, _kills }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x._photonView.Owner == player);
    }
}