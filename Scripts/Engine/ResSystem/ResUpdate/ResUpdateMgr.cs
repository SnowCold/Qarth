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

namespace Qarth
{
    public class ResUpdateMgr : TSingleton<ResUpdateMgr>
    {
        public const int RES_UPDATE_EVENT_FINISH = 1;

        private class ResPackageWrap
        {
            private ResPackage m_Package;
            private Action<ResPackage, int> m_Listener;
            private ResPackageHandler m_Handler;

            public ResPackageWrap(ResPackage package, Action<ResPackage, int> l)
            {
                m_Package = package;
                m_Listener = l;
            }

            public ResPackageHandler handler
            {
                get
                {
                    if (m_Handler == null)
                    {
                        m_Handler = new ResPackageHandler(m_Package);
                    }

                    return m_Handler;
                }
            }

            public void FireEvent(int eventID)
            {
                if (m_Listener == null)
                {
                    return;
                }

                m_Listener(m_Package, eventID);
            }
        }

        private List<ResPackageWrap> m_UpdatePackageList;
        private Action<bool> m_OnCheckUpdateListener;
        private Action<bool> m_OnUpdateListener;
        private int m_CheckFinishCount = 0;
        private int m_UpdateIndex = -1;
        private bool m_IsUpdateing = false;

        public bool isUpdateing
        {
            get { return m_IsUpdateing; }
        }

        public void Clear()
        {
            if (m_UpdatePackageList != null)
            {
                m_UpdatePackageList.Clear();
            }

            m_OnCheckUpdateListener = null;
            m_OnUpdateListener = null;
        }

        public ResPackageHandler currentUpdateHandler
        {
            get
            {
                if (!m_IsUpdateing)
                {
                    return null;
                }

                if (m_UpdateIndex < 0 || m_UpdateIndex >= m_UpdatePackageList.Count)
                {
                    return null;
                }

                return m_UpdatePackageList[m_UpdateIndex].handler;
            }
        }

        public float needUpdateFileSize
        {
            get
            {
                if (m_UpdatePackageList == null || m_UpdatePackageList.Count == 0)
                {
                    return 0;
                }

                float size = 0;
                for (int i = 0; i < m_UpdatePackageList.Count; ++i)
                {
                    size += m_UpdatePackageList[i].handler.needUpdateFileSize;
                }
                return size;
            }
        }

        public float alreadyUpdateFileSize
        {
            get
            {
                if (m_UpdatePackageList == null || m_UpdatePackageList.Count == 0)
                {
                    return 0;
                }

                float size = 0;
                for (int i = 0; i < m_UpdatePackageList.Count; ++i)
                {
                    size += m_UpdatePackageList[i].handler.alreadyUpdateFileSize;
                }
                return size + WWWDownloader.S.alreadyDownloadSize;
            }
        }

        public int alreadyUpdateFileCount
        {
            get
            {
                if (m_UpdatePackageList == null || m_UpdatePackageList.Count == 0)
                {
                    return 0;
                }

                int size = 0;
                for (int i = 0; i < m_UpdatePackageList.Count; ++i)
                {
                    size += m_UpdatePackageList[i].handler.alreadyUpdateFileCount;
                }
                return size;
            }
        }

        public int needUpdateFileCount
        {
            get
            {
                if (m_UpdatePackageList == null || m_UpdatePackageList.Count == 0)
                {
                    return 0;
                }

                int size = 0;
                for (int i = 0; i < m_UpdatePackageList.Count; ++i)
                {
                    size += m_UpdatePackageList[i].handler.needUpdateFileCount;
                }
                return size;
            }
        }

        public void AddPackage(ResPackage package, Action<ResPackage, int> l)
        {
            if (package == null)
            {
                return;
            }

            if (m_UpdatePackageList == null)
            {
                m_UpdatePackageList = new List<ResPackageWrap>();
            }

            m_UpdatePackageList.Add(new ResPackageWrap(package, l));
        }

        public void CheckUpdate(Action<bool> checkResultListener)
        {
            if (m_IsUpdateing)
            {
                return;
            }

            m_IsUpdateing = true;
            m_OnCheckUpdateListener = checkResultListener;

            if (m_UpdatePackageList == null || m_UpdatePackageList.Count == 0)
            {
                FireCheckUpdateEvent();
                return;
            }

            m_CheckFinishCount = 0;

            for (int i = 0; i < m_UpdatePackageList.Count; ++i)
            {
                m_UpdatePackageList[i].handler.CheckUpdateList(OnPackageUpdateCheckResult);
            }
        }

        public void StartUpdate(Action<bool> updateResultListener)
        {
            if (m_IsUpdateing)
            {
                return;
            }

            m_IsUpdateing = true;
            m_OnUpdateListener = updateResultListener;

            if (!needUpdate)
            {
                FireUpdateEvent();
                return;
            }

            m_UpdateIndex = -1;

            TryStartNextUpdate();
        }

        private void TryStartNextUpdate()
        {
            if (m_UpdatePackageList == null || m_UpdatePackageList.Count == 0)
            {
                FireUpdateEvent();
                return;
            }

            while(++m_UpdateIndex < m_UpdatePackageList.Count)
            {
                ResPackageWrap wrap = m_UpdatePackageList[m_UpdateIndex];
                if (!wrap.handler.needUpdate)
                {
                    continue;
                }

                wrap.handler.StartUpdate(OnPackageUpdateResult);
                break;
            }

            if (m_UpdateIndex >= m_UpdatePackageList.Count)
            {
                FireUpdateEvent();
            }
        }

        public bool needUpdate
        {
            get
            {
                if (m_UpdatePackageList == null || m_UpdatePackageList.Count == 0)
                {
                    return false;
                }

                for (int i = 0; i < m_UpdatePackageList.Count; ++i)
                {
                    if (m_UpdatePackageList[i].handler.needUpdate)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private void OnPackageUpdateCheckResult(ResPackageHandler handler)
        {
            ++m_CheckFinishCount;
            if (m_CheckFinishCount == m_UpdatePackageList.Count)
            {
                FireCheckUpdateEvent();
            }
        }

        private void OnPackageUpdateResult(ResPackageHandler handler)
        {
            ResPackageWrap wrap = m_UpdatePackageList[m_UpdateIndex];
            wrap.FireEvent(RES_UPDATE_EVENT_FINISH);
            TryStartNextUpdate();
        }

        private void FireCheckUpdateEvent()
        {
            m_IsUpdateing = false;
            if (m_OnCheckUpdateListener != null)
            {
                m_OnCheckUpdateListener(needUpdate);
                m_OnCheckUpdateListener = null;
            }
        }

        private void FireUpdateEvent()
        {
            m_IsUpdateing = false;
            if (m_OnUpdateListener != null)
            {
                m_OnUpdateListener(!needUpdate);
                m_OnUpdateListener = null;
            }
        }

    }
}
