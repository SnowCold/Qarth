using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class GameObjectPoolGroup
    {
        private Transform               m_Parent;
        private IGameObjectPoolStrategy m_Strategy;
        private Dictionary<string, GameObjectPool> m_PoolMap = new Dictionary<string, GameObjectPool>();

        public GameObjectPoolGroup(Transform parent, IGameObjectPoolStrategy strategy = null)
        {
            m_Parent = parent;
            m_Strategy = strategy;
        }

        public bool HasPool(string name)
        {
            return m_PoolMap.ContainsKey(name);
        }

        public void AddPool(string poolName, GameObject prefab, int maxCount, int initCount)
        {
            if (m_PoolMap.ContainsKey(poolName))
            {
                Log.w("Already Init GameObjectPool:" + poolName);
                return;
            }

            GameObjectPool cell = new GameObjectPool();
            cell.InitPool(poolName, m_Parent, prefab, maxCount, initCount, m_Strategy);
            m_PoolMap.Add(poolName, cell);
        }

        public void RemovePool(string poolName, bool destroyPrefab)
        {
            GameObjectPool pool = null;
            if(m_PoolMap.TryGetValue(poolName, out pool))
            {
                pool.RemoveAllObject(true, destroyPrefab);
                m_PoolMap.Remove(poolName);
            }
        }

        public void RemoveAllPool(bool destroyPrefab)
        {
            foreach (var pool in m_PoolMap)
            {
                pool.Value.RemoveAllObject(true, destroyPrefab);
            }

            m_PoolMap.Clear();
        }

        public GameObject Allocate(string poolName)
        {
            GameObjectPool cell = null;
            if (!m_PoolMap.TryGetValue(poolName, out cell))
            {
                Log.e("Allocate Not Find Pool:" + poolName);
                return null;
            }

            return cell.Allocate();
        }

        public void Recycle(string poolName, GameObject obj)
        {
            GameObjectPool cell = null;
            if (!m_PoolMap.TryGetValue(poolName, out cell))
            {
                Log.e("Recycle Not Find Pool:" + poolName);
                return;
            }

            cell.Recycle(obj);
        }

        public void Recycle(GameObject obj)
        {
            Recycle(obj.name, obj);
        }
    }

}
