using UnityEngine;

[DisallowMultipleComponent]
public class MenuManager : MonoBehaviour
{
    [SerializeField] private Menu[] menus = null;

    public static MenuManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].MenuName == menuName)
            {
                menus[i].Open();
            }
            else if (menus[i].IsOpen)
            {
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].IsOpen)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu) 
    { 
        menu.Close();
    }
}