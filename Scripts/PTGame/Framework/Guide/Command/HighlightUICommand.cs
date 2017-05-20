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
        public GuideHighlightMask.Shape shape;

        private RectTransform m_TargetNode;

        public HighlightUICommand(string targetPanel, string uiNodePath, GuideHighlightMask.Shape shape)
        {
            this.targetPanelName = targetPanel;
            this.targetUINodePath = uiNodePath;
            this.shape = shape;
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
