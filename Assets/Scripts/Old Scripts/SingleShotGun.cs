using Photon.Pun;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PhotonView))]
public class SingleShotGun : Gun
{
    [SerializeField] private Camera _camera = null;

    private PhotonView _photonView = null;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public override void Use()
    {
        Shoot();
    }

    private void Shoot()
    {
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = _camera.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(((GunInfo)ItemInfo).Damage);
            }
            _photonView.RPC("RPC_Shoot", RpcTarget.All, hitInfo.point, hitInfo.normal);
        }
    }

    [PunRPC]
    private void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            Vector3 position = hitPosition + hitNormal * 0.001f;
            Quaternion rotation = Quaternion.LookRotation(hitNormal, Vector3.up) * BulletImpactPrefab.transform.rotation;

            GameObject bulletImpactObj = Instantiate(BulletImpactPrefab, position, rotation);
            bulletImpactObj.transform.SetParent(colliders[0].transform);

            Destroy(bulletImpactObj, 10.0f);
        }
    }
}