using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class HighlightUICommand : GuideCommand
    {
		private string targetPanelName;
		private string targetUINodePath;
        private GuideHighlightMask.Shape shape;

        private RectTransform m_TargetNode;

		public override void SetParam(string param)
		{
			
			string[] pv = param.Split (',');
			if (pv.Length == 0)
			{
				Log.w ("HighlightUICommand Init With Invalid Param.");
				return;
			}

			try
			{
				this.targetPanelName = pv[0];
				this.targetUINodePath = pv[1];
				this.shape = (GuideHighlightMask.Shape)int.Parse(pv[2]);
			}
			catch(Exception e)
			{
				Log.e (e);
			}

		}

        public override void Start()
        {
            m_TargetNode = GuideHelper.FindTransformInPanel(targetPanelName, targetUINodePath);

            if (m_TargetNode == null)
            {
                return;
            }

			UIMgr.S.OpenTopPanel(EngineUI.GuidePanel, OnGuidePanelOpen);
		}

        public override void OnFinish()
        {
            UIMgr.S.ClosePanelAsUIID(EngineUI.GuidePanel);
        }

        private void OnGuidePanelOpen(AbstractPanel panel)
        {
            if (m_TargetNode == null)
            {
                return;
            }

            GuidePanel guidePanel = panel as GuidePanel;
            if (guidePanel == null)
            {
                return;
            }

            guidePanel.highlightMask.target = m_TargetNode;
            guidePanel.highlightMask.shape = shape;
        }
    }
}
