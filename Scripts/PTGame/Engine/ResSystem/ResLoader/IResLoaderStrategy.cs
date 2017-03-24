using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    //资源加载&释放策略
    public interface IResLoaderStrategy
    {
        void OnAllTaskFinish(IResLoader loader);
        void OnSyncLoadFinish(IResLoader loader, IRes res);
        void OnSyncLoadFinish(IResLoader loader, AssetBundleRes res);
        void OnSyncLoadFinish(IResLoader loader, AssetRes res);
        void OnSyncLoadFinish(IResLoader loader, InternalRes res);

        void OnAsyncLoadFinish(IResLoader loader, IRes res);
        void OnAsyncLoadFinish(IResLoader loader, AssetBundleRes res);
        void OnAsyncLoadFinish(IResLoader loader, AssetRes res);
        void OnAsyncLoadFinish(IResLoader loader, InternalRes res);
    }

    public class AbstractResLoaderStrategy : IResLoaderStrategy
    {
        public virtual void OnAllTaskFinish(IResLoader loader)
        {

        }

        public virtual void OnSyncLoadFinish(IResLoader loader, IRes res)
        {

        }

        public virtual void OnSyncLoadFinish(IResLoader loader, AssetBundleRes res)
        {

        }

        public virtual void OnSyncLoadFinish(IResLoader loader, AssetRes res)
        {

        }

        public virtual void OnSyncLoadFinish(IResLoader loader, InternalRes res)
        {

        }

        public virtual void OnAsyncLoadFinish(IResLoader loader, IRes res)
        {

        }

        public virtual void OnAsyncLoadFinish(IResLoader loader, AssetBundleRes res)
        {

        }

        public virtual void OnAsyncLoadFinish(IResLoader loader, AssetRes res)
        {

        }

        public virtual void OnAsyncLoadFinish(IResLoader loader, InternalRes res)
        {

        }
    }
}
