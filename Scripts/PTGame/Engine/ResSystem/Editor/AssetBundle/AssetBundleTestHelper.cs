using System;
using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SCFramework.Editor
{
    class AssetBundleTestHelper
    {
        [MenuItem("Assets/SCEngine/Tester/SetAssetBundlePath")]
        public static void SetAssetBundlePath()
        {
            //EditUtils.GetFullPath4AssetsPath(EditUtils.CurrentSelectPath);
        }

        [MenuItem("Assets/SCEngine/Tester/UnloadUnUsedAsset")]
        public static void UnloadUnUsedAsset()
        {
            Resources.UnloadUnusedAssets();
        }
    }
}
