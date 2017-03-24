using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
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

        public FSMState<T> GetState<K>(K k, bool forceCreate = false) where K : IConvertible
        {
            if (m_CreatorMap == null)
            {
                return null;
            }

            int key = k.ToInt32(null);

            StateCreator creator = null;
            if (!m_CreatorMap.TryGetValue(key, out creator))
            {
                Log.w("Not Find State Creator For: " + k);
                return null;
            }

            if (forceCreate || m_AlwaysCreate)
            {
                return creator();
            }

            FSMState<T> result = GetStateFromCache(key);
            if (result == null)
            {
                result = creator();
                AddState2Cache(key, result);
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

        private void AddState2Cache(int key, FSMState<T> state)
        {
            if (state == null)
            {
                return;
            }

            if (m_StateCache == null)
            {
                m_StateCache = new Dictionary<int, FSMState<T>>();
            }

            if (m_StateCache.ContainsKey(key))
            {
                return;
            }

            m_StateCache.Add(key, state);
        }
    }
}
