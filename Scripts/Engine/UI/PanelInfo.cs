//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Qarth
{
    public partial class UIMgr
    {
        class PanelInfo : ICacheAble
        {
            public class OpenParam : ICacheAble
            {
                public int MasterID;
                public object[] Args;
                private bool m_CacheFlag = false;

                public bool cacheFlag
                {
                    get
                    {
                        return m_CacheFlag;
                    }

                    set
                    {
                        m_CacheFlag = value;
                    }
                }

                public OpenParam()
                {

                }

                public OpenParam Set(int masterID, object[] args)
                {
                    MasterID = masterID;
                    Args = args;
                    return this;
                }

                public void OnCacheReset()
                {
                    MasterID = -1;
                    Args = null;
                }
            }

            class PageWrap
            {
                private int m_UIID;
                private AbstractPage m_Page;
                private Action<AbstractPage> m_Listeners;

                public int uiID
                {
                    get { return m_UIID; }
                }

                public AbstractPage page
                {
                    get { return m_Page; }
                    set
                    {
                        m_Page = value;
                        if (m_Listeners != null)
                        {
                            m_Listeners(m_Page);
                            m_Listeners = null;
                        }
                    }
                }

                public void AddListener(Action<AbstractPage> listener)
                {
                    if (m_Page != null)
                    {
                        listener(m_Page);
                        return;
                    }

                    m_Listeners += listener;
                }

                public PageWrap(int uiID)
                {
                    m_UIID = uiID;
                }
            }

            private enum ePanelState : byte
            {
                UnInit,
                Loading,
                Ready,
                UnVisible,
                OnlyVisible,
                Interactive,
            }

            private AbstractPanel       m_Panel;
            private int                 m_PanelID;
            private int                 m_UIID;
            private int                 m_NextMaster = -1;
            private int                 m_CurrentMaster = -1;
            private List<OpenParam>     m_OpenInfoList;
            private int                 m_SortIndex = 0;
            private Vector3             m_DefaultPos;
            private ePanelState         m_PanelState = ePanelState.UnInit;
            private bool                m_CacheFlag = false;
            private bool                m_CustomVisibleFlag = true;

            private ResLoader           m_ResLoader;
            private Action<AbstractPanel> m_OpenListeners;
            private List<PageWrap>      m_PageWrapList;

            public void Set(int uiID, int panelID)
            {
                m_PanelID = panelID;
                m_UIID = uiID;
            }

            #region 缓存接口
            public void OnCacheReset()
            {
                m_Panel = null;
                m_NextMaster = -1;
                m_PanelID = -1;
                m_UIID = -1;
                m_SortIndex = -1;
                m_PanelState = ePanelState.UnInit;
                m_OpenListeners = null;
                m_CustomVisibleFlag = true;

                m_ResLoader.Recycle2Cache();
                m_ResLoader = null;

                if (m_PageWrapList != null)
                {
                    m_PageWrapList.Clear();
                }

                if (m_OpenInfoList != null)
                {
                    m_OpenInfoList.Clear();
                }
            }

            public bool cacheFlag
            {
                get
                {
                    return m_CacheFlag;
                }

                set
                {
                    m_CacheFlag = value;
                }
            }
            #endregion

            public bool customVisibleFlag
            {
                get { return m_CustomVisibleFlag; }
                set
                {
                    m_CustomVisibleFlag = value;

                    if (m_Panel != null)
                    {
                        m_Panel.customVisibleFlag = value;
                    }
                }
            }

            public int panelID
            {
                get { return m_PanelID; }
            }

            public int uiID
            {
                get { return m_UIID; }
            }

            public int sortIndex
            {
                get
                {
                    if (m_Panel != null && m_Panel.sortIndex > 0)
                    {
                        return m_Panel.sortIndex;
                    }

                    return m_SortIndex;
                }

                set
                {
                    m_SortIndex = value;
                }
            }

            public int nextMaster
            {
                get
                {
                    return m_NextMaster;
                }
            }

            public int currentMaster
            {
                get
                {
                    return m_CurrentMaster;
                }
            }

            public bool isReady
            {
                get { return m_Panel != null; }
            }

            public void SetSiblingIndexAndSortingOrder(int siblingIndex, int sortingOrder)
            {
                if (m_Panel == null)
                {
                    return;
                }

                m_Panel.SetSiblingIndexAndSortingOrder(siblingIndex, sortingOrder);
            }

            public void SetSortingOrderDirty()
            {
                if (m_Panel == null)
                {
                    return;
                }

                m_Panel.SetSortingOrderDirty();
            }

            public int maxSortingOrder
            {
                get
                {
                    if (m_Panel == null)
                    {
                        return -1;
                    }

                    return m_Panel.maxSortingOrder;
                }
            }

            public void AddOpenCallback(Action<AbstractPanel> listener)
            {
                if (listener == null)
                {
                    return;
                }

                if (m_Panel != null)
                {
                    listener(m_Panel);
                    return;
                }

                m_OpenListeners += listener;
            }

            private void CallOpenCallbck()
            {
                if (m_OpenListeners != null)
                {
                    m_OpenListeners(m_Panel);
                    m_OpenListeners = null;
                }
            }

            public void SetActive(bool visible, bool interactive)
            {
                if (m_Panel == null)
                {
                    return;
                }

                visible = (visible && m_Panel.customVisibleFlag);

                ePanelState nextState = ePanelState.UnInit;
                if (visible)
                {
                    if (interactive)
                    {
                        nextState = ePanelState.Interactive;
                    }
                    else
                    {
                        nextState = ePanelState.OnlyVisible;
                    }
                }
                else
                {
                    nextState = ePanelState.UnVisible;
                }

                if (nextState == m_PanelState)
                {
                    return;
                }

                m_PanelState = nextState;

                //1.使用LayerMask 模式设置
                if (visible)
                {
                    if (!m_Panel.gameObject.activeSelf)
                    {
                        m_Panel.gameObject.SetActive(true);
                    }

                    UITools.SetGameObjectLayer(m_Panel.gameObject, LayerDefine.LAYER_UI);
                    if (interactive)
                    {
                        UITools.SetGraphicRaycasterState(m_Panel.gameObject, true);
                    }
                    else
                    {
                        UITools.SetGraphicRaycasterState(m_Panel.gameObject, false);
                    }

                    Transform tr = m_Panel.transform;
                    UIMgr.S.uiRoot.ReleaseFreePos(tr.localPosition);
                    tr.localPosition = m_DefaultPos;
                    m_Panel.OnBecomeVisible();
                }
                else
                {
                    UITools.SetGameObjectLayer(m_Panel.gameObject, LayerDefine.LAYER_HIDE_UI);
                    m_Panel.gameObject.transform.localPosition = UIMgr.S.uiRoot.RequireNextFreePos();
                    m_Panel.OnBecomeHide();
                }

                //2. 使用GameObject开关模式,经过测试每次切换的开销比较大，但是在编辑器模式下方便编辑调试
                /*
                if (m_Panel.gameObject.activeSelf != state)
                {
                    m_Panel.gameObject.SetActive(state);
                }
                */

                //3.使用Canvas 组件的开关模式，该模式和1类似，时间上和1类似

                /*
                if (state)
                {
                    if (!m_Panel.gameObject.activeSelf)
                    {
                        m_Panel.gameObject.SetActive(true);
                    }

                    UITools.SetCanvasState(m_Panel.gameObject, true);
                    Transform tr = m_Panel.transform;
                    UIMgr.S.uiRoot.ReleaseFreePos(tr.localPosition);
                    tr.localPosition = m_DefaultPos;
                }
                else
                {
                    UITools.SetCanvasState(m_Panel.gameObject, false);
                    m_Panel.gameObject.transform.localPosition = UIMgr.S.uiRoot.RequireNextFreePos();
                }
                */
            }

            public int hideMask
            {
                get
                {
                    if (m_Panel == null || !m_CustomVisibleFlag)
                    {
                        return 0;
                    }

                    return (int)m_Panel.hideMask;
                }
            }

            public AbstractPanel abstractPanel
            {
                get { return m_Panel; }
                set
                {
                    m_Panel = value;
                    if (m_Panel != null)
                    {
                        m_Panel.panelID = m_PanelID;
                        m_Panel.uiID = m_UIID;
                        m_DefaultPos = m_Panel.transform.localPosition;
                    }
                }
            }

            public void AddMaster(int master, params object[] args)
            {
                if (master <= 0)
                {
                    return;
                }

                if (m_OpenInfoList == null)
                {
                    m_OpenInfoList = new List<OpenParam>();
                }

                for (int i = m_OpenInfoList.Count - 1; i >= 0; --i)
                {
                    if (m_OpenInfoList[i].MasterID == master)
                    {
                        ObjectPool<OpenParam>.S.Recycle(m_OpenInfoList[i]);
                        m_OpenInfoList.RemoveAt(i);
                        break;
                    }
                }

                m_OpenInfoList.Add(ObjectPool<OpenParam>.S.Allocate().Set(master, args));

                SelectNextMaster();
            }

            public void RemoveMaster(int panelID)
            {
                if (panelID <= 0)
                {
                    return;
                }

                //如果是删除对自己的依赖，那么先删除对当前依赖的依赖
                if (panelID == m_PanelID)
                {
                    if (panelID != m_NextMaster)
                    {
                        for (int i = m_OpenInfoList.Count - 1; i >= 0; --i)
                        {
                            if (m_OpenInfoList[i].MasterID == m_NextMaster)
                            {
                                ObjectPool<OpenParam>.S.Recycle(m_OpenInfoList[i]);
                                m_OpenInfoList.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }

                if (m_OpenInfoList != null)
                {
                    for (int i = m_OpenInfoList.Count - 1; i >= 0; --i)
                    {
                        if (m_OpenInfoList[i].MasterID == panelID)
                        {
                            ObjectPool<OpenParam>.S.Recycle(m_OpenInfoList[i]);
                            m_OpenInfoList.RemoveAt(i);
                            break;
                        }
                    }
                }

                if (m_NextMaster == panelID || panelID == m_PanelID)
                {
                    SelectNextMaster();
                }
            }

            private void SelectNextMaster()
            {
                m_CurrentMaster = -1;
                m_NextMaster = -1;

                if (m_OpenInfoList == null)
                {
                    m_SortIndex = -1;
                    return;
                }

                if (m_OpenInfoList.Count == 0)
                {
                    m_SortIndex = -1;
                    return;
                }

                int maxIndex = -1;

                for (int i = m_OpenInfoList.Count - 1; i >= 0; --i)
                {
                    PanelInfo info = UIMgr.S.FindPanelInfoByPanelID(m_OpenInfoList[i].MasterID);
                    if (info == null)
                    {
                        ObjectPool<OpenParam>.S.Recycle(m_OpenInfoList[i]);
                        m_OpenInfoList.RemoveAt(i);
                        continue;
                    }

                    if (info.sortIndex > maxIndex)
                    {
                        maxIndex = info.sortIndex;
                        m_NextMaster = info.panelID;
                    }
                }

                if (m_NextMaster == m_PanelID)
                {
                    m_SortIndex = maxIndex;
                }
                else
                {
                    m_SortIndex = maxIndex + 1;
                }
            }

            private OpenParam GetOpenInfo(int master)
            {
                if (m_OpenInfoList == null)
                {
                    return null;
                }

                for (int i = m_OpenInfoList.Count - 1; i >= 0; --i)
                {
                    if (m_OpenInfoList[i].MasterID == master)
                    {
                        return m_OpenInfoList[i];
                    }
                }

                return null;
            }

            public void OpenPanel()
            {
                if (m_NextMaster == m_CurrentMaster)
                {
                    return;
                }

                if (m_NextMaster < 0)
                {
                    m_CurrentMaster = m_NextMaster;
                    return;
                }

                if (m_Panel == null)//这个条件的检测反而比较少
                {
                    return;
                }

                m_CurrentMaster = m_NextMaster;

                OpenParam info = GetOpenInfo(m_CurrentMaster);
                if (info == null)
                {
                    m_CurrentMaster = m_NextMaster = -1;
                    return;
                }

                ERunner.Run(m_Panel.OnPanelOpen, true, info.Args);
                
                ERunner.Run(CallOpenCallbck);
            }

            public void ClosePanel(bool destroy)
            {
                if (m_Panel == null)
                {
                    return;
                }

                m_Panel.OnPanelClose(destroy);
                if (destroy)
                {
                    GameObject.Destroy(m_Panel.gameObject);
                    m_Panel = null;
                }
                else
                {
                    SetActive(false, false);
                }
            }

            public void LoadPanelResAsync()
            {
                if (m_PanelState != ePanelState.UnInit)
                {
                    return;
                }

                m_PanelState = ePanelState.Loading;

                UIData data = UIDataTable.Get(m_UIID);

                if (data == null)
                {
                    return;
                }

                OpenParam info = GetOpenInfo(m_CurrentMaster);

                object[] args = null;

                if (info != null)
                {
                    args = info.Args;
                }

                if (m_ResLoader == null)
                {
                    m_ResLoader = ResLoader.Allocate("PanelInfo");
                }

                CollectDynamicResource(data, m_ResLoader, args);

                m_ResLoader.Add2Load(data.fullPath, (state, res) =>
                {
                    if (!state)
                    {
                        return;
                    }

                    OnPanelResLoadSuccess((GameObject)res.asset);
                });

                m_ResLoader.LoadSync();
            }

            public AbstractPage GetPage(int uiID)
            {
                PageWrap wrap = FindPageWrap(uiID);
                if (wrap == null)
                {
                    return null;
                }

                return wrap.page;
            }

            private PageWrap FindPageWrap(int uiID)
            {
                if (m_PageWrapList == null || m_PageWrapList.Count == 0)
                {
                    return null;
                }

                for (int i = m_PageWrapList.Count - 1; i >= 0; --i)
                {
                    if (m_PageWrapList[i].uiID == uiID)
                    {
                        return m_PageWrapList[i];
                    }
                }

                return null;
            }

            public void LoadPageResAsync(int uiID, Transform parent, bool singleton, Action<AbstractPage> listener)
            {
                UIData data = UIDataTable.Get(uiID);
                if (data == null)
                {
                    Log.e("Failed to load UI, Not Find UIData For:" + uiID);
                    return;
                }

                if (singleton)
                {
                    if (m_PageWrapList == null)
                    {
                        m_PageWrapList = new List<PageWrap>();
                    }

                    PageWrap wrap = FindPageWrap(uiID);

                    if (wrap != null)
                    {
                        wrap.AddListener(listener);
                        return;
                    }
                    else
                    {
                        wrap = new PageWrap(uiID);
                        wrap.AddListener(listener);

                        m_PageWrapList.Add(wrap);
                    }
                }

                if (m_ResLoader == null)
                {
                    m_ResLoader = ResLoader.Allocate("PanelInfo");
                }

                CollectDynamicResource(data, m_ResLoader);

                m_ResLoader.Add2Load(data.fullPath, (state, res) =>
                {
                    if (!state)
                    {
                        return;
                    }

                    OnPageResLoadSuccess(uiID, parent, (GameObject)res.asset, singleton, listener);
                });

                m_ResLoader.LoadSync();
            }

            private void OnPageResLoadSuccess(int uiID, Transform parent, GameObject prefab, bool singleton, Action<AbstractPage> listener)
            {
                if (m_NextMaster < 0)
                {
                    Log.i("PanelInfo Already Close，But Still Call OnPageResLoadSuccess");
                    return;
                }

                if (prefab == null)
                {
                    Log.e(string.Format("Failed to Attach Page!"));
                    return;
                }

                GameObject go = UIMgr.S.InstantiateUIPrefab(prefab);
                go.SetActive(false);
                UIMgr.S.InitOpenUIParam(go, parent);

                AbstractPage page = UIMgr.S.ProcessAttachPage(m_PanelID, uiID, go);

                if (singleton)
                {
                    PageWrap wrap = FindPageWrap(uiID);
                    if (wrap == null)
                    {
                        return;
                    }

                    wrap.page = page;
                }
                else
                {
                    if (listener != null)
                    {
                        listener(page);
                    }
                }
            }

            private void OnPanelResLoadSuccess(GameObject prefab)
            {
                if (m_NextMaster < 0)
                {
                    Log.i("PanelInfo Already Close，But Still Call OnPanelResLoadSuccess");
                    m_PanelState = ePanelState.UnInit;
                    return;
                }

                if (prefab == null)
                {
                    UIData data = UIDataTable.Get(m_UIID);
                    Log.e(string.Format("Failed to Load UI Prefab:{0} Path:{1}", m_UIID, data.fullPath));
                    return;
                }

                m_PanelState = ePanelState.Ready;

                GameObject go = UIMgr.S.InstantiateUIPrefab(prefab);
                go.SetActive(false);
                UIMgr.S.InitPanelParem(go);

                AbstractPanel panel = go.GetComponent<AbstractPanel>();

                if (panel == null)
                {
                    //该类型面板不纳入管理
                    go.gameObject.SetActive(true);
                    Log.e("Not Find Panel Class In Prefab For Panel:" + m_UIID);
                    //需要删除PanelInfo From Activity.
                    return;
                }

                panel.customVisibleFlag = m_CustomVisibleFlag;

                UIData panelData = UIDataTable.Get(m_UIID);

                if (panelData.panelClassType != null)
                {
                    if (panel.GetType() != panelData.panelClassType)
                    {
                        Log.e("ERROR: Prefab Bind C# Class Is Not Same With Define:" + panelData.name);
                    }
                }

                abstractPanel = panel;

                UIMgr.S.SetPanelSortingOrderDirty();
            }

        }
    }
}
