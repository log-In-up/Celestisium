using Photon.Realtime;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] private ItemInfo _itemInfo;
    [SerializeField] private GameObject _itemGameObject;

    public GameObject ItemGameObject => _itemGameObject;
    public ItemInfo ItemInfo => _itemInfo;

    public abstract void Use();
}