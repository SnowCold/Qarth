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

    #region 事件接口
    public delegate void OnEvent(int key, params object[] param);
    #endregion

    public class EventSystem : TSingleton<EventSystem>, ICacheAble
    {
        private bool        m_CacheFlag = false;
        private Dictionary<int, ListenerWrap> m_AllListenerMap = new Dictionary<int, ListenerWrap>(50);

        public EventSystem()
        {
            
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

        #region 内部结构
        private class ListenerWrap
        {
            private LinkedList<OnEvent>     m_EventList;

            public bool Fire(int key, params object[] param)
            {
                if (m_EventList == null)
                {
                    return false;
                }

                LinkedListNode<OnEvent> next = m_EventList.First;
                OnEvent call = null;
                LinkedListNode<OnEvent> nextCache = null;

                while (next != null)
                {
                    call = next.Value;
                    nextCache = next.Next;
                    call(key, param);

                    //1.该事件的回调删除了自己OK 2.该事件的回调添加了新回调OK， 3.该事件删除了其它回调(被删除的回调可能有回调，可能没有)
                    next = (next.Next == null) ? nextCache : next.Next;
                }

                return true;
            }

            public bool Add(OnEvent listener)
            {
                if (m_EventList == null)
                {
                    m_EventList = new LinkedList<OnEvent>();
                }

                if (m_EventList.Contains(listener))
                {
                    return false;
                }

                m_EventList.AddLast(listener);
                return true;
            }

            public void Remove(OnEvent listener)
            {
                if (m_EventList == null)
                {
                    return;
                }

                m_EventList.Remove(listener);
            }
        }
        #endregion

        #region 功能函数

        public bool Register<T>(T key, OnEvent fun) where T : IConvertible
        {
            int kv = key.ToInt32(null);
            ListenerWrap wrap;
            if (!m_AllListenerMap.TryGetValue(kv, out wrap))
            {
                wrap = new ListenerWrap();
                m_AllListenerMap.Add(kv, wrap);
            }

            if (wrap.Add(fun))
            {
                return true;
            }

            Log.w("Already Register Same Event:" + key);
            return false;
        }

        public void UnRegister<T>(T key, OnEvent fun) where T : IConvertible
        {
            ListenerWrap wrap;
            if (m_AllListenerMap.TryGetValue(key.ToInt32(null), out wrap))
            {
                wrap.Remove(fun);
            }
        }

        public bool Send<T>(T key, params object[] param) where T : IConvertible
        {
            int kv = key.ToInt32(null);
            ListenerWrap wrap;
            if (m_AllListenerMap.TryGetValue(kv, out wrap))
            {
                return wrap.Fire(kv, param);
            }
            return false;
        }

        private static object[] s_EmptyParam = new object[0];

        public bool Send<T>(T key) where T : IConvertible
        {
            int kv = key.ToInt32(null);
            ListenerWrap wrap;
            if (m_AllListenerMap.TryGetValue(kv, out wrap))
            {
                return wrap.Fire(kv, s_EmptyParam);
            }
            return false;
        }

        public void OnCacheReset()
        {
            m_AllListenerMap.Clear();
        }

        #endregion

    }
}
