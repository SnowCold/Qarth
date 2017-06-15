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
    public class DefaultLoaderStrategy : IResLoaderStrategy
    {
        public void OnAllTaskFinish(IResLoader loader)
        {

        }

        public void OnAsyncLoadFinish(IResLoader loader, AssetRes res)
        {

        }

        public void OnAsyncLoadFinish(IResLoader loader, AssetBundleRes res)
        {

        }

        public void OnAsyncLoadFinish(IResLoader loader, InternalRes res)
        {

        }

        public void OnAsyncLoadFinish(IResLoader loader, IRes res)
        {

        }

        public void OnSyncLoadFinish(IResLoader loader, InternalRes res)
        {

        }

        public void OnSyncLoadFinish(IResLoader loader, AssetRes res)
        {

        }

        public void OnSyncLoadFinish(IResLoader loader, AssetBundleRes res)
        {

        }

        public void OnSyncLoadFinish(IResLoader loader, IRes res)
        {

        }
    }
}
