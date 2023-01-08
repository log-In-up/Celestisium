using Photon.Pun;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private const string PHOTON_PREFABS_FOLDER = "PhotonPrefabs", PLAYER_MANAGER_PREFAB_NAME = "Player Manager";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine(PHOTON_PREFABS_FOLDER, PLAYER_MANAGER_PREFAB_NAME), Vector3.zero, Quaternion.identity);
        }
    }
}