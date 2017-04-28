using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PTGame.Framework
{
    public class ResPackageHandler
    {
        private bool m_UpdateResult;
        private ResPackage m_Package;
        private List<ABUnit> m_UpdateUnitList;
        private ResLoader m_Loader;
        private Action<ResPackageHandler> m_CheckListener;
        private Action<ResPackageHandler> m_DownloadListener;
        private Action<ResPackageHandler> m_UpdateListener;

        public bool updateResult
        {
            get { return m_UpdateResult; }
        }

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
        }

        public void CheckUpdateList(Action<ResPackageHandler> callBack)
        {
            if (m_Loader != null)
            {
                Log.w("Package Handler is Working.");
                return;
            }

            m_Loader = ResLoader.Allocate("ResPackageHolder");

            m_CheckListener = callBack;
            string resName = ResUpdateMgr.AssetName2ResName(m_Package.configFile);
            m_Loader.Add2Load(resName, OnRemoteABConfigDownloadFinish);

            HotUpdateRes hotUpdateRes = ResMgr.S.GetRes<HotUpdateRes>(resName);

            string fullPath = FilePath.persistentDownloadCachePath + m_Package.configFile;
            hotUpdateRes.SetUpdateInfo(fullPath, m_Package.GetAssetUrl(m_Package.configFile));

            if (m_Loader != null)
            {
                m_Loader.LoadAsync();
            }
        }

        public void MoveABConfig2Use()
        {
            string sourceFile = FilePath.persistentDownloadCachePath + m_Package.configFile;
            string destFile = FilePath.persistentDataPath4Res + m_Package.configFile;

            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }

            File.Move(sourceFile, destFile);
        }

        public void StartUpdate(Action<ResPackageHandler> callBack)
        {
            if (m_UpdateUnitList == null || m_UpdateUnitList.Count == 0)
            {
                Log.i("No Update List For Update");
                callBack(this);
                return;
            }

            if (m_Loader != null)
            {
                Log.w("Package Handler is Working.");
                return;
            }

            m_Loader = ResLoader.Allocate("ResPackageHolder");

            m_UpdateListener = callBack;

            for (int i = 0; i < m_UpdateUnitList.Count; ++i)
            {
                string resName = ResUpdateMgr.AssetName2ResName(m_UpdateUnitList[i].abName);
                m_Loader.Add2Load(resName);
                HotUpdateRes res = ResMgr.S.GetRes<HotUpdateRes>(resName);
                string relativePath = m_Package.GetABLocalRelativePath(m_UpdateUnitList[i].abName);
                string fullPath = FilePath.persistentDataPath4Res + relativePath;
                res.SetUpdateInfo(fullPath, m_Package.GetAssetUrl(relativePath));
            }

            m_Loader.LoadAsync(OnPackageUpdateFinish);
        }

        public void DownloadPackage(Action<ResPackageHandler> callback)
        {
            if (m_Loader != null)
            {
                Log.w("Package Handler is Working.");
                return;
            }

            m_Loader = ResLoader.Allocate("ResPackageHolder");

            m_DownloadListener = callback;
            string resName = ResUpdateMgr.AssetName2ResName(m_Package.packageName);
            m_Loader.Add2Load(resName, OnPackageDownloadFinish);

            HotUpdateRes hotUpdateRes = ResMgr.S.GetRes<HotUpdateRes>(resName);

            //这里换文件地址

            string fullPath = FilePath.persistentDownloadCachePath + m_Package.relativeLcalParentFolder + m_Package.zipFileName;
            hotUpdateRes.SetUpdateInfo(fullPath, m_Package.zipUrl);
            if (m_Loader != null)
            {
                m_Loader.LoadAsync();
            }
        }

        public void UnZipPackage(Action<ResPackageHandler> callback)
        {
            string zipFilePath = FilePath.persistentDownloadCachePath + m_Package.relativeLcalParentFolder + m_Package.zipFileName;
            string targetFolder = FilePath.persistentDataPath4Res + m_Package.relativeLcalParentFolder;
            
            ZipMgr.S.UnZip(zipFilePath, targetFolder, null, null, null);
        }

        private void OnPackageDownloadFinish(bool result, IRes res)
        {
            ClearLoader();

            if (m_DownloadListener != null)
            {
                m_DownloadListener(this);
                m_DownloadListener = null;
            }
        }

        private void OnPackageUpdateFinish()
        {
            m_UpdateResult = false;
            if (m_Loader != null)
            {
                m_UpdateResult = m_Loader.IsAllResLoadSuccess();

                if (m_UpdateResult)
                {
                    m_UpdateUnitList.Clear();
                    m_UpdateUnitList = null;
                }
            }

            ClearLoader();

            if (m_UpdateListener != null)
            {
                m_UpdateListener(this);
                m_UpdateListener = null;
            }
        }

        private void OnRemoteABConfigDownloadFinish(bool result, IRes res)
        {
            if (!result)
            {
                Log.e("Download remote abConfig File Failed.");
                ClearLoader();
                return;
            }

            HotUpdateRes hotUpdateRes = res as HotUpdateRes;

            if (hotUpdateRes == null)
            {
                ClearLoader();
                return;
            }

            //所有AB更新完成后需要替换当前的Config文件，启用独立的ResLoader来完成
            ProcessRemoteABConfig(hotUpdateRes);
        }

        private void ClearLoader()
        {
            if (m_Loader != null)
            {
                m_Loader.Recycle2Cache();
                m_Loader = null;
            }
        }

        private void ProcessRemoteABConfig(HotUpdateRes res)
        {
            AssetDataTable remoteDataTable = new AssetDataTable();

            remoteDataTable.LoadPackageFromFile(res.localResPath);

            m_UpdateUnitList = CalculateUpdateList(AssetDataTable.S, remoteDataTable);

            ClearLoader();

            if (m_CheckListener != null)
            {
                m_CheckListener(this);
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

        public void Dump()
        {
            if (m_UpdateUnitList == null)
            {
                Log.i("Not Need 2 Update");
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("#Package:" + m_Package.packageName);
            for (int i = 0; i < m_UpdateUnitList.Count; ++i)
            {
                builder.AppendLine("    :" + m_UpdateUnitList[i].ToString());
            }
            Log.i(builder.ToString());
        }
    }
}
