using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserInterface
{
    public sealed class LoadingScreenObserver : ScreenObserver
    {
        #region Properties
        public override UIScreen Screen => UIScreen.Loading;
        #endregion

        #region Overridden methods
        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
        #endregion
    }
}