using UnityEngine;

[DisallowMultipleComponent]
public class Menu : MonoBehaviour
{
    [SerializeField] private string _menuName;
    [SerializeField] private bool _isOpen = false;

    public bool IsOpen => _isOpen;
    public string MenuName => _menuName;

    public void Open()
    {
        _isOpen = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        _isOpen = false;
        gameObject.SetActive(false);
    }
}
