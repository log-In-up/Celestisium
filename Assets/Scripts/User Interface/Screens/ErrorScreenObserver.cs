using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public sealed class ErrorScreenObserver : ScreenObserver
    {
        #region Editor fields
        [SerializeField] private Button _ok = null;
        #endregion

        #region Properties
        public override UIScreen Screen => UIScreen.Error;
        #endregion

        #region Overridden methods
        public override void Activate()
        {
            _ok.onClick.AddListener(OnClickOk);

            base.Activate();
        }

        public override void Deactivate()
        {
            _ok.onClick.RemoveListener(OnClickOk);

            base.Deactivate();
        }
        #endregion

        #region Editor fields
        private void OnClickOk()
        {
            UICore.OpenScreen(UIScreen.Title);
        }
        #endregion
    }
}