//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Qarth
{
    //交互劫持
    public class ButtonHackCommand : AbstractGuideCommand
    {
		private IUINodeFinder m_Finder;
        private Transform m_TargetButton;
        private bool m_HasDown = false;
        private static List<RaycastResult> m_Result = new List<RaycastResult>();

		public override void SetParam (object[] param)
		{
			m_Finder = param[0] as IUINodeFinder;
		}

		protected override void OnStart()
        {
			m_TargetButton = m_Finder.FindNode (false);

            if (m_TargetButton == null)
            {
                return;
            }

			UIMgr.S.topPanelHideMask = PanelHideMask.UnInteractive;
			AppLoopMgr.S.onUpdate += Update;
        }
        
		protected override void OnFinish (bool forceClean)
		{
			UIMgr.S.topPanelHideMask = PanelHideMask.None;
			AppLoopMgr.S.onUpdate -= Update;
		}

        private void OnClickUpOnTarget()
        {
			FinishStep ();
            ExecuteEvents.Execute<IPointerClickHandler>(m_TargetButton.gameObject, new PointerEventData(UnityEngine.EventSystems.EventSystem.current), ExecuteEvents.pointerClickHandler);
            //FinishStep();
        }

        private void OnClickDownOnTarget()
        {
            ExecuteEvents.Execute<IPointerDownHandler>(m_TargetButton.gameObject, new PointerEventData(UnityEngine.EventSystems.EventSystem.current), ExecuteEvents.pointerDownHandler);
        }

        private void Update()
        {
            if (m_TargetButton == null)
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

			var graphicRaycasr = m_TargetButton.GetComponentInParent<GraphicRaycaster>();

			if (graphicRaycasr == null)
			{
				return false;
			}

			graphicRaycasr.Raycast(pd, m_Result);

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
