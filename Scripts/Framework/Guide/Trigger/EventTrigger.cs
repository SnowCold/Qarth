using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qarth;

namespace Qarth
{
    public class EventTrigger : ITrigger
    {
        private string m_TypeName;
        private string m_EnumName;
        private bool m_IsReady = false;
        private int m_EventID = -1;
        private Action<bool, ITrigger> m_Listener;

        public void SetParam(object[] param)
        {
            m_TypeName = param[0] as string;
            m_EnumName = param[1] as string;

            try
            {
                Type enumType = Type.GetType(m_TypeName);
                m_EventID = (int)Enum.Parse(enumType, m_EnumName);
            }
            catch (Exception e)
            {
                Log.e(e);
            }

        }

        public void Start(Action<bool, ITrigger> l)
        {
            m_Listener = l;
            EventSystem.S.Register(m_EventID, OnEventListener);
        }

        public void Stop()
        {
            m_Listener = null;
            EventSystem.S.UnRegister(m_EventID, OnEventListener);
        }

        public bool isReady
        {
            get
            {
                return m_IsReady;
            }
        }

        private void OnEventListener(int key, params object[] args)
        {
            m_IsReady = true;
            if (m_Listener == null)
            {
                return;
            }

            if (isReady)
            {
                m_Listener(true, this);
            }
            else
            {
                m_Listener(false, this);
            }
        }
    }
}