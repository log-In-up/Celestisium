using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Players
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PhotonView))]
    public sealed class PlayerManager : MonoBehaviourPunCallbacks
    {
        private PhotonView _photonView = null;
        private PlayerController _controller = null;        

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
                CreatePlayerController();
            }
        }

        private void CreatePlayerController()
        {
            string prefabName = Path.Combine(PHOTON_PREFABS_FOLDER, PLAYER_CONTROLLER_PREFAB_NAME);
            byte group = 0;
            object[] data = new object[] { _photonView.ViewID };

            Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
            GameObject controller = PhotonNetwork.Instantiate(prefabName, spawnpoint.position, spawnpoint.rotation, group, data);
            if (controller.TryGetComponent(out PlayerController playerController))
            {
                _controller = playerController;
            }
        }

        [PunRPC]
        void RPC_GetKill()
        {
            _kills++;

            Hashtable hash = new Hashtable { { KILLS_KEY, _kills } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public void Die()
        {
            _deaths++;

            Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
            _controller.transform.position = spawnpoint.position;
            _controller.ResetPlayerStats();

            Hashtable hash = new Hashtable { { DEATHS_KEY, _deaths } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public void GetKill()
        {
            _photonView.RPC(nameof(RPC_GetKill), _photonView.Owner);
        }

        public static PlayerManager Find(Player player)
        {
            return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x._photonView.Owner == player);
        }
    }
}