using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class ResUpdateMgr : TSingleton<ResUpdateMgr>
    {
        protected ResLoader m_ResLoader;
        protected Action m_CheckListener;
        protected Dictionary<string, List<ABUnit>> m_UpdateItemMap;

        public override void OnSingletonInit()
        {
            m_ResLoader = ResLoader.Allocate();
        }

        public void CheckUpdate(Action checkCallback)
        {
            m_CheckListener = checkCallback;

            //1.下载远程config文件
            AddRemoteABConfigDownloadTask("res/asset_bindle_config.bin");
            AddRemoteABConfigDownloadTask("res/i18res/cn/asset_bindle_config.bin");
            AddRemoteABConfigDownloadTask("res/i18res/en/asset_bindle_config.bin");

            //1.内置资源包启动阶段就需要检测更新，并强制更新
            //2.扩展资源包可以在启动时检测是否有更新，但不强制更新。可以在后续需要的时候启动更新，如果更新未完成就暂时冻结该资源包功能就行。

            m_ResLoader.LoadAsync(OnAllRemoteConfigFileReady);
        }

        public void DownloadPackage(string name, string url)
        {
            string key = string.Format("HotUpdateRes:{0}", name);
            m_ResLoader.Add2Load(key, OnPackageDownloadFinish);

            HotUpdateRes hotUpdateRes = ResMgr.S.GetRes<HotUpdateRes>(key);

            hotUpdateRes.SetUpdateInfo(name, "http://blog.csdn.net/yupu56/article/details/47301481", true);
        }

        public void StartUpdateItem(List<ABUnit> list)
        {
            if (list == null)
            {
                return;
            }

            for (int i = 0; i < list.Count; ++i)
            {
                string key = string.Format("HotUpdateRes:{0}", list[i].abName);
                m_ResLoader.Add2Load(key);
                HotUpdateRes res = ResMgr.S.GetRes<HotUpdateRes>(key);
                res.SetUpdateInfo(list[i].abName, string.Format("http://{0}", list[i].abName), false);
            }

            string[] urls = new string[]
                {
                    "http://blog.csdn.net/fanbin168/article/details/38521679",
                    "http://www.cnblogs.com/zhaoqingqing/p/5439852.html"
                };

            if (list.Count == 0)
            {
                for (int i = 0; i < urls.Length; ++i)
                {
                    AddTestUnit(string.Format("Key{0}", i), urls[i]);
                }
            }

            m_ResLoader.LoadAsync();
        }

        private void OnAllRemoteConfigFileReady()
        {
            if (m_CheckListener != null)
            {
                m_CheckListener();
            }
        }

        private void OnPackageDownloadFinish(bool result, IRes res)
        {

        }

        private void AddRemoteABConfigDownloadTask(string name)
        {
            string key = string.Format("HotUpdateRes:{0}", name);
            m_ResLoader.Add2Load(key, OnRemoteABConfigDownloadFinish);

            HotUpdateRes hotUpdateRes = ResMgr.S.GetRes<HotUpdateRes>(key);

            hotUpdateRes.SetUpdateInfo(name, "http://blog.csdn.net/yupu56/article/details/47301481", true);
        }

        private void OnRemoteABConfigDownloadFinish(bool result, IRes res)
        {
            if (!result)
            {
                return;
            }

            HotUpdateRes hotUpdateRes = res as HotUpdateRes;

            if (hotUpdateRes == null)
            {
                return;
            }

            //所有AB更新完成后需要替换当前的Config文件，启用独立的ResLoader来完成
            StartProcessPackage(hotUpdateRes);
        }

        private void StartProcessPackage(HotUpdateRes res)
        {
            AssetDataTable remoteDataTable = new AssetDataTable();

            remoteDataTable.LoadFromFile(res.destionResPath);

            var downloadList = CalculateUpdateList(AssetDataTable.S, remoteDataTable);

            if (downloadList != null && downloadList.Count > 0)
            {
                if (m_UpdateItemMap == null)
                {
                    m_UpdateItemMap = new Dictionary<string, List<ABUnit>>();
                }

                m_UpdateItemMap.Add(res.name, downloadList);
            }
            //1.这里其实不需要forceUpdate的概念，只需要建立一个有更新的Config 集合记录下。并通知上层是否有更新
            //由上层决定是否更新，更新哪些
            //PrepareDownloadItem(downloadList);
        }

        private void AddTestUnit(string name, string url)
        {
            string key = string.Format("HotUpdateRes:{0}", name);
            m_ResLoader.Add2Load(key);
            HotUpdateRes res = ResMgr.S.GetRes<HotUpdateRes>(key);
            res.SetUpdateInfo(name, url, false);
        }

        private List<ABUnit> CalculateDeleteList(AssetDataTable local, AssetDataTable remote)
        {
            if (remote == null || local == null)
            {
                return null;
            }

            var remoteABList = remote.GetAllABUnit();

            //防止远程清单文件下载失败导致本地被删光
            if (remoteABList.Count == 0)
            {
                return null;
            }

            List<ABUnit> localABList = local.GetAllABUnit();

            List<ABUnit> deleteABList = new List<ABUnit>();

            for (int i = localABList.Count - 1; i >= 0; --i)
            {
                ABUnit localABUnit = localABList[i];
                ABUnit remoteABUnit = remote.GetABUnit(localABUnit.abName);

                if (remoteABUnit == null)
                {
                    deleteABList.Add(localABUnit);
                    continue;
                }
            }

            return deleteABList;
        }

        private List<ABUnit> CalculateUpdateList(AssetDataTable local, AssetDataTable remote)
        {
            if (remote == null || local == null)
            {
                return null;
            }

            List<ABUnit> remoteABUnitList = remote.GetAllABUnit();

            List<ABUnit> downloadABList = new List<ABUnit>();

            for (int i = remoteABUnitList.Count - 1; i >= 0; --i)
            {
                ABUnit remoteUnit = remoteABUnitList[i];
                ABUnit localABUnit = local.GetABUnit(remoteUnit.abName);

                if (localABUnit == null)
                {
                    //更新的新资源
                    downloadABList.Add(remoteUnit);
                    continue;
                }

                if (localABUnit.md5.Equals(remoteUnit.md5))
                {
                    continue;
                }

                if (localABUnit.buildTime < remoteUnit.buildTime)
                {
                    downloadABList.Add(remoteUnit);
                }
            }

            return downloadABList;
        }
    }
}
