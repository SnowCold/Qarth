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

    public class AssetBundleRes : AbstractRes
    {
        public static int s_ActiveCount = 0;
        private bool        m_UnloadFlag = true;
        private string[]    m_DependResList;
        private AssetBundleCreateRequest m_AssetBundleCreateRequest;

        public static AssetBundleRes Allocate(string name)
        {
            AssetBundleRes res = ObjectPool<AssetBundleRes>.S.Allocate();
            if (res != null)
            {
                res.name = name;
                res.InitAssetBundleName();
            }
            return res;
        }

        public AssetBundle assetBundle
        {
            get
            {
                return (AssetBundle)m_Asset;
            }

            set
            {
                m_Asset = value;
            }
        }

        public AssetBundleRes(string name) : base(name)
        {

        }

        public AssetBundleRes()
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

            resState = eResState.kLoading;

            //TimeDebugger timer = ResMgr.S.timeDebugger;

            string url = ProjectPathConfig.AssetBundleName2Url(m_Name);

            //timer.Begin("LoadSync AssetBundle:" + m_Name);
            AssetBundle bundle = AssetBundle.LoadFromFile(url);
            //timer.End();

            m_UnloadFlag = true;

            if (bundle == null)
            {
                Log.e("Failed Load AssetBundle:" + m_Name);
                OnResLoadFaild();
                return false;
            }

            assetBundle = bundle;
            resState = eResState.kReady;
            ++s_ActiveCount;
            //Log.i(string.Format("Load AssetBundle Success.ID:{0}, Name:{1}", bundle.GetInstanceID(), bundle.name));

            //timer.Dump(-1);
            return true;
        }

        public override void LoadAsync()
        {
            if (!CheckLoadAble())
            {
                return;
            }
            
            resState = eResState.kLoading;

            ResMgr.S.PostIEnumeratorTask(this);
        }

        public override IEnumerator StartIEnumeratorTask(Action finishCallback)
        {
            //开启的时候已经结束了
            if (refCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            string url = ProjectPathConfig.AssetBundleName2Url(m_Name);

            AssetBundleCreateRequest abcR = AssetBundle.LoadFromFileAsync(url);

            m_AssetBundleCreateRequest = abcR;
            yield return abcR;
            m_AssetBundleCreateRequest = null;

            if (!abcR.isDone)
            {
                Log.e("AssetBundleCreateRequest Not Done! Path:" + m_Name);
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            assetBundle = abcR.assetBundle;

            resState = eResState.kReady;
            ++s_ActiveCount;

            finishCallback();
        }

        public override string[] GetDependResList()
        {
            return m_DependResList;
        }

        public override bool UnloadImage(bool flag)
        {
            if (assetBundle != null)
            {
                m_UnloadFlag = flag;
            }

            return true;
        }
        
        public override void Recycle2Cache()
        {
            ObjectPool<AssetBundleRes>.S.Recycle(this);
        }
        
        public override void OnCacheReset()
        {
            base.OnCacheReset();
            m_UnloadFlag = true;
            m_DependResList = null;
        }

        protected override float CalculateProgress()
        {
            if (m_AssetBundleCreateRequest == null)
            {
                return 0;
            }

            return m_AssetBundleCreateRequest.progress;
        }

        protected override void OnReleaseRes()
        {
            if (assetBundle != null)
            {
                //ResMgr.S.timeDebugger.Begin("Unload AssetBundle:" + m_Name);
                assetBundle.Unload(m_UnloadFlag);
                assetBundle = null;
                --s_ActiveCount;
                //ResMgr.S.timeDebugger.End();
            }
        }

        private void InitAssetBundleName()
        {
            m_DependResList = AssetDataTable.S.GetAllDependenciesByUrl(name);
        }
    }
}
