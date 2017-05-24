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
			UIData data = UIDataTable.Get (targetPanelName);
			if (data == null)
			{
				return null;
			}
            //string panelName = string.Format("{0}(Clone)", targetPanelName);
			AbstractPanel panel = UIMgr.S.FindPanel (data.uiID);//UIMgr.S.uiRoot.panelRoot.Find(targetPanelName);
			if (panel == null)
			{
				return null;
			}

			Transform targetPanel = panel.transform;
            if (targetPanel == null)
            {
				//Log.w("# FindTransformInPanel Not Find Panel:" + panelName);
                return null;
            }

			if (string.IsNullOrEmpty(targetUINodePath))
			{
				return targetPanel as RectTransform;
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
