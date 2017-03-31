using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class ABManifestHandler
    {
        private static AssetBundleManifest m_Manifest;

        public static AssetBundleManifest manifest
        {
            get
            {
                return m_Manifest;
            }

            set
            {
                m_Manifest = value;
            }
        }


        public static AssetBundleManifest LoadInstance()
        {
            ResLoader loader = ResLoader.Allocate();

            AssetBundleManifest manifest = loader.LoadSync(ProjectPathConfig.abManifestAssetName) as AssetBundleManifest;

            loader.UnloadImage(false);

            return manifest;
        }

        public static string[] GetAllDependenciesByUrl(string url)
        {
            return m_Manifest.GetAllDependencies(ProjectPathConfig.AssetBundleUrl2Name(url));
        }
    }
}
