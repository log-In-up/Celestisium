using UnityEngine;

namespace UserInterface
{
    [DisallowMultipleComponent]
    public abstract class ScreenObserver : MonoBehaviour
    {
        #region Fields
        private bool _isOpened = default;
        private UICore _uiCore = null;
        #endregion

        #region Properties
        public bool IsOpen => _isOpened;
        public UICore UICore => _uiCore;

        public abstract UIScreen Screen { get; }
        #endregion

        #region Virtual methods
        public virtual void Activate()
        {
            _isOpened = true;
            gameObject.SetActive(_isOpened);
        }

        public virtual void Deactivate()
        {
            _isOpened = false;
            gameObject.SetActive(_isOpened);
        }
        #endregion

        #region Public methods
        public void SetScreenData(UICore uiCore)
        {
            _uiCore = uiCore;
        }
        #endregion
    }
}