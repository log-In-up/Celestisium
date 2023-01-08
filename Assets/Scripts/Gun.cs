using UnityEngine;

public abstract class Gun : Item
{
    [SerializeField] private GameObject _bulletImpactPrefab;

    public GameObject BulletImpactPrefab => _bulletImpactPrefab;

    public abstract override void Use();
}
