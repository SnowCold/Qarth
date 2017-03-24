using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    [TMonoSingletonAttribute("[Tools]/PoolMgr")]
    public class GameObjectPoolMgr : TMonoSingleton<GameObjectPoolMgr>
    {
        private GameObjectPoolGroup m_Group;

        public override void OnSingletonInit()
        {
            m_Group = new GameObjectPoolGroup(transform);
        }

        public GameObjectPoolGroup CreatePoolGroup(IGameObjectPoolStrategy strategy = null)
        {
            GameObjectPoolGroup group = new GameObjectPoolGroup(transform, strategy);
            return group;
        }

        public GameObjectPool CreatePool(string poolName, GameObject prefab, int maxCount, int initCount, IGameObjectPoolStrategy strategy = null)
        {
            GameObjectPool pool = new GameObjectPool();
            pool.InitPool(poolName, transform, prefab, maxCount, initCount, strategy);
            return pool;
        }

        public void AddPool(string poolName, GameObject prefab, int maxCount, int initCount)
        {
            m_Group.AddPool(poolName, prefab, maxCount, initCount);
        }

        public void RemovePool(string poolName, bool deletePrefab)
        {
            m_Group.RemovePool(poolName, deletePrefab);
        }

        public void RemoveAllPool(bool deletePrefab)
        {
            m_Group.RemoveAllPool(deletePrefab);
        }

        public GameObject Allocate(string poolName)
        {
            return m_Group.Allocate(poolName);
        }

        public void Recycle(string poolName, GameObject obj)
        {
            m_Group.Recycle(poolName, obj);
        }

        public void Recycle(GameObject obj)
        {
            m_Group.Recycle(obj.name, obj);
        }
    }
}
