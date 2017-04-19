using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class ResLoader : DisposableObject, IResLoader, ICacheAble, ICacheType
    {
        class CallBackWrap
        {
            private Action<bool, IRes>  m_Listener;
            private IRes                m_Res;

            public CallBackWrap(IRes r, Action<bool, IRes> l)
            {
                m_Res = r;
                m_Listener = l;
            }

            public void Release()
            {
                m_Res.UnRegisteResListener(m_Listener);
            }

            public bool IsRes(IRes res)
            {
                if (res.name == m_Res.name)
                {
                    return true;
                }
                return false;
            }
        }

        private List<IRes>                      m_ResArray = new List<IRes>();
        private LinkedList<IRes>                m_WaitLoadList = new LinkedList<IRes>();
        private Action                          m_Listener;
        private IResLoaderStrategy              m_Strategy;

        private bool                            m_CacheFlag = false;
        private int                             m_LoadingCount = 0;

        private LinkedList<CallBackWrap>        m_CallbackRecardList;
        private static DefaultLoaderStrategy    s_DefaultStrategy;

        public static IResLoaderStrategy defaultStrategy
        {
            get
            {
                if (s_DefaultStrategy == null)
                {
                    s_DefaultStrategy = new DefaultLoaderStrategy();
                }
                return s_DefaultStrategy;
            }
        }

        public float progress
        {
            get
            {
                if (m_WaitLoadList.Count == 0)
                {
                    return 1;
                }

                float unit = 1.0f / m_ResArray.Count;
                float currentValue = unit * (m_ResArray.Count - m_LoadingCount);

                LinkedListNode<IRes> currentNode = m_WaitLoadList.First;

                while (currentNode != null)
                {
                    currentValue += unit * currentNode.Value.progress;
                    currentNode = currentNode.Next;
                }

                return currentValue;
            }
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

        public static ResLoader Allocate(IResLoaderStrategy strategy = null)
        {
            ResLoader loader = ObjectPool<ResLoader>.S.Allocate();
            loader.SetStrategy(strategy);
            return loader;
        }

        public ResLoader()
        {
            SetStrategy(s_DefaultStrategy);
        }

        public void Add2Load(List<string> list)
        {
            if (list == null)
            {
                return;
            }

            for (int i = list.Count - 1; i >= 0; --i)
            {
                Add2Load(list[i]);
            }
        }

        public void Add2Load(string name, Action<bool, IRes> listener = null, bool lastOrder = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                Log.e("Res Name Is Null.");
                return;
            }

            IRes res = FindResInArray(m_ResArray, name);
            if (res != null)
            {
                if (listener != null)
                {
                    AddResListenerReward(res, listener);
                    res.RegisteResListener(listener);
                }
                return;
            }

            res = ResMgr.S.GetRes(name, true);

            if (res == null)
            {
                return;
            }

            if (listener != null)
            {
                AddResListenerReward(res, listener);
                res.RegisteResListener(listener);
            }

            //无论该资源是否加载完成，都需要添加对该资源依赖的引用
            string[] depends = res.GetDependResList();

            if (depends != null)
            {
                for (int i = 0; i < depends.Length; ++i)
                {
                    Add2Load(depends[i]);
                }
            }

            AddRes2Array(res, lastOrder);
        }

        public void LoadSync()
        {
            while (m_WaitLoadList.Count > 0)
            {
                IRes first = m_WaitLoadList.First.Value;
                --m_LoadingCount;
                m_WaitLoadList.RemoveFirst();

                if (first == null)
                {
                    return;
                }

                if (first.LoadSync())
                {
                    first.AcceptLoaderStrategySync(this, m_Strategy);
                }
            }

            m_Strategy.OnAllTaskFinish(this);
        }

        public UnityEngine.Object LoadSync(string name)
        {
            Add2Load(name);
            LoadSync();

            IRes res = ResMgr.S.GetRes(name, false);
            if (res == null)
            {
                Log.e("Failed to Load Res:" + name);
                return null;
            }

            return res.asset;
        }

        public void LoadAsync(Action listener = null)
        {
            m_Listener = listener;
            //ResMgr.S.timeDebugger.Begin("LoadAsync");
            DoLoadAsync();
        }

        public void ReleaseRes(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            IRes res = ResMgr.S.GetRes(name, false);
            if (res == null)
            {
                return;
            }

            if (m_WaitLoadList.Remove(res))
            {
                --m_LoadingCount;
                if (m_LoadingCount == 0)
                {
                    m_Listener = null;
                }
            }

            if (m_ResArray.Remove(res))
            {
                res.UnRegisteResListener(OnResLoadFinish);
                res.SubRef();
                ResMgr.S.SetResMapDirty();
            }
        }

        public void ReleaseRes(string[] names)
        {
            if (names == null || names.Length == 0)
            {
                return;
            }

            for (int i = names.Length - 1; i >=0; --i)
            {
                ReleaseRes(names[i]);
            }
        }

        public void ReleaseAllRes()
        {
            m_Listener = null;
            m_LoadingCount = 0;
            m_WaitLoadList.Clear();

            if(m_ResArray.Count > 0)
            {
                //确保首先删除的是AB，这样能对Asset的卸载做优化
                m_ResArray.Reverse();

                for (int i = m_ResArray.Count - 1; i >= 0; --i)
                {
                    m_ResArray[i].UnRegisteResListener(OnResLoadFinish);
                    m_ResArray[i].SubRef();
                }

                m_ResArray.Clear();
                ResMgr.S.SetResMapDirty();
            }

            RemoveAllCallbacks(true);
        }

        public void UnloadImage(bool flag)
        {
            if (m_ResArray.Count > 0)
            {
                for (int i = m_ResArray.Count - 1; i >= 0; --i)
                {
                    if (m_ResArray[i].UnloadImage(flag))
                    {
                        if(m_WaitLoadList.Remove(m_ResArray[i]))
                        {
                            --m_LoadingCount;
                        }

                        RemoveCallback(m_ResArray[i], true);

                        m_ResArray[i].UnRegisteResListener(OnResLoadFinish);
                        m_ResArray[i].SubRef();
                        m_ResArray.RemoveAt(i);
                    }
                }
                ResMgr.S.SetResMapDirty();
            }
        }

        public override void Dispose()
        {
            ReleaseAllRes();
            base.Dispose();
        }

        public void Dump()
        {
            for (int i = 0; i < m_ResArray.Count; ++i)
            {
                Log.i(m_ResArray[i].name);
            }
        }

        private void SetStrategy(IResLoaderStrategy strategy)
        {
            m_Strategy = strategy;
            if (m_Strategy == null)
            {
                m_Strategy = defaultStrategy;
            }
        }

        private void DoLoadAsync()
        {
            var nextNode = m_WaitLoadList.First;
            LinkedListNode<IRes> currentNode = null;
            while (nextNode != null)
            {
                currentNode = nextNode;
                IRes res = currentNode.Value;
                nextNode = currentNode.Next;
                if (res.IsDependResLoadFinish())
                {
                    m_WaitLoadList.Remove(currentNode);

                    if (res.resState != eResState.kReady)
                    {
                        res.RegisteResListener(OnResLoadFinish);
                        res.LoadAsync();
                    }
                    else
                    {
                        --m_LoadingCount;
                    }
                }
            }
        }

        private void RemoveCallback(IRes res, bool release)
        {
            if (m_CallbackRecardList != null)
            {
                LinkedListNode<CallBackWrap> current = m_CallbackRecardList.First;
                LinkedListNode<CallBackWrap> next = null;
                while (current != null)
                {
                    next = current.Next;
                    if (current.Value.IsRes(res))
                    {
                        if (release)
                        {
                            current.Value.Release();
                        }
                        m_CallbackRecardList.Remove(current);
                    }
                    current = next;
                }
            }
        }

        private void RemoveAllCallbacks(bool release)
        {
            if (m_CallbackRecardList != null)
            {
                int count = m_CallbackRecardList.Count;
                while (count > 0)
                {
                    --count;
                    if (release)
                    {
                        m_CallbackRecardList.Last.Value.Release();
                    }
                    m_CallbackRecardList.RemoveLast();
                }
            }
        }

        private void OnResLoadFinish(bool result, IRes res)
        {
            --m_LoadingCount;

            res.AcceptLoaderStrategyAsync(this, m_Strategy);
            DoLoadAsync();
            if (m_LoadingCount == 0)
            {
                RemoveAllCallbacks(false);

                //ResMgr.S.timeDebugger.End();
                //ResMgr.S.timeDebugger.Dump(-1);
                if (m_Listener != null)
                {
                    m_Listener();
                }

                m_Strategy.OnAllTaskFinish(this);
            }
        }

        private void AddRes2Array(IRes res, bool lastOrder)
        {
            //再次确保队列中没有它
            IRes oldRes = FindResInArray(m_ResArray, res.name);

            if (oldRes != null)
            {
                return;
            }

            res.AddRef();
            m_ResArray.Add(res);

            if (res.resState != eResState.kReady)
            {
                ++m_LoadingCount;
                if (lastOrder)
                {
                    m_WaitLoadList.AddLast(res);
                }
                else
                {
                    m_WaitLoadList.AddFirst(res);
                }
            }
        }

        private static IRes FindResInArray(List<IRes> list, string name)
        {
            if (list == null)
            {
                return null;
            }

            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].name.Equals(name))
                {
                    return list[i];
                }
            }

            return null;
        }

        private void AddResListenerReward(IRes res, Action<bool, IRes> listener)
        {
            if (m_CallbackRecardList == null)
            {
                m_CallbackRecardList = new LinkedList<CallBackWrap>();
            }

            m_CallbackRecardList.AddLast(new CallBackWrap(res, listener));
        }

        public void OnCacheReset()
        {
            ReleaseAllRes();
        }

        public void Recycle2Cache()
        {
            ObjectPool<ResLoader>.S.Recycle(this);
        }
    }
}
