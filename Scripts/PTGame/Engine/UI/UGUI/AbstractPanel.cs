using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace PTGame.Framework
{
    public enum PanelType : byte
    {
        Bottom,
        Auto,
        Top,  //顶层
    }

    public enum PanelHideMask : byte
    {
        None,//不遮挡
        UnInteractive = 1,//隐藏交互
        Hide = 2,//隐藏下层
    }

    [RequireComponent(typeof(Canvas))]
    public class AbstractPanel : AbstractPage
    {
        //需要预加载动态资源的复写该方法。两种参数都支持
        /*
        public static List<string> PrepareDynamicResource()
        public static string PrepareDynamicResource()
        */
        [SerializeField]
        private PanelHideMask   m_HideMask = PanelHideMask.None;
        [SerializeField]
        private bool            m_CustomVisibleFlag = true;
        [SerializeField]
        private int             m_SortingOrder = -1;
        private bool            m_HasStart = false;
        private int             m_MaxSortingOrder = -1;
        private bool            m_IsOrderDirty = false;
        private int             m_LastOpenFrame = -1;

        public int lastOpenFrame
        {
            get { return m_LastOpenFrame; }
        }

        //面板影藏属性,作用于其下层面板
        public PanelHideMask hideMask
        {
            get { return m_HideMask; }
            set
            {
                if (m_HideMask == value)
                {
                    return;
                }

                m_HideMask = value;

                if (m_ParentPage == null)
                {
                    UIMgr.S.SetPanelSortingOrderDirty();
                }
            }
        }

        public bool customVisibleFlag
        {
            get { return m_CustomVisibleFlag; }
            set
            {
                if (m_CustomVisibleFlag == value)
                {
                    return;
                }

                m_CustomVisibleFlag = value;

                if (m_ParentPage == null)
                {
                    UIMgr.S.SetPanelVisible(this, m_CustomVisibleFlag);
                    UIMgr.S.SetPanelSortingOrderDirty();
                }
            }
        }

        public void SetSortingOrderDirty()
        {
            m_IsOrderDirty = true;
        }

        public void SetSiblingIndexAndSortingOrder(int siblingIndex, int sortingOrder)
        {
            if (m_IsOrderDirty || m_SortingOrder != sortingOrder)
            {

                m_SortingOrder = sortingOrder;
                transform.SetSiblingIndex(siblingIndex);
                UpdateCanvasSortingOrder();
                //PanelCanvas.sortingOrder = m_SortingOrder;
            }
        }

        public int maxSortingOrder
        {
            get { return m_MaxSortingOrder; }
            set { m_MaxSortingOrder = value; }
        }

        //需要自己控制层级的面板重写改函数(返回值大于0生效)
        public virtual int sortIndex
        {
            get { return -1; }
        }
        public void OnPanelOpen(bool firstOpen, params object[] args)
        {
            m_LastOpenFrame = Time.frameCount;
            SendViewEvent(ViewEvent.OnPanelOpen);
            ERunner.Run(OnPanelOpen, args);
            SendViewEvent(ViewEvent.OnParamUpdate);
        }

        public void OnPanelClose(bool destroy)
        {
            SendViewEvent(ViewEvent.OnPanelClose);
        }

        public virtual void OnBecomeHide()
        {

        }

        public virtual void OnBecomeVisible()
        {

        }


        protected override void OnViewEvent(ViewEvent e, object[] args)
        {
            switch (e)
            {
                case ViewEvent.Action_ClosePanel:
                    if (m_ParentPage == null)
                    {
                        UIMgr.S.ClosePanel(this);
                    }
                    break;
                case ViewEvent.Action_HidePanel:
                    if (m_ParentPage == null)
                    {
                        customVisibleFlag = false;
                    }
                    break;
                case ViewEvent.Action_ShowPanel:
                    if (m_ParentPage == null)
                    {
                        customVisibleFlag = true;
                    }
                    break;
                default:
                    base.OnViewEvent(e, args);
                    break;
            }
        }

        protected void Start()
        {
            if (m_ParentPage != null)
            {
                return;
            }

            m_HasStart = true;
            m_IsOrderDirty = true;
            UIMgr.S.SetPanelSortingOrderDirty();
        }

        private void UpdateCanvasSortingOrder()
        {
            m_MaxSortingOrder = m_SortingOrder;

            Canvas[] canvas = GetComponentsInChildren<Canvas>(true);

            int offset = 0;
            if (canvas != null)
            {
                for (int i = 0; i < canvas.Length; ++i)
                {
                    canvas[i].overrideSorting = true;
                    canvas[i].pixelPerfect = false;
                    canvas[i].sortingOrder = m_SortingOrder + offset;
                    offset += UIMgr.CANVAS_OFFSET;
                }

                m_MaxSortingOrder += offset;
            }

            SendViewEvent(ViewEvent.OnSortingLayerUpdate);
            m_IsOrderDirty = false;
        }

        protected override void BeforDestroy()
        {
            if (m_ParentPage == null)
            {
                UIMgr.S.OnPanelForceDestroy(this);
            }
        }

        protected virtual void OnPanelOpen(params object[] args)
        {

        }

        private void OnValidate()
        {
            if (!m_HasStart)
            {
                return;
            }

            //真正的面板才能生效
            if (m_ParentPage == null)
            {
                UIMgr.S.SetPanelVisible(this, m_CustomVisibleFlag);
                UIMgr.S.SetPanelSortingOrderDirty();
            }
        }
    }
}
