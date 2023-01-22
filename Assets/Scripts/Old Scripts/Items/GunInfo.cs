using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")]
public class GunInfo : ItemInfo
{
    [SerializeField,Min(0.0f)] private float _damage;

    public float Damage => _damage;
}
