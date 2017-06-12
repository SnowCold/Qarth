//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SCEngine
{
    [TMonoSingletonAttribute("[Tools]/ResMgr")]
    public class ResMgr : TMonoSingleton<ResMgr>, IEnumeratorTaskMgr
    {
        private const string INNER_RES_BUILDTIME = "eirv_1988520905";
        private const string INNER_RES_PACKAGE = "eirp_1988520905";

#region 字段
        private Dictionary<string, IRes>    m_ResDictionary = new Dictionary<string, IRes>();
        private List<IRes>                  m_ResList = new List<IRes>();
        [SerializeField]
        private int                         m_CurrentCoroutineCount = 0;
        private int                         m_MaxCoroutineCount = 8;//最快协成大概在6到8之间
        private TimeDebugger                m_TimeDebugger;
        private LinkedList<IEnumeratorTask> m_IEnumeratorTaskStack = new LinkedList<IEnumeratorTask>();

        private bool                        m_IsWorking = true;
        //Res 在ResMgr中 删除的问题，ResMgr定时收集列表中的Res然后删除
        private bool                        m_IsResMapDirty = false;

        #endregion

        public int totalResCount
        {
            get { return m_ResList.Count; }
        }

        public override void OnSingletonInit()
        {
            ReloadABTable();
        }

        public void Dump()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("## BEGIN DUMP ALL AssetBundle State");
            builder.AppendLine("# ActiveCount:" + AssetBundleRes.s_ActiveCount);

            for (int i = 0; i < m_ResList.Count; ++i)
            {
                if (m_ResList[i] is AssetBundleRes)
                {
                    builder.AppendLine("    #ABName:" + m_ResList[i].name);
                }
            }

            builder.AppendLine("## END DUMP ALL AssetBundle State");

            Log.i(builder.ToString());
        }

        public void ReloadABTable()
        {
            AssetDataTable.S.Reset();
            List<string> outResult = new List<string>();

            //首先加载Inner的Config
            FileMgr.S.GetFileInInner(ProjectPathConfig.abConfigfileName, outResult);
            for (int i = 0; i < outResult.Count; ++i)
            {
                AssetDataTable.S.LoadPackageFromFile(outResult[i]);
            }

            ProcessNewInstall();

            //然后加载外存中的，如果存在同名Package则直接替换
            outResult.Clear();

            FilePath.GetFileInFolder(FilePath.persistentDataPath4Res, ProjectPathConfig.abConfigfileName, outResult);
            for (int i = 0; i < outResult.Count; ++i)
            {
                AssetDataTable.S.LoadPackageFromFile(outResult[i]);
            }

            AssetDataTable.S.SwitchLanguage("cn");
        }

        private void ProcessNewInstall()
        {
            bool isNewInstall = CheckIsNewInstall();
            Log.i("Check Is New Install:" + isNewInstall);

            if (isNewInstall)
            {
                //对比外部文件
                List<string> outResult = new List<string>();
                FilePath.GetFileInFolder(FilePath.persistentDataPath4Res, ProjectPathConfig.abConfigfileName, outResult);

                AssetDataTable exterTable = new AssetDataTable();
                for (int i = 0; i < outResult.Count; ++i)
                {
                    exterTable.LoadPackageFromFile(outResult[i]);
                }

                //生成差异文件列表:
                List<ABUnit> needDeleteList = ABUnitHelper.CalculateLateList(exterTable, AssetDataTable.S, false);

                for (int i = 0; i < needDeleteList.Count; ++i)
                {
                    ABUnit unit = needDeleteList[i];
                    string exterFilePath = ProjectPathConfig.AssetBundleName2ExterUrl(unit.abName);
                    if (File.Exists(exterFilePath))
                    {
                        File.Delete(exterFilePath);
                    }
                }

                var packages = AssetDataTable.S.allAssetDataPackages;
                if (packages.Count > 0)
                {
                    var package = packages[0];
                    PlayerPrefs.SetString(INNER_RES_PACKAGE, package.key);
                    PlayerPrefs.SetInt(INNER_RES_BUILDTIME, (int)package.buildTime);
                    PlayerPrefs.Save();
                }
            }
        }

        //检测当前版本是否是新装版本,如果安装了新版本，则需要对比内外部资源
        //使用默认assetpackage的创建时间来
        private bool CheckIsNewInstall()
        {
            string defaultPackage = PlayerPrefs.GetString(INNER_RES_PACKAGE, "");

            bool isNewPackage = false;
            if (string.IsNullOrEmpty(defaultPackage))
            {
                //重来没装过，或者卸载过，那么此时一定是新装版本
                isNewPackage = true;
            }
            else
            {
                int buildTime = PlayerPrefs.GetInt(INNER_RES_BUILDTIME);

                AssetDataPackage package = AssetDataTable.S.GetAssetDataPackage(defaultPackage);
                if (package == null)
                {
                    //新包连这个package都没了，那么一定重新装过
                    isNewPackage = true;
                }
                else
                {
                    int pBuildTime = (int)package.buildTime;
                    int offset = pBuildTime - buildTime;

                    if (offset < -1 || offset > 1)
                    {
                        isNewPackage = true;
                    }
                }
            }

            return isNewPackage;
        }

        public void InitResMgr()
        {
            Log.i("Init[ResMgr]");
        }

        #region 属性
        public TimeDebugger timeDebugger
        {
            get
            {
                if (m_TimeDebugger == null)
                {
                    m_TimeDebugger = new TimeDebugger("#Res");
                }
                return m_TimeDebugger;
            }
        }

        public void SetResMapDirty()
        {
            m_IsResMapDirty = true;
        }

        public void PostIEnumeratorTask(IEnumeratorTask task)
        {
            if (task == null)
            {
                return;
            }

            m_IEnumeratorTaskStack.AddLast(task);
            TryStartNextIEnumeratorTask();
        }

        public IRes GetRes(string name, bool createNew = false)
        {
            IRes res = null;
            if (m_ResDictionary.TryGetValue(name, out res))
            {
                return res;
            }

            if (!createNew)
            {
                return null;
            }

            res = ResFactory.Create(name);

            if (res != null)
            {
                m_ResDictionary.Add(name, res);
                m_ResList.Add(res);
            }
            return res;
        }

        public R GetRes<R>(string name) where R : IRes
        {
            IRes res = null;
            if (m_ResDictionary.TryGetValue(name, out res))
            {
                return (R)res;
            }

            return default(R);
        }

        public R GetAsset<R>(string name) where R : UnityEngine.Object
        {
            IRes res = null;
            if (m_ResDictionary.TryGetValue(name, out res))
            {
                return res.asset as R;
            }

            return null;
        }

#endregion

#region Private Func

        private void Update()
        {
            if (m_IsWorking)
            {
                if (m_IsResMapDirty)
                {
                    RemoveUnusedRes();
                }
            }
        }

        private void RemoveUnusedRes()
        {
            if (!m_IsResMapDirty)
            {
                return;
            }

            m_IsResMapDirty = false;

            IRes res = null;
            for (int i = m_ResList.Count - 1; i >= 0; --i)
            {
                res = m_ResList[i];
                if (res.refCount <= 0 && res.resState != eResState.kLoading)
                {
                    if (res.ReleaseRes())
                    {
                        m_ResList.RemoveAt(i);
                        m_ResDictionary.Remove(res.name);
                        res.Recycle2Cache();
                    }
                }
            }
        }

        private void OnIEnumeratorTaskFinish()
        {
            --m_CurrentCoroutineCount;
            TryStartNextIEnumeratorTask();
        }

        private void TryStartNextIEnumeratorTask()
        {
            if (m_IEnumeratorTaskStack.Count == 0)
            {
                return;
            }

            if (m_CurrentCoroutineCount >= m_MaxCoroutineCount)
            {
                return;
            }

            IEnumeratorTask task = m_IEnumeratorTaskStack.First.Value;
            m_IEnumeratorTaskStack.RemoveFirst();

            ++m_CurrentCoroutineCount;
            StartCoroutine(task.StartIEnumeratorTask(OnIEnumeratorTaskFinish));
        }
#endregion
    }
}
