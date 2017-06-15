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
    public class FSMStateFactory<T>
    {
        public delegate FSMState<T> StateCreator();

        private bool                            m_AlwaysCreate = false;
        private Dictionary<int, FSMState<T>>    m_StateCache;
        private Dictionary<int, StateCreator>   m_CreatorMap;

        public FSMStateFactory(bool alwaysCreate)
        {
            m_AlwaysCreate = alwaysCreate;
        }

        public void RegisterCreator(IConvertible key, StateCreator creator)
        {
            if (creator == null)
            {
                Log.e("Can not register null State Creator.");
                return;
            }

            if (m_CreatorMap == null)
            {
                m_CreatorMap = new Dictionary<int, StateCreator>();
            }

            m_CreatorMap.Add(key.ToInt32(null), creator);
        }
        
        public void UnRegisterCreator<K>(K key) where K : IConvertible
        {
            if (m_CreatorMap == null)
            {
                return;
            }

            if (m_CreatorMap.ContainsKey(key.ToInt32(null)))
            {
                m_CreatorMap.Remove(key.ToInt32(null));
            }
        }

        public void RegisterState<K>(K k, FSMState<T> state) where K : IConvertible
        {
            if (state == null)
            {
                return;
            }

            if (m_StateCache == null)
            {
                m_StateCache = new Dictionary<int, FSMState<T>>();
            }

            int key = k.ToInt32(null);

            if (m_StateCache.ContainsKey(key))
            {
                return;
            }

            m_StateCache.Add(key, state);
        }

        public FSMState<T> GetState<K>(K k, bool forceCreate = false) where K : IConvertible
        {
            int key = k.ToInt32(null);

            if (forceCreate || m_AlwaysCreate)
            {
                if (m_CreatorMap == null)
                {
                    return null;
                }

                StateCreator creator = null;
                if (!m_CreatorMap.TryGetValue(key, out creator))
                {
                    Log.w("Not Find State Creator For: " + k);
                    return null;
                }

                return creator();
            }

            FSMState<T> result = GetStateFromCache(key);
            if (result == null)
            {
                if (m_CreatorMap == null)
                {
                    return null;
                }

                StateCreator creator = null;
                if (!m_CreatorMap.TryGetValue(key, out creator))
                {
                    Log.w("Not Find State Creator For: " + k);
                    return null;
                }

                result = creator();
                RegisterState(key, result);
            }

            return result;
        }

        private FSMState<T> GetStateFromCache(int key)
        {
            if (m_StateCache == null)
            {
                return null;
            }

            FSMState<T> result = null;
            m_StateCache.TryGetValue(key, out result);

            return result;
        }
    }
}
