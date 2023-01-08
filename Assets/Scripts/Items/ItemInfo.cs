using UnityEngine;

public class ItemInfo : ScriptableObject
{
    [SerializeField] private string _itemName;

    public string ItemName => _itemName;
}
