using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PTGame.Framework
{
    public class HighlightUICommand : AbstractGuideCommand
    {
		private IUINodeFinder m_Finder;
        private bool m_NeedCreateGraphicRaycast = false;
		private bool m_NeedClose = true;

        private Canvas m_Canvas;
        private int m_CanvasPreSortingOrder = -1;
        private GraphicRaycaster m_GraphicRaycaster;
        private bool m_IsCreateGraphicRaycaster;

		public override void SetParam(object[] pv)
		{
			if (pv.Length == 0)
			{
				Log.w ("HighlightUICommand Init With Invalid Param.");
				return;
			}

            m_Finder = pv[0] as IUINodeFinder;

            if (pv.Length > 1)
            {
                m_NeedCreateGraphicRaycast = Helper.String2Bool((string)pv[1]);
                if (pv.Length > 2)
                {
                    m_NeedClose = Helper.String2Bool((string)pv[2]);
                }
            }
        }

		protected override void OnStart()
        {
			RectTransform targetNode = m_Finder.FindNode(false) as RectTransform;

			if (targetNode == null)
            {
                return;
            }

            m_Canvas = targetNode.GetComponent<Canvas>();
            if (m_Canvas == null)
            {
                m_Canvas = targetNode.gameObject.AddComponent<Canvas>();
                m_CanvasPreSortingOrder = -1;
            }
            else
            {
                m_CanvasPreSortingOrder = m_Canvas.sortingOrder;
            }

            if (m_NeedCreateGraphicRaycast)
            {
                m_GraphicRaycaster = targetNode.GetComponent<GraphicRaycaster>();
                if (m_GraphicRaycaster == null)
                {
                    m_IsCreateGraphicRaycaster = true;
                    m_GraphicRaycaster = targetNode.gameObject.AddComponent<GraphicRaycaster>();
                }
            }

            Action<int> orderUpdate = OnSortingOrderUpdate;
            UIMgr.S.OpenTopPanel(EngineUI.HighlightMaskPanel, null, orderUpdate);
		}

        protected void OnSortingOrderUpdate(int panelOrder)
        {
            m_Canvas.sortingOrder = panelOrder + 1;
        }

        protected override void OnFinish(bool forceClean)
        {
			if (m_NeedClose || forceClean)
			{
				UIMgr.S.ClosePanelAsUIID(EngineUI.HighlightMaskPanel);	
			}

            if (m_GraphicRaycaster != null)
            {
                if (m_IsCreateGraphicRaycaster)
                {
                    GameObject.Destroy(m_GraphicRaycaster);
                }
                m_GraphicRaycaster = null;
            }

            if (m_Canvas != null)
            {
                if (m_CanvasPreSortingOrder >= 0)
                {
                    m_Canvas.sortingOrder = m_CanvasPreSortingOrder;
                }
                else
                {
                    GameObject.Destroy(m_Canvas);
                }
                m_Canvas = null;
            }
        }
    }
}
