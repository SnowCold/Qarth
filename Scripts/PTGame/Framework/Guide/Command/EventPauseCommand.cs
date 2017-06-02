using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class EventPauseCommand : AbstractGuideCommand
    {
        protected override void OnStart()
        {
            UIMgr.S.topPanelHideMask = PanelHideMask.UnInteractive;
        }

        protected override void OnFinish(bool forceClean)
        {
            UIMgr.S.topPanelHideMask = PanelHideMask.None;
        }
    }
}
