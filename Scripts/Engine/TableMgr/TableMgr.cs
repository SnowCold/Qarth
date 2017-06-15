//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System.Collections;
using System;

namespace Qarth
{
    public class TableMgr : TSingleton<TableMgr>
    {
        // 表格读取进度
        private float       m_TableReadProgress;
        private bool        m_IsLoading = false;

        public float tableReadProgress
        {
            get { return m_TableReadProgress; }
        }

        public bool isLoading
        {
            get { return m_IsLoading; }
        }

        /// <summary>
        /// 预先读取Language Const表
        /// </summary>
        /// <returns></returns>
        public IEnumerator PreReadAll(Action onLoadFinish)
        {
            TableReadThreadWork readWork = CreateTableReadJobs(TableConfig.preLoadTableArray);

            readWork.Start();
            while (readWork.IsDone == false)
            {
                yield return 0;
            }

            if (onLoadFinish != null)
            {
                onLoadFinish();
            }
            yield return 0;
        }

        public IEnumerator ReadAll(TDTableMetaData[] dataArray, Action onLoadFinish)
        {
            m_IsLoading = true;
            TableReadThreadWork readWork = CreateTableReadJobs(dataArray);
            readWork.Start();
            while (readWork.IsDone == false)
            {
                m_TableReadProgress = readWork.finishedCount * 1.0f / readWork.readMaxCount * 1.0f;
                yield return 0;
            }

            m_IsLoading = false;

            if (onLoadFinish != null)
            {
                onLoadFinish();
            }
            yield return 0;
        }

        public void ReloadAll()
        {
            TableReadThreadWork readWork = CreateTableReadJobs(TableConfig.preLoadTableArray, TableConfig.delayLoadTableArray);
            readWork.Start();
            // 阻塞Reload
            while (readWork.IsDone == false)
            {

            }
        }

        private TableReadThreadWork CreateTableReadJobs(TDTableMetaData[] tableArrayA, TDTableMetaData[] tableArrayB = null)
        {
            TableReadThreadWork readWork = new TableReadThreadWork();
            if (tableArrayA != null)
            {
                for (int i = 0; i < tableArrayA.Length; ++i)
                {
                    readWork.AddJob(tableArrayA[i].tableName, tableArrayA[i].onParse);
                }
            }

            if (tableArrayB != null)
            {
                for (int i = 0; i < tableArrayB.Length; ++i)
                {
                    readWork.AddJob(tableArrayB[i].tableName, tableArrayA[i].onParse);
                }
            }

            return readWork;
        }
    }
}
