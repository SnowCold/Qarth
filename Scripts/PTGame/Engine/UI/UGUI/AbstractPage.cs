using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PTGame.Framework
{
    public class AbstractPage : AbstractUIElement
    {
        [SerializeField]
        protected AbstractPage m_ParentPage;
        [SerializeField]
        private SpritesData[] m_SpritesData;

        private int m_PanelID = -1;
        private EventRegisterHelper m_EventRegisterHelper;
        private bool m_HasInitUI = false;
        private bool m_HasOpen = false;
        private SpritesHandler m_SpritesHandler;
        private ResLoader m_GlobalResLoader;

        protected int m_UIID;

        public int uiID
        {
            get { return m_UIID; }
            set { m_UIID = value; }
        }

        public bool hasOpen
        {
            get { return m_HasOpen; }
        }

        public AbstractPage parentPage
        {
            get { return m_ParentPage; }
            set
            {
                m_ParentPage = value;
            }
        }

        public int panelID
        {
            get
            {
                return GetParentPanelID();
            }
            set { m_PanelID = value; }
        }

#region 通用功能接口

        public Sprite FindSprite(string spriteName, bool global = false)
        {
            Sprite result = null;
            if (m_SpritesData == null || m_SpritesData.Length == 0)
            {

            }
            else
            {
                if (m_SpritesHandler == null)
                {
                    m_SpritesHandler = new SpritesHandler();
                    m_SpritesHandler.SetData(m_SpritesData);
                }

                result = m_SpritesHandler.FindSprite(spriteName);
            }

            if (result == null)
            {
                result = FindSpriteFromParent(spriteName, global);
            }

            if (result == null && global)
            {
                if (m_GlobalResLoader == null)
                {
                    m_GlobalResLoader = ResLoader.Allocate();
                }

                result = m_GlobalResLoader.LoadSync(spriteName) as Sprite;
            }

            if (result == null)
            {
                Log.w("Not Find Sprite:" + spriteName);
            }

            return result;
        }

        public void ReleaseAllGlobalRes()
        {
            if (m_GlobalResLoader != null)
            {
                m_GlobalResLoader.Recycle2Cache();
                m_GlobalResLoader = null;
            }
        }

        public int GetParentPanelID()
        {
            if (m_ParentPage == null)
            {
                return m_PanelID;
            }
            return m_ParentPage.GetParentPanelID();
        }

        public void SendEvent<K>(K key, params object[] args) where K : IConvertible
        {
            EventSystem.S.Send(key, args);
        }

        public void RegisterEvent<K>(K key, OnEvent callback) where K : IConvertible
        {
            if (m_EventRegisterHelper == null)
            {
                m_EventRegisterHelper = ObjectPool<EventRegisterHelper>.S.Allocate();
                m_EventRegisterHelper.eventSystem = EventSystem.S;
            }

            m_EventRegisterHelper.Register(key, callback);
        }

        public void UnRegisterAllEvent()
        {
            if (m_EventRegisterHelper == null)
            {
                return;
            }

            ObjectPool<EventRegisterHelper>.S.Recycle(m_EventRegisterHelper);
            m_EventRegisterHelper = null;
        }

        public void SendViewEvent(ViewEvent key, params object[] args)
        {
            UIMgr.S.uiEventSystem.Send(GetParentPanelID(), key, args);
        }

        public void CloseSelfPanel()
        {
            SendViewEvent(ViewEvent.Action_ClosePanel);
        }

        public void HideSelfPanel()
        {
            SendViewEvent(ViewEvent.Action_HidePanel);
        }

        public void ShowSelfPanel()
        {
            SendViewEvent(ViewEvent.Action_ShowPanel);
        }

        //Only Can Find Attach singleton page
        public AbstractPage GetPage<T>(T uiID) where T : IConvertible
        {
            return UIMgr.S.FindPanelPage(GetParentPanelID(), uiID);
        }

#endregion

#region 生命周期
        protected void Awake()
        {
            if (!m_HasInitUI)
            {
                m_HasInitUI = true;
                RegisterParentPanelEvent();
                ERunner.Run(OnUIInit);
            }
        }

        protected void OnDestroy()
        {
            if (UIMgr.isApplicationQuit)
            {
                return;
            }

            ClosePage();

            int panelID = GetParentPanelID();
            if (panelID > 0)
            {
                UIMgr.S.uiEventSystem.UnRegister(panelID, OnParentPanelEvent);
            }

            BeforDestroy();

            if (m_GlobalResLoader != null)
            {
                m_GlobalResLoader.Recycle2Cache();
                m_GlobalResLoader = null;
            }

            m_ParentPage = null;
        }

        protected virtual void BeforDestroy()
        {

        }

#endregion
        //注册父Panel的顶层事件
        protected void RegisterParentPanelEvent()
        {
            int panelID = GetParentPanelID();
            if (panelID > 0)
            {
                UIMgr.S.uiEventSystem.Register(panelID, OnParentPanelEvent);
            }
        }

        protected void OnParentPanelEvent(int key, params object[] args)
        {
            if (args == null || args.Length <= 0)
            {
                return;
            }

            ViewEvent e = (ViewEvent)args[0];

            //默认事件已经处理了
            switch (e)
            {
                case ViewEvent.OnPanelClose:
                    ClosePage();
                    break;
                case ViewEvent.OnPanelOpen:
                    OpenPage();
                    break;
                case ViewEvent.OnParamUpdate:
                    ERunner.Run(OnParamUpdate);
                    break;
                case ViewEvent.OnSortingLayerUpdate:
                    ERunner.Run(OnSortingLayerUpdate);
                    break;
                default:
                    break;
            }

            object[] newArgs = null;

            if (args.Length > 1)
            {
                newArgs = new object[args.Length - 1];
                for (int i = 0; i < newArgs.Length; ++i)
                {
                    newArgs[i] = args[i + 1];
                }
            }

            ERunner.Run(OnViewEvent, e, newArgs);
        }

        private void ClosePage()
        {
            if (m_HasOpen)
            {
                m_HasOpen = false;
                UnRegisterAllEvent();
                ERunner.Run(OnClose);
            }
        }

        public void OpenPage()
        {
            if (!m_HasOpen)
            {
                m_HasOpen = true;
                ERunner.Run(OnOpen);
            }
        }

        // 挂载某个UI子面板到节点上，确保parent 确实是该面板的节点
        protected void AttachPage<T>(T uiID, Transform parent, Action<AbstractPage> listener = null, bool singleton = true) where T : IConvertible
        {
            UIMgr.S.AttachPage(GetParentPanelID(), uiID, parent, listener, singleton);
        }

        protected void OpenDependPanel<T>(T uiID, params object[] args) where T : IConvertible
        {
            UIMgr.S.OpenDependPanel(GetParentPanelID(), uiID, null, args);
        }

        protected void CloseDependPanel<T>(T uiID) where T : IConvertible
        {
            UIMgr.S.CloseDependPanel(GetParentPanelID(), uiID);
        }

        private Sprite FindSpriteFromParent(string spriteName, bool global)
        {
            if (m_ParentPage == null)
            {
                return null;
            }

            return m_ParentPage.FindSprite(spriteName, global);
        }

        #region 子类需重载
        //初始化面板
        protected virtual void OnUIInit()
        {

        }

        /************************************************************************/
        /* 面板开启进入，可重入界面会多次进入*/
        /************************************************************************/
        protected virtual void OnOpen()
        {

        }

        //面板被关闭的时候进入
        protected virtual void OnClose()
        {

        }

        protected virtual void OnSortingLayerUpdate()
        {

        }

        protected virtual void OnParamUpdate()
        {

        }

        protected virtual void OnViewEvent(ViewEvent e, object[] args)
        {

        }
#endregion

    }
}
