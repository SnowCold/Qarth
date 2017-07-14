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
    public class AbstractActor : MonoBehaviour
    {
        [SerializeField]
        private List<string>    m_ComsNameList = new List<string>();
        private bool            m_HasAwake = false;
        private bool            m_HasStart = false;
        private List<ICom>      m_ComponentList = new List<ICom>();
        private List<ICom>      m_UpdateComList = new List<ICom>();
        private EventSystem     m_EventSystem;

#region Public

        public EventSystem eventSystem
        {
            get
            {
                if (m_EventSystem == null)
                {
                    m_EventSystem = ObjectPool<EventSystem>.S.Allocate();
                }
                return m_EventSystem;
            }
        }

        public void AddCom(ICom com)
        {
            if (com == null)
            {
                return;
            }

            if (GetCom(com.GetType()) != null)
            {
                Log.w("Already Add Component:" + name);
                return;
            }

            //ComWrap wrap = new ComWrap(com);

            m_ComponentList.Add(com);

            m_ComsNameList.Add(com.GetType().Name);
            
            if (com is MonoBehaviour)
            {
            }
            else
            {
                m_UpdateComList.Add(com);
                m_UpdateComList.Sort(ComWrapComparison);
            }

            OnAddCom(com);

            if (m_HasAwake)
            {
                AwakeCom(com);
            }

            if (m_HasStart)
            {
                StartCom(com);
            }
        }

        public T AddCom<T>() where T : ICom, new()
        {
            T com = new T();
            AddCom(com);
            return com;
        }

        public T AddMonoCom<T>() where T : MonoBehaviour, ICom
        {
            T com = gameObject.AddComponent<T>();
            AddCom(com);
            return com;
        }

        public void RemoveCom<T>() where T : ICom
        {
            for (int i = m_ComponentList.Count - 1; i >= 0; --i)
            {
                if (m_ComponentList[i] is T)
                {
                    ICom com = m_ComponentList[i];

                    m_ComponentList.RemoveAt(i);
                    m_ComsNameList.RemoveAt(i);

                    if (com is MonoBehaviour)
                    {
                    }
                    else
                    {
                        m_UpdateComList.Remove(com);
                    }

                    OnRemoveCom(com);

                    DestroyCom(com);
                    return;
                }
            }
        }

        public void RemoveCom(ICom com)
        {
            for (int i = m_ComponentList.Count - 1; i >= 0; --i)
            {
                if (m_ComponentList[i] == com)
                {
                    m_ComponentList.RemoveAt(i);
                    m_ComsNameList.RemoveAt(i);

                    if (com is MonoBehaviour)
                    {
                    }
                    else
                    {
                        m_UpdateComList.Remove(com);
                    }

                    OnRemoveCom(com);

                    DestroyCom(com);
                    return;
                }
            }
        }

        public T GetCom<T>() where T : ICom
        {
            for (int i = m_ComponentList.Count - 1; i >= 0; --i)
            {
                if (m_ComponentList[i] is T)
                {
                    return (T)m_ComponentList[i];
                }
            }

            return default(T);
        }

#endregion


#region MonoBehaviour
        private void Awake()
        {
            InitMonoCom();

            OnActorAwake();

            for (int i = m_ComponentList.Count - 1; i >= 0; --i)
            {
                AwakeCom(m_ComponentList[i]);
            }

            m_HasAwake = true;
        }

        private void Start()
        {
            OnActorStart();
            for (int i = m_ComponentList.Count - 1; i >= 0; --i)
            {
                StartCom(m_ComponentList[i]);
            }
            m_HasStart = true;
        }

        //关于Update的优化方案，可以后续再做
        private void Update()
        {
            UpdateAllComs();
        }

        private void LateUpdate()
        {
            LateUpdateAllComs();
        }

        private void OnDestroy()
        {
            for (int i = m_ComponentList.Count - 1; i >= 0; --i)
            {
                DestroyCom(m_ComponentList[i]);
            }

            m_ComponentList.Clear();
            m_ComsNameList.Clear();
            m_UpdateComList.Clear();

            OnActorDestroy();
        }

        private void InitMonoCom()
        {
            var coms = GetComponents<AbstractMonoCom>();
            if (coms != null && coms.Length > 0)
            {
                for (int i = 0; i < coms.Length; ++i)
                {
                    AddCom(coms[i]);
                }
            }
        }
#endregion

#region Private

        private ICom GetCom(Type t)
        {
            for (int i = m_ComponentList.Count - 1; i >= 0; --i)
            {
                if (m_ComponentList[i].GetType() == t)
                {
                    return m_ComponentList[i];
                }
            }
            return null;
        }

        //这玩意会产生alloac
        protected void ProcessAllCom(Action<ICom> process)
        {
            for (int i = m_ComponentList.Count - 1; i >= 0; --i)
            {
                process(m_ComponentList[i]);
            }
        }

        protected void LateUpdateAllComs()
        {
            float dt = Time.deltaTime;
            for (int i = m_UpdateComList.Count - 1; i >= 0; --i)
            {
                m_UpdateComList[i].OnComLateUpdate(dt);
            }
        }

        protected void UpdateAllComs()
        {
            float dt = Time.deltaTime;
            for (int i = m_UpdateComList.Count - 1; i >= 0; --i)
            {
                m_UpdateComList[i].OnComUpdate(dt);
            }
        }

        protected void AwakeCom(ICom wrap)
        {
            wrap.AwakeCom(this);
        }

        protected void StartCom(ICom wrap)
        {
            wrap.OnComStart();
        }

        protected void DestroyCom(ICom wrap)
        {
            wrap.DestroyCom();
        }

        private int ComWrapComparison(ICom a, ICom b)
        {
            return a.comOrder - b.comOrder;
        }

        protected virtual void OnActorAwake()
        {

        }

        protected virtual void OnActorStart()
        {

        }

        protected virtual void OnActorDestroy()
        {

        }

        protected virtual void OnAddCom(ICom com)
        {

        }

        protected virtual void OnRemoveCom(ICom com)
        {

        }
#endregion
    }
}
