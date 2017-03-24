using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class SceneRes : AssetRes
    {
        public new static SceneRes Allocate(string name)
        {
            SceneRes res = ObjectPool<SceneRes>.S.Allocate();
            if (res != null)
            {
                res.name = name;
                res.InitAssetBundleName();
            }
            return res;
        }

        public SceneRes(string name) : base(name)
        {

        }

        public SceneRes()
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
                Log.e("Failed to Load Asset, Not Find AssetBundleImage:" + abR);
                return false;
            }


            resState = eResState.kReady;
            return true;
        }

        public override void LoadAsync()
        {
            LoadSync();
        }


        public override void Recycle2Cache()
        {
            ObjectPool<SceneRes>.S.Recycle(this);
        }
    }
}
