using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class HighlightUICommand : GuideCommand
    {
        public string targetPanelName;
        public string targetUINodePath;

        private RectTransform m_TargetNode;

        public HighlightUICommand()
        {

        }

        public HighlightUICommand(string targetPanel, string uiNodePath)
        {
            this.targetPanelName = targetPanel;
            this.targetUINodePath = uiNodePath;
        }

        public override void Start()
        {
            string panelName = string.Format("{0}(Clone)", targetPanelName);
            Transform targetPanel = UIMgr.S.uiRoot.panelRoot.Find(panelName);

            if (targetPanel == null)
            {
                Log.w("# HighlightUICommand Not Find Panel:" + panelName);
                return;
            }

            m_TargetNode = targetPanel.Find(targetUINodePath) as RectTransform;

            if (m_TargetNode == null)
            {
                Log.w(string.Format("# HighlightUICommand Not Find Node:{0}/{1}", panelName, targetUINodePath));
                return;
            }

            UIMgr.S.OpenTopPanel(EngineUI.GuidePanel, OnGuidePanelOpen);
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
        }
    }
}
