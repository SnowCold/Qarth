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

namespace PTGame.Framework
{
    public class ResFactory
    {
        private delegate IRes ResCreator(string name);

        static ResFactory()
        {
            Log.i("Init[ResFactory]");
            ObjectPool<AssetBundleRes>.S.maxCacheCount = 20;
            ObjectPool<AssetRes>.S.maxCacheCount = 40;
            ObjectPool<InternalRes>.S.maxCacheCount = 40;
            ObjectPool<NetImageRes>.S.maxCacheCount = 20;
        }

        public static IRes Create(string name)
        {
            short assetType = 0;
            if (name.StartsWith("Resources/"))
            {
                assetType = eResType.kInternal;
            }
            else if (name.StartsWith("NetImage:"))
            {
                assetType = eResType.kNetImageRes;
            }
            else if (name.StartsWith("HotUpdateRes:"))
            {
                assetType = eResType.kHotUpdateRes;
            }
            else
            {
                AssetData data = AssetDataTable.S.GetAssetData(name);
                if (data == null)
                {
                    Log.e("Failed to Create Res. Not Find AssetData:" + name);
                    return null;
                }
                else
                {
                    assetType = data.assetType;
                }
            }

            return Create(name, assetType);
        }

        public static IRes Create(string name, short assetType)
        {
            switch (assetType)
            {
                case eResType.kAssetBundle:
                    return AssetBundleRes.Allocate(name);
                case eResType.kABAsset:
                    return AssetRes.Allocate(name);
                case eResType.kABScene:
                    return SceneRes.Allocate(name);
                case eResType.kInternal:
                    return InternalRes.Allocate(name);
                case eResType.kNetImageRes:
                    return NetImageRes.Allocate(name);
                case eResType.kHotUpdateRes:
                    return HotUpdateRes.Allocate(name);
                default:
                    Log.e("Invalid assetType :" + assetType);
                    return null;
            }
        }
    }
}
