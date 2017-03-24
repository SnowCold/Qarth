using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class eResState
    {
        public const short kWaiting = 0;
        public const short kLoading = 1;
        public const short kReady = 2;
        public const short kDisposing = 4;

        public static bool isReady(short value)
        {
            return value == kReady;
        }
    }

    public class eResType
    {
        public const short kAssetBundle = 0;
        public const short kABAsset = 1;
        public const short kABScene = 2;
        public const short kInternal = 3;
        public const short kNetImageRes = 4;
    }

    public interface IRes : IRefCounter, ICacheType, IEnumeratorTask
    {
        string name
        {
            get;
        }

        short resState
        {
            get;
            set;
        }

        UnityEngine.Object asset
        {
            get;
            set;
        }

        object rawAsset
        {
            get;
        }

        float progress
        {
            get;
        }

        void RegisteResListener(Action<bool, IRes> listener);
        void UnRegisteResListener(Action<bool, IRes> listener);

        bool UnloadImage(bool flag);

        bool LoadSync();

        void LoadAsync();

        string[] GetDependResList();

        bool IsDependResLoadFinish();

        bool ReleaseRes();

        void AcceptLoaderStrategySync(IResLoader loader, IResLoaderStrategy strategy);
        void AcceptLoaderStrategyAsync(IResLoader loader, IResLoaderStrategy strategy);
    }
}
