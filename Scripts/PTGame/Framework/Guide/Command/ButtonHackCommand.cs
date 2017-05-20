using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PTGame.Framework
{
    //交互劫持
    public class ButtonHackCommand : GuideCommand
    {
        public string targetPanelName;
        public string targetUINodePath;

        private Transform m_TargetButton;
        private GraphicRaycaster m_GraphicRaycaster;
        private bool m_HasDown = false;
        private static List<RaycastResult> m_Result = new List<RaycastResult>();

        public ButtonHackCommand(string targetPanel, string uiNodePath)
        {
            this.targetPanelName = targetPanel;
            this.targetUINodePath = uiNodePath;
        }

        public override void Start()
        {
            m_TargetButton = GuideHelper.FindTransformInPanel(targetPanelName, targetUINodePath);

            if (m_TargetButton == null)
            {
                return;
            }

            m_GraphicRaycaster = m_TargetButton.GetComponentInParent<GraphicRaycaster>();

            if (m_GraphicRaycaster == null)
            {
                return;
            }

            UIMgr.S.OpenTopPanel(EngineUI.GuidePanel, OnGuidePanelOpen);
        }

        private void OnGuidePanelOpen(AbstractPanel panel)
        {
            if (m_TargetButton == null)
            {
                return;
            }

            GuidePanel guidePanel = panel as GuidePanel;
            if (guidePanel == null)
            {
                return;
            }

            guidePanel.hideMask = PanelHideMask.UnInteractive;
            AppLoopMgr.S.onUpdate += Update;
        }
        
        private void OnClickUpOnTarget()
        {
            ExecuteEvents.Execute<IPointerClickHandler>(m_TargetButton.gameObject, new PointerEventData(UnityEngine.EventSystems.EventSystem.current), ExecuteEvents.pointerClickHandler);
            FinishStep();
        }

        private void OnClickDownOnTarget()
        {
            ExecuteEvents.Execute<IPointerDownHandler>(m_TargetButton.gameObject, new PointerEventData(UnityEngine.EventSystems.EventSystem.current), ExecuteEvents.pointerDownHandler);
        }

        private void Update()
        {
            if (m_GraphicRaycaster == null || m_TargetButton == null)
            {
                AppLoopMgr.S.onUpdate -= Update;
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (CheckIsTouchInTarget())
                {
                    m_HasDown = true;
                    OnClickDownOnTarget();
                }
            }

            if (!m_HasDown)
            {
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_HasDown = false;

                if (CheckIsTouchInTarget())
                {
                    OnClickUpOnTarget();
                }
            }
        }

        private bool CheckIsTouchInTarget()
        {
            PointerEventData pd = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            pd.position = Input.mousePosition;

            m_GraphicRaycaster.Raycast(pd, m_Result);

            if (m_Result.Count == 0)
            {
                return false;
            }

            if (IsHitWhiteObject(m_Result))
            {
                m_Result.Clear();
                return true;
            }

            m_Result.Clear();
            return false;
        }

        private bool IsHitWhiteObject(List<RaycastResult> result)
        {
            if (result == null || result.Count == 0)
            {
                return false;
            }

            for (int i = result.Count - 1; i >= 0; --i)
            {
                GameObject go = result[i].gameObject;
                if (go != null)
                {
                    if (IsHitWhiteObject(go.transform))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsHitWhiteObject(Transform tr)
        {
            if (tr.IsChildOf(m_TargetButton.transform))
            {
                return true;
            }

            return false;
        }
    }
}
