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
    public class KeyboardInputter : IInputter
    {
#region 按键监控单元类
        private class KeyCodeMonitor
        {
            private Run                     m_BeginDelegate;
            private Run                     m_EndDelegate;
            private Run                     m_PrecessingDelegate;
            private bool                    m_IsPrecessing;
            private KeyCode                 m_KeyCode;

            public KeyCodeMonitor(KeyCode code, Run begin, Run end, Run precessing)
            {
                m_KeyCode = code;
                m_BeginDelegate = begin;
                m_EndDelegate = end;
                m_PrecessingDelegate = precessing;
            }

            public void LateUpdate()
            {
                if (Input.GetKey(m_KeyCode))
                {
                    if (m_IsPrecessing)
                    {
                        if (m_PrecessingDelegate != null)
                        {
                            m_PrecessingDelegate();
                        }
                    }
                    else
                    {
                        m_IsPrecessing = true;
                        if (m_BeginDelegate != null)
                        {
                            m_BeginDelegate();
                        }
                    }
                }
                else
                {
                    if (m_IsPrecessing)
                    {
                        m_IsPrecessing = false;
                        if (m_EndDelegate != null)
                        {
                            m_EndDelegate();
                        }
                    }
                }
            }
        }
#endregion

#region
        private List<KeyCodeMonitor>    m_MonitorList;
        private bool                    m_IsEnable = true;

        public bool isEnabled
        {
            get { return m_IsEnable; }
            set { m_IsEnable = value; }
        }

        public void RegisterKeyCodeMonitor(KeyCode code, Run begin, Run end, Run press)
        {
            AddKeyCodeMonitor(code, begin, end, press);
        }

        public void Release()
        {
            m_MonitorList = null;
        }

        public void LateUpdate()
        {
            if (!m_IsEnable)
            {
                return;
            }

            if (m_MonitorList == null || m_MonitorList.Count == 0)
            {
                return;
            }

            for (int i = m_MonitorList.Count - 1; i >= 0; --i)
            {
                m_MonitorList[i].LateUpdate();
            }
        }
#endregion

        private void AddKeyCodeMonitor(KeyCode code, Run begin, Run end, Run processing)
        {
            KeyCodeMonitor monitor = new KeyCodeMonitor(code, begin, end, processing);
            if (m_MonitorList == null)
            {
                m_MonitorList = new List<KeyCodeMonitor>();
            }

            m_MonitorList.Add(monitor);
        }
    }
}
