//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
    public class InternalRes : BaseRes
    {
        private ResourceRequest m_ResourceRequest;

        public static InternalRes Allocate(string name)
        {
            InternalRes res = ObjectPool<InternalRes>.S.Allocate();
            if (res != null)
            {
                res.name = name;
            }
            return res;
        }

        public static string Name2Path(string name)
        {
            return name.Substring(10);
        }

        public InternalRes(string name) : base(name)
        {

        }

        public InternalRes()
        {

        }

        public override void AcceptLoaderStrategySync(IResLoader loader, IResLoaderStrategy strategy)
        {
            strategy.OnSyncLoadFinish(loader, this);
        }

        public override void AcceptLoaderStrategyAsync(IResLoader loader, IResLoaderStrategy strategy)
        {
            strategy.OnAsyncLoadFinish(loader, this);
        }

        public override bool LoadSync()
        {
            if (!CheckLoadAble())
            {
                return false;
            }

            if (string.IsNullOrEmpty(m_Name))
            {
                return false;
            }

            resState = eResState.kLoading;

            //TimeDebugger timer = ResMgr.S.timeDebugger;

            //timer.Begin("Resources.Load:" + m_Name);
            m_Asset = Resources.Load(Name2Path(m_Name));
            //timer.End();

            if (m_Asset == null)
            {
                Log.e("Failed to Load Asset From Resources:" + Name2Path(m_Name));
                OnResLoadFaild();
                return false;
            }

            resState = eResState.kReady;
            return true;
        }

        public override void LoadAsync()
        {
            if (!CheckLoadAble())
            {
                return;
            }

            if (string.IsNullOrEmpty(m_Name))
            {
                return;
            }

            resState = eResState.kLoading;

            ResMgr.S.PostIEnumeratorTask(this);
        }

        public override IEnumerator StartIEnumeratorTask(Action finishCallback)
        {
            if (refCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            ResourceRequest rQ = Resources.LoadAsync(Name2Path(m_Name));

            m_ResourceRequest = rQ;
            yield return rQ;
            m_ResourceRequest = null;

            if (!rQ.isDone)
            {
                Log.e("Failed to Load Resources:" + m_Name);
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            m_Asset = rQ.asset;

            resState = eResState.kReady;

            finishCallback();
        }

        public override void Recycle2Cache()
        {
            ObjectPool<InternalRes>.S.Recycle(this);
        }

        protected override float CalculateProgress()
        {
            if (m_ResourceRequest == null)
            {
                return 0;
            }

            return m_ResourceRequest.progress;
        }
    }
}
