using UnityEngine;

[DisallowMultipleComponent]
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    private Spawnpoint[] _spawnpoints = null;

    private void Awake()
    {
        Instance = this;
        _spawnpoints = GetComponentsInChildren<Spawnpoint>();
    }

    public Transform GetSpawnpoint()
    {
        return _spawnpoints[Random.Range(0, _spawnpoints.Length)].transform;
    }
}