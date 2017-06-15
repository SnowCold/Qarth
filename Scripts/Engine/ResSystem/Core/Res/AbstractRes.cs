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
    public class AbstractRes : RefCounter, IRes, ICacheAble
    {
        protected string                    m_Name;
        private short                       m_ResState = eResState.kWaiting;
        private bool                        m_CacheFlag = false;
        protected UnityEngine.Object        m_Asset;
        private event Action<bool, IRes>    m_ResListener;
        private event Action<bool, IRes>    m_LoaderListener;
        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public short resState
        {
            get { return m_ResState; }
            set
            {
                m_ResState = value;
                if (m_ResState == eResState.kReady)
                {
                    NotifyResEvent(true);
                }
            }
        }

        public float progress
        {
            get
            {
                if (m_ResState == eResState.kLoading)
                {
                    return CalculateProgress();
                }

                if (m_ResState == eResState.kReady)
                {
                    return 1;
                }

                return 0;
            }
        }

        protected virtual float CalculateProgress()
        {
            return 0;
        }

        public UnityEngine.Object asset
        {
            get { return m_Asset; }
            set { m_Asset = value; }
        }

        public virtual object rawAsset
        {
            get { return null; }
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

        public virtual void AcceptLoaderStrategySync(IResLoader loader, IResLoaderStrategy strategy)
        {
            strategy.OnSyncLoadFinish(loader, this);
        }

        public virtual void AcceptLoaderStrategyAsync(IResLoader loader, IResLoaderStrategy strategy)
        {
            strategy.OnAsyncLoadFinish(loader, this);
        }

        public void RegisteResListener(Action<bool, IRes> listener)
        {
            if (listener == null)
            {
                return;
            }

            if (m_ResState == eResState.kReady)
            {
                listener(true, this);
                return;
            }

            m_ResListener += listener;
        }

        public void LoaderRegisteListener(Action<bool, IRes> listener)
        {
            if (listener == null)
            {
                return;
            }

            if (m_ResState == eResState.kReady)
            {
                listener(true, this);
                return;
            }

            m_LoaderListener += listener;
        }

        public void UnRegisteResListener(Action<bool, IRes> listener)
        {
            if (listener == null)
            {
                return;
            }

            if (m_ResListener == null)
            {
                return;
            }

            m_ResListener -= listener;
        }

        public void LoaderUnRegisteListener(Action<bool, IRes> listener)
        {
            if (listener == null)
            {
                return;
            }

            if (m_LoaderListener == null)
            {
                return;
            }

            m_LoaderListener -= listener;
        }

        protected void OnResLoadFaild()
        {
            m_ResState = eResState.kWaiting;
            NotifyResEvent(false);
        }

        private void NotifyResEvent(bool result)
        {
            if (m_ResListener != null)
            {
                m_ResListener(result, this);
                m_ResListener = null;
            }

            if (m_LoaderListener != null)
            {
                m_LoaderListener(result, this);
                m_LoaderListener = null;
            }
        }

        protected AbstractRes(string name)
        {
            m_Name = name;
        }

        public AbstractRes()
        {

        }

        protected bool CheckLoadAble()
        {
            if (m_ResState == eResState.kWaiting)
            {
                return true;
            }

            return false;
        }

        protected void HoldDependRes()
        {
            string[] depends = GetDependResList();
            if (depends == null || depends.Length == 0)
            {
                return;
            }

            for (int i = depends.Length - 1; i >= 0; --i)
            {
                var res = ResMgr.S.GetRes(depends[i], false);
                if (res != null)
                {
                    res.AddRef();
                }
            }
        }

        protected void UnHoldDependRes()
        {
            string[] depends = GetDependResList();
            if (depends == null || depends.Length == 0)
            {
                return;
            }

            for (int i = depends.Length - 1; i >= 0; --i)
            {
                var res = ResMgr.S.GetRes(depends[i], false);
                if (res != null)
                {
                    res.SubRef();
                }
            }
        }

        #region 子类实现
        public virtual bool LoadSync()
        {
            return false;
        }

        public virtual void LoadAsync()
        {
        }

        public virtual string[] GetDependResList()
        {
            return null;
        }

        public bool IsDependResLoadFinish()
        {
            string[] depends = GetDependResList();
            if (depends == null || depends.Length == 0)
            {
                return true;
            }

            for (int i = depends.Length - 1; i >= 0; --i)
            {
                var res = ResMgr.S.GetRes(depends[i], false);
                if (res == null || res.resState != eResState.kReady)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual bool UnloadImage(bool flag)
        {
            return false;
        }

        public bool ReleaseRes()
        {
            m_ResListener = null;
            m_LoaderListener = null;

            if (m_ResState == eResState.kLoading)
            {
                return false;
            }

            if (m_ResState != eResState.kReady)
            {
                return true;
            }

            //Log.i("Release Res:" + m_Name);

            OnReleaseRes();

            m_ResState = eResState.kWaiting;

            return true;
        }

        protected virtual void OnReleaseRes()
        {

        }

        protected override void OnZeroRef()
        {
            if (m_ResState == eResState.kLoading)
            {
                return;
            }

            ReleaseRes();
        }

        public virtual void Recycle2Cache()
        {
            
        }

        public virtual void OnCacheReset()
        {
            m_Name = null;
            m_ResListener = null;
        }

        public virtual IEnumerator StartIEnumeratorTask(Action finishCallback)
        {
            finishCallback();
            yield break;
        }
        #endregion
    }
}
