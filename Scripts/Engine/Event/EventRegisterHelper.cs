//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System;
using System.Collections;

using System.Collections.Generic;


namespace Qarth
{

    public class EventRegisterHelper : ICacheAble
    {
        public void OnCacheReset()
        {
            Reset();
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

        #region 内部类
        private class EventRegisterUnit
        {
            int m_Key;
            OnEvent m_Listener;

            public EventRegisterUnit(int key, OnEvent l)
            {
                m_Key = key;
                m_Listener = l;
            }

            public void UnRegister(EventSystem es)
            {
                es.UnRegister(m_Key, m_Listener);
            }
        }
        #endregion

        private List<EventRegisterUnit> m_DataList;
        private EventSystem             m_EventSystem;
        private bool                    m_CacheFlag = false;

        public EventRegisterHelper()
        {

        }

        public EventRegisterHelper(EventSystem system)
        {
            m_EventSystem = system;
        }

        public EventSystem eventSystem
        {
            get { return m_EventSystem; }
            set { m_EventSystem = value; }
        }

        public void Reset()
        {
            UnRegisterAll();
            m_EventSystem = null;
        }

        public void Register<T>(T key, OnEvent l) where T : IConvertible
        {
            if (m_DataList == null)
            {
                m_DataList = new List<EventRegisterUnit>();
            }

            if (m_EventSystem.Register(key, l))
            {
                m_DataList.Add(new EventRegisterUnit(key.ToInt32(null), l));
            }
        }

        public void UnRegisterAll()
        {
            if (m_DataList == null)
            {
                return;
            }

            if (m_DataList.Count == 0)
            {
                return;
            }

            for (int i = m_DataList.Count - 1; i >= 0; --i)
            {
                m_DataList[i].UnRegister(m_EventSystem);
            }

            m_DataList.Clear();
        }

    }

}


