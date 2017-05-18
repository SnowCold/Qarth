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
        private List<ABUnit> m_NeedUpdateFileList;
        private Dictionary<string, ABUnit> m_UpdateUnitMap;

        private ResLoader m_Loader;
        private Action<ResPackageHandler> m_CheckListener;
        private Action<ResPackageHandler> m_DownloadListener;
        private Action<ResPackageHandler> m_UpdateListener;
        private ResUpdateRecord m_Record;
        private List<ABUnit> m_UpdateFailedList = new List<ABUnit>();
        private float m_NeedUpdateFileSize = -1;
        private float m_AlreadyUpdateFileSize;
        private int m_AlreadyUpdateFileCount;

        public static string AssetName2ResName(string assetName)
        {
            return string.Format("HotUpdateRes:{0}", assetName);
        }

        public bool updateResult
        {
            get { return m_UpdateResult; }
        }

        public ResPackage package
        {
            get { return m_Package; }
        }

        public bool needUpdate
        {
            get
            {
                if (m_NeedUpdateFileList == null || m_NeedUpdateFileList.Count == 0)
                {
                    return false;
                }

                return true;
            }
        }

        public int needUpdateFileCount
        {
            get
            {
                if (m_NeedUpdateFileList == null)
                {
                    return 0;
                }

                return m_NeedUpdateFileList.Count;
            }
        }

        public int alreadyUpdateFileCount
        {
            get { return m_AlreadyUpdateFileCount; }
        }

        public float alreadyUpdateFileSize
        {
            get
            {
                if (alreadyUpdateFileCount < needUpdateFileCount)
                {
                    return m_AlreadyUpdateFileSize + WWWDownloader.S.alreadyDownloadByte;
                }
                return m_AlreadyUpdateFileSize;
            }
        }

        public float needUpdateFileSize
        {
            get
            {
                if (m_NeedUpdateFileSize < 0)
                {
                    m_NeedUpdateFileSize = 0;

                    for (int i = 0; i < m_NeedUpdateFileList.Count; ++i)
                    {
                        m_NeedUpdateFileSize += m_NeedUpdateFileList[i].fileSize;
                    }
                }

                return m_NeedUpdateFileSize;
            }
        }

        public string currentUpdateFile
        {
            get
            {
                return WWWDownloader.S.targetFile;
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

            ApplyUpdateRecord();

            m_Loader = ResLoader.Allocate("ResPackageHolder");

            m_CheckListener = callBack;
            string resName = AssetName2ResName(m_Package.configFile);
            m_Loader.Add2Load(resName, OnRemoteABConfigDownloadFinish);

            HotUpdateRes hotUpdateRes = ResMgr.S.GetRes<HotUpdateRes>(resName);

            string fullPath = FilePath.persistentDownloadCachePath + m_Package.configFile;
            hotUpdateRes.SetUpdateInfo(fullPath, m_Package.GetAssetUrl(m_Package.configFile));

            if (m_Loader != null)
            {
                m_Loader.LoadAsync();
            }
        }

        //使用已更新状态修正本地清单
        private void ApplyUpdateRecord()
        {
            if (m_Record != null)
            {
                m_Record.Close();
                m_Record = null;
            }

            m_Record = new ResUpdateRecord(m_Package);
            m_Record.Load();

            m_Record.ModifyAssetDataTable(AssetDataTable.S);

            m_Record.Close();
        }

        private void MoveABConfig2Use()
        {
            string sourceFile = FilePath.persistentDownloadCachePath + m_Package.configFile;
            string destFile = FilePath.persistentDataPath4Res + m_Package.configFile;

            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }

            File.Move(sourceFile, destFile);
        }

        private void InnerStartUpdate(List<ABUnit> updateList)
        {
            if (updateList == null || updateList.Count == 0)
            {
                return;
            }

            if (m_Loader != null)
            {
                Log.w("Package Handler is Working.");
                return;
            }

            m_Loader = ResLoader.Allocate("ResPackageHolder");

            if (m_UpdateUnitMap == null)
            {
                m_UpdateUnitMap = new Dictionary<string, ABUnit>();
            }
            else
            {
                m_UpdateUnitMap.Clear();
            }

            for (int i = 0; i < updateList.Count; ++i)
            {
                string resName = AssetName2ResName(updateList[i].abName);

                m_UpdateUnitMap.Add(resName, updateList[i]);

                m_Loader.Add2Load(resName, OnResUpdateFinish);
                HotUpdateRes res = ResMgr.S.GetRes<HotUpdateRes>(resName);
                string relativePath = m_Package.GetABLocalRelativePath(updateList[i].abName);
                string fullPath = FilePath.persistentDataPath4Res + relativePath;
                res.SetUpdateInfo(fullPath, m_Package.GetAssetUrl(relativePath));
            }

            m_Loader.LoadAsync(OnPackageUpdateFinish);
        }

        public void StartUpdate(Action<ResPackageHandler> callBack)
        {
            if (m_NeedUpdateFileList == null || m_NeedUpdateFileList.Count == 0)
            {
                Log.i("No Update List For Update");
                callBack(this);
                return;
            }

            m_UpdateListener = callBack;

            m_AlreadyUpdateFileCount = 0;
            m_AlreadyUpdateFileSize = 0;
            m_NeedUpdateFileSize = -1;

            InnerStartUpdate(m_NeedUpdateFileList);
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
            string resName = AssetName2ResName(m_Package.packageName);
            m_Loader.Add2Load(resName, OnPackageDownloadFinish);

            HotUpdateRes hotUpdateRes = ResMgr.S.GetRes<HotUpdateRes>(resName);

            string fullPath = FilePath.persistentDownloadCachePath + m_Package.relativeLocalParentFolder + m_Package.zipFileName;
            hotUpdateRes.SetUpdateInfo(fullPath, m_Package.zipUrl);
            if (m_Loader != null)
            {
                m_Loader.LoadAsync();
            }
        }

        public void UnZipPackage(Action<ResPackageHandler> callback)
        {
            string zipFilePath = FilePath.persistentDownloadCachePath + m_Package.relativeLocalParentFolder + m_Package.zipFileName;
            string targetFolder = FilePath.persistentDataPath4Res + m_Package.relativeLocalParentFolder;
            
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

        private void OnResUpdateFinish(bool result, IRes res)
        {
            ABUnit unit = null;
            if (m_UpdateUnitMap.TryGetValue(res.name, out unit))
            {

                if (!result)
                {
                    Log.e("Update Res Failed:" + res.name);
                    m_UpdateFailedList.Add(unit);
                    return;
                }

                m_AlreadyUpdateFileSize += unit.fileSize;
                ++m_AlreadyUpdateFileCount;

                m_Record.AddRecord(unit.abName, unit.md5, unit.fileSize, unit.buildTime);
            }
        }

        private void OnFailedStartTimeReach(int count)
        {
            Log.w("## Try Start Update Failed Res.");
            InnerStartUpdate(m_UpdateFailedList);
            m_UpdateFailedList.Clear();
        }

        private void OnPackageUpdateFinish()
        {
            if (m_UpdateFailedList.Count > 0)
            {
                ClearLoader();
                Log.w("## Try Start Update Failed Res.");
                InnerStartUpdate(m_UpdateFailedList);
                m_UpdateFailedList.Clear();
                return;
            }

            m_UpdateResult = false;

            if (m_Loader != null)
            {
                m_UpdateResult = m_Loader.IsAllResLoadSuccess();

                if (m_UpdateResult)
                {
                    m_NeedUpdateFileList.Clear();
                    m_NeedUpdateFileList = null;

                    m_UpdateUnitMap.Clear();
                    m_UpdateUnitMap = null;
                }
            }

            ClearLoader();

            MoveABConfig2Use();

            if (m_Record != null)
            {
                m_Record.Delete();
            }

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

                if (m_CheckListener != null)
                {
                    m_CheckListener(this);
                    m_CheckListener = null;
                }
                return;
            }

            HotUpdateRes hotUpdateRes = res as HotUpdateRes;

            if (hotUpdateRes == null)
            {
                ClearLoader();

                if (m_CheckListener != null)
                {
                    m_CheckListener(this);
                    m_CheckListener = null;
                }
                return;
            }

            ProcessRemoteABConfig(hotUpdateRes);

            if (m_CheckListener != null)
            {
                m_CheckListener(this);
                m_CheckListener = null;
            }
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

            try
            {
                remoteDataTable.LoadPackageFromFile(res.localResPath);
            }
            catch (Exception e)
            {
                Log.e(e);
            }
            m_NeedUpdateFileList = ABUnitHelper.CalculateLateList(AssetDataTable.S, remoteDataTable, true);

            if (m_Package.updateBlackList != null)
            {
                var list = m_Package.updateBlackList;

                for (int i = 0; i < list.Count; ++i)
                {
                    for (int j = m_NeedUpdateFileList.Count - 1; j >= 0; --j)
                    {
                        if (m_NeedUpdateFileList[j].abName.Equals(list[i]))
                        {
                            m_NeedUpdateFileList.RemoveAt(j);
                            break;
                        }
                    }
                }
            }

            ClearLoader();
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

        public void Dump()
        {
            if (m_NeedUpdateFileList == null)
            {
                Log.i("Not Need 2 Update");
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("#Package:" + m_Package.packageName);
            for (int i = 0; i < m_NeedUpdateFileList.Count; ++i)
            {
                builder.AppendLine("    :" + m_NeedUpdateFileList[i].ToString());
            }
            Log.i(builder.ToString());
        }
    }
}
