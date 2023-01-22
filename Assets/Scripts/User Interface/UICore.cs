using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UserInterface
{
    [DisallowMultipleComponent]
    public sealed class UICore : MonoBehaviour
    {
        #region Editor fields
        [Header("Main components")]
        [SerializeField] private RectTransform _screenContent = null;
        #endregion

        #region Fields
        private Dictionary<UIScreen, ScreenObserver> _screens = null;
        #endregion

        #region MonoBehaiour API
        private void Awake()
        {
            _screens = GetScreens();
        }

        private void Start()
        {
            InitializeScreens();            
        }
        #endregion

        #region Public methods
        public void OpenScreen(UIScreen screen)
        {
            foreach (KeyValuePair<UIScreen, ScreenObserver> userInterfaceScreen in _screens)
            {
                if (screen.Equals(userInterfaceScreen.Key))
                {
                    userInterfaceScreen.Value.Activate();
                }
                else if (userInterfaceScreen.Value.IsOpen)
                {
                    userInterfaceScreen.Value.Deactivate();
                }
            }
        }
        #endregion

        #region Methods
        private void InitializeScreens()
        {
            foreach (ScreenObserver screen in _screens.Values)
            {
                screen.SetScreenData(this);
            }
        }

        private Dictionary<UIScreen, ScreenObserver> GetScreens()
        {
            List<ScreenObserver> screens = _screenContent.GetComponentsInChildren<ScreenObserver>(true).ToList();
#if UNITY_EDITOR
            if (screens.Count != Enum.GetNames(typeof(UIScreen)).Length)
            {
                StringBuilder message = new StringBuilder("The number of screens implemented is ");

                message.Append(screens.Count > Enum.GetNames(typeof(UIScreen)).Length ? "more" : "less");
                message.Append($" than in {_screenContent}.");
                Debug.LogError(message.ToString());
            }
#endif
            Dictionary<UIScreen, ScreenObserver> result = new Dictionary<UIScreen, ScreenObserver>();

            foreach (ScreenObserver screen in screens)
            {
                result.Add(screen.Screen, screen);
            }

            return result;
        }
        #endregion
    }
}