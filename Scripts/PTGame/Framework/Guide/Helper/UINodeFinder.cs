using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
	public class UINodeFinder : IUINodeFinder
	{

		private string m_TargetPanel;
		private string m_TargetUINode;

		private Transform m_Result;

		public override string ToString ()
		{
			return string.Format("Panel:{0},UI:{1}", m_TargetPanel, m_TargetUINode);
		}

		public void SetParam(object[] pv)
		{
			if (pv.Length == 0)
			{
				Log.w ("UINodeFinder Init With Invalid Param.");
				return;
			}

			m_TargetPanel = pv[0] as string;
			if (pv.Length > 1)
			{
				m_TargetUINode = pv[1] as string;
			}

		}

		public Transform FindNode(bool search)
		{
			//if (search)
			{
				m_Result = FindTransformInPanel(m_TargetPanel, m_TargetUINode);
			}

			return m_Result;
		}

        public static RectTransform FindTransformInPanel(string targetPanelName, string targetUINodePath)
        {
			UIData data = UIDataTable.Get(targetPanelName);
			if (data == null)
			{
				return null;
			}
            //string panelName = string.Format("{0}(Clone)", targetPanelName);
			AbstractPanel panel = UIMgr.S.FindPanel(data.uiID);//UIMgr.S.uiRoot.panelRoot.Find(targetPanelName);
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

