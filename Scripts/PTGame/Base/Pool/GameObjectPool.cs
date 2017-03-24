using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class GameObjectPool
    {
        private string                  m_PoolName;
        private Transform               m_Root; 
        private GameObject              m_Prefab;
        private int                     m_MaxCount = 10;
        private IGameObjectPoolStrategy m_Strategy;

        private Stack<GameObject>       m_CacheStack;

        //maxCount <= 0 不限制大小
        public void InitPool(string poolName, Transform parent, GameObject prefab, int maxCount, int initCount, IGameObjectPoolStrategy strategy = null)
        {
            if (m_Prefab != null)
            {
                Log.e("Already Init GameObjectPool.");
                return;
            }

            if (prefab == null)
            {
                return;
            }

            m_Strategy = strategy;

            if (m_Strategy == null)
            {
                m_Strategy = DefaultPoolStrategy.S;
            }

            m_PoolName = poolName;

            GameObject container = new GameObject(m_PoolName);
            m_Root = container.transform;
            m_Root.SetParent(parent);

            m_Strategy.ProcessContainer(container);

            m_Prefab = prefab;
            //Prefab 自己玩去
            //m_Prefab.transform.SetParent(m_Root, true);

            if (maxCount > 0)
            {
                m_MaxCount = Mathf.Max(1, maxCount);
                initCount = Mathf.Min(maxCount, initCount);
            }
            else
            {
                m_MaxCount = maxCount;
            }

            if (initCount > 0)
            {
                for (int i = 0; i < initCount; ++i)
                {
                    Recycle(CreateNewGameObject());
                }
            }
        }

        public int currentCount
        {
            get
            {
                if (m_CacheStack == null)
                {
                    return 0;
                }

                return m_CacheStack.Count;
            }
        }

        public int maxCacheCount
        {
            get { return m_MaxCount; }
            set
            {
                m_MaxCount = value;

                if (m_CacheStack != null)
                {
                    if (m_MaxCount < m_CacheStack.Count)
                    {
                        int removeCount = m_MaxCount - m_CacheStack.Count;
                        while (removeCount > 0)
                        {
                            m_CacheStack.Pop();
                            --removeCount;
                        }
                    }
                }
            }
        }

        public void RemoveAllObject(bool destroySelf, bool destroyPrefab)
        {
            if (destroyPrefab && m_Prefab != null)
            {
                GameObject.Destroy(m_Prefab);
                m_Prefab = null;
            }

            if (destroySelf)
            {
                if (m_Root != null)
                {
                    GameObject.Destroy(m_Root.gameObject);
                    m_Root = null;
                }

                if (m_CacheStack != null)
                {
                    m_CacheStack.Clear();
                }

                return;
            }

            if (m_CacheStack == null)
            {
                return;
            }

            if (m_CacheStack.Count == 0)
            {
                return;
            }

            GameObject next = null;
            while(m_CacheStack.Count > 0)
            {
                next = m_CacheStack.Pop();
                GameObject.Destroy(next);
            }

        }

        public GameObject Allocate()
        {
            GameObject result;
            if (m_CacheStack == null || m_CacheStack.Count == 0)
            {
                if (m_Prefab == null)
                {
                    Log.e("GameObjectPool Not Init With Prefab.");
                    return null;
                }
                result = CreateNewGameObject();
            }
            else
            {
                result = m_CacheStack.Pop();
            }

            m_Strategy.OnAllocate(result);

            return result;
        }

        public void Recycle(GameObject t)
        {
            if (t == null)
            {
                return;
            }

            if (m_CacheStack == null)
            {
                m_CacheStack = new System.Collections.Generic.Stack<GameObject>();
            }
            else if (m_MaxCount > 0)
            {
                if (m_CacheStack.Count >= m_MaxCount)
                {
                    PoolObjectComponent comItem = t.GetComponent<PoolObjectComponent>();
                    if (comItem != null)
                    {
                        comItem.OnReset2Cache();
                    }
                    GameObject.Destroy(t);
                    return;
                }
            }

            t.transform.SetParent(m_Root, true);

            m_Strategy.OnRecycle(t);

            m_CacheStack.Push(t);

            PoolObjectComponent item = t.GetComponent<PoolObjectComponent>();
            if (item != null)
            {
                item.OnReset2Cache();
            }

            //是否要处理Active False
        }

        private GameObject CreateNewGameObject()
        {
            if (m_Prefab == null)
            {
                return null;
            }
            GameObject obj = GameObject.Instantiate(m_Prefab, m_Root, true) as GameObject;
            //obj.transform.SetParent(m_Root, true);
            obj.name = m_PoolName;
            return obj;
        }
    }
}
