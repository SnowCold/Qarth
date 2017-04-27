using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class ResUpdateMgr : TSingleton<ResUpdateMgr>
    {
        protected Action m_CheckListener;
        protected Dictionary<string, ResPackageHandler> m_PackageMap = new Dictionary<string, ResPackageHandler>();

        public override void OnSingletonInit()
        {

        }

        public static string AssetName2ResName(string assetName)
        {
            return string.Format("HotUpdateRes:{0}", assetName);
        }

        public bool IsPackageNeedUpdate(string packageName)
        {
            if (m_PackageMap == null)
            {
                return false;
            }

            if (!m_PackageMap.ContainsKey(packageName))
            {
                return false;
            }

            return m_PackageMap[packageName].needUpdate;
        }

        public void CheckPackage(ResPackage package, Action<ResPackageHandler> checkCallback)
        {
            ResPackageHandler handler = new ResPackageHandler(package);

            if (m_PackageMap.ContainsKey(package.packageName))
            {
                m_PackageMap.Remove(package.packageName);
            }

            m_PackageMap.Add(package.packageName, handler);

            handler.CheckUpdateList(checkCallback);
        }

        //下载资源zip包:完成后解压
        public void DownloadPackage(string packageName, Action<ResPackageHandler> downloadCallback)
        {
            ResPackageHandler handler = m_PackageMap[packageName];

            handler.DownloadPackage(downloadCallback);
        }

        public void StartUpdatePackage(string packageName, Action<ResPackageHandler> updateListener)
        {
            if (!IsPackageNeedUpdate(packageName))
            {
                return;
            }

            ResPackageHandler handler = m_PackageMap[packageName];

            handler.StartUpdate(updateListener);
        }

        private void OnAllRemoteConfigFileReady()
        {
            if (m_CheckListener != null)
            {
                m_CheckListener();
            }
        }
    }
}
