using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace PTGame.Framework
{
    public class GuideHelper
    {
        public static RectTransform FindTransformInPanel(string targetPanelName, string targetUINodePath)
        {
            string panelName = string.Format("{0}(Clone)", targetPanelName);
            Transform targetPanel = UIMgr.S.uiRoot.panelRoot.Find(panelName);

            if (targetPanel == null)
            {
				//Log.w("# FindTransformInPanel Not Find Panel:" + panelName);
                return null;
            }

            RectTransform result = targetPanel.Find(targetUINodePath) as RectTransform;

            if (result == null)
            {
				//Log.w(string.Format("# FindTransformInPanel Not Find Node:{0}/{1}", panelName, targetUINodePath));
                return null;
            }

            return result;
        }
    }
}
