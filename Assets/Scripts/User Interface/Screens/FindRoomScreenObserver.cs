using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public sealed class FindRoomScreenObserver : ScreenObserver
    {
        #region Editor field
        [SerializeField] private Button _back = null;
        #endregion

        #region Properties
        public override UIScreen Screen => UIScreen.FindRoom;
        #endregion

        #region Overridden methods
        public override void Activate()
        {
            _back.onClick.AddListener(OnClickBack);

            base.Activate();
        }

        public override void Deactivate()
        {
            _back.onClick.RemoveListener(OnClickBack);

            base.Deactivate();
        }
        #endregion

        #region Event handler
        private void OnClickBack()
        {
            UICore.OpenScreen(UIScreen.Title);
        }
        #endregion
    }
}