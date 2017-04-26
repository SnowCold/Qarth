using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class ResPackageHandler
    {
        private ResPackage m_Package;
        private List<ABUnit> m_UpdateUnitList;
        private ResLoader m_Loader;
        private Action m_CheckListener;
        private Action m_DownloadListener;
        private Action m_UpdateListener;

        public ResPackage package
        {
            get { return m_Package; }
        }

        public List<ABUnit> updateList
        {
            get { return m_UpdateUnitList; }
        }

        public bool needUpdate
        {
            get
            {
                if (m_UpdateUnitList == null || m_UpdateUnitList.Count == 0)
                {
                    return false;
                }

                return true;
            }
        }

        public ResPackageHandler(ResPackage package)
        {
            m_Package = package;
            m_Loader = ResLoader.Allocate("ResPackageHolder");
        }

        public void CheckUpdateList(Action callBack)
        {
            m_CheckListener = callBack;
            string resName = ResUpdateMgr.AssetName2ResName(m_Package.configFile);
            m_Loader.Add2Load(resName, OnRemoteABConfigDownloadFinish);

            HotUpdateRes hotUpdateRes = ResMgr.S.GetRes<HotUpdateRes>(resName);

            hotUpdateRes.SetUpdateInfo(m_Package.configFile, m_Package.GetAssetUrl(m_Package.configFile), true);
        }

        public void StartUpdate(Action callBack)
        {
            if (m_UpdateUnitList == null || m_UpdateUnitList.Count == 0)
            {
                return;
            }

            m_UpdateListener = callBack;

            for (int i = 0; i < m_UpdateUnitList.Count; ++i)
            {
                string resName = ResUpdateMgr.AssetName2ResName(m_UpdateUnitList[i].abName);
                m_Loader.Add2Load(resName);
                HotUpdateRes res = ResMgr.S.GetRes<HotUpdateRes>(resName);
                res.SetUpdateInfo(m_UpdateUnitList[i].abName, string.Format("http://{0}", m_UpdateUnitList[i].abName), false);
            }

            m_Loader.LoadAsync(OnPackageUpdateFinish);
        }

        public void DownloadPackage(Action callback)
        {
            m_DownloadListener = callback;
            string resName = ResUpdateMgr.AssetName2ResName(m_Package.packageName);
            m_Loader.Add2Load(resName, OnPackageDownloadFinish);

            HotUpdateRes hotUpdateRes = ResMgr.S.GetRes<HotUpdateRes>(resName);

            hotUpdateRes.SetUpdateInfo(m_Package.localPath, m_Package.zipUrl, true);
            m_Loader.LoadAsync();
        }

        private void OnPackageDownloadFinish(bool result, IRes res)
        {
            if (m_DownloadListener != null)
            {
                m_DownloadListener();
                m_DownloadListener = null;
            }
        }

        private void OnPackageUpdateFinish()
        {
            if (m_UpdateListener != null)
            {
                m_UpdateListener();
                m_UpdateListener = null;
            }
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
            ProcessRemoteABConfig(hotUpdateRes);
        }

        private void ProcessRemoteABConfig(HotUpdateRes res)
        {
            AssetDataTable remoteDataTable = new AssetDataTable();

            remoteDataTable.LoadPackageFromFile(res.destionResPath);

            m_UpdateUnitList = CalculateUpdateList(AssetDataTable.S, remoteDataTable);

            if (m_CheckListener != null)
            {
                m_CheckListener();
                m_CheckListener = null;
            }
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
