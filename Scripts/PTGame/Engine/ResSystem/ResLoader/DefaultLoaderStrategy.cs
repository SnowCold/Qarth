using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
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
