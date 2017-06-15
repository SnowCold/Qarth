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
    public class AssetRes : BaseRes
    {
        protected string[]            m_AssetBundleArray;
        protected AssetBundleRequest  m_AssetBundleRequest;

        public static AssetRes Allocate(string name)
        {
            AssetRes res = ObjectPool<AssetRes>.S.Allocate();
            if (res != null)
            {
                res.name = name;
                res.InitAssetBundleName();
            }
            return res;
        }

        protected string assetBundleName
        {
            get
            {
                if (m_AssetBundleArray == null)
                {
                    return null;
                }
                return m_AssetBundleArray[0];
            }
        }
        public AssetRes(string name) : base(name)
        {
            
        }

        public AssetRes()
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

            if (string.IsNullOrEmpty(assetBundleName))
            {
                return false;
            }

            AssetBundleRes abR = ResMgr.S.GetRes<AssetBundleRes>(assetBundleName);

            if (abR == null || abR.assetBundle == null)
            {
				Log.e("Failed to Load Asset, Not Find AssetBundleImage:" + assetBundleName);
                return false;
            }

            resState = eResState.kLoading;

            //TimeDebugger timer = ResMgr.S.timeDebugger;

            //timer.Begin("LoadSync Asset:" + m_Name);

            HoldDependRes();

            UnityEngine.Object obj = abR.assetBundle.LoadAsset(m_Name);
            //timer.End();

            UnHoldDependRes();

            if (obj == null)
            {
                Log.e("Failed Load Asset:" + m_Name);
                OnResLoadFaild();
                return false;
            }

            m_Asset = obj;

            resState = eResState.kReady;
            //Log.i(string.Format("Load AssetBundle Success.ID:{0}, Name:{1}", m_Asset.GetInstanceID(), m_Name));

            //timer.Dump(-1);
            return true;
        }

        public override void LoadAsync()
        {
            if (!CheckLoadAble())
            {
                return;
            }

            if (string.IsNullOrEmpty(assetBundleName))
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

            AssetBundleRes abR = ResMgr.S.GetRes<AssetBundleRes>(assetBundleName);

            if (abR == null || abR.assetBundle == null)
            {
                Log.e("Failed to Load Asset, Not Find AssetBundleImage:" + assetBundleName);
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            //确保加载过程中依赖资源不被释放:目前只有AssetRes需要处理该情况
            HoldDependRes();

            AssetBundleRequest abQ = abR.assetBundle.LoadAssetAsync(m_Name);
            m_AssetBundleRequest = abQ;

            yield return abQ;

            m_AssetBundleRequest = null;

            UnHoldDependRes();

            if (refCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            if (!abQ.isDone)
            {
                Log.e("Failed Load Asset:" + m_Name);
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            m_Asset = abQ.asset;

            if (m_Asset == null)
            {
                Log.e("Failed Load Asset:" + m_Name);
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            resState = eResState.kReady;

            finishCallback();
        }

        public override string[] GetDependResList()
        {
            return m_AssetBundleArray;
        }

        public override void OnCacheReset()
        {
            m_AssetBundleArray = null;
        }
        
        public override void Recycle2Cache()
        {
            ObjectPool<AssetRes>.S.Recycle(this);
        }

        protected override float CalculateProgress()
        {
            if (m_AssetBundleRequest == null)
            {
                return 0;
            }

            return m_AssetBundleRequest.progress;
        }

        protected void InitAssetBundleName()
        {
            m_AssetBundleArray = null;

            AssetData config = AssetDataTable.S.GetAssetData(m_Name);

            if (config == null)
            {
                Log.e("Not Find AssetData For Asset:" + m_Name);
                return;
            }

            string assetBundleName = AssetDataTable.S.GetAssetBundleName(config.assetName, config.assetBundleIndex);

            if (string.IsNullOrEmpty(assetBundleName))
            {
                Log.e("Not Find AssetBundle In Config:" + config.assetBundleIndex);
                return;
            }
            m_AssetBundleArray = new string[1];
            m_AssetBundleArray[0] = assetBundleName;
        }
    }
}
