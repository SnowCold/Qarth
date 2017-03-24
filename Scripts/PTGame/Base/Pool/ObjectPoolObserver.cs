using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class ObjectPoolObserver : MonoBehaviour
    {
        [SerializeField]
        private List<string> m_PoolTypeArray = new List<string>();
        [SerializeField]
        private List<int> m_PoolCount = new List<int>();

        private List<CountObserverAble> m_ObserverObjectList = new List<CountObserverAble>();

        private void Awake()
        {
            //InvokeRepeating("UpdateState", 1, 1);
        }

        private void AddObserverObject(string name, CountObserverAble obj)
        {
            m_ObserverObjectList.Add(obj);
            m_PoolTypeArray.Add(name);
            m_PoolCount.Add(obj.currentCount);
        }

        private void UpdateState()
        {
            for (int i = 0; i < m_ObserverObjectList.Count; ++i)
            {
                m_PoolCount[i] = m_ObserverObjectList[i].currentCount;
            }
        }
    }
}
