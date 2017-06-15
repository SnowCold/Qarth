//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
    public static partial class TDLanguageTable
    {
        private static TDTableMetaData m_MetaData = new TDTableMetaData(TDLanguageTable.Parse, "language");
        public static TDTableMetaData metaData
        {
            get { return m_MetaData; }
        }
        
        private static Dictionary<string, TDLanguage> m_DataCache = new Dictionary<string, TDLanguage>();
        private static List<TDLanguage> m_DataList = new List<TDLanguage>();
        
        public static void Parse(byte[] fileData)
        {
            m_DataCache.Clear();
            m_DataList.Clear();
            if (fileData == null)
            {
                return;
            }

            DataStreamReader dataR = new DataStreamReader(fileData);
            int rowCount = dataR.GetRowCount();
            for (int i = 0; i < rowCount; ++i)
            {
                TDLanguage memberInstance = new TDLanguage();
                memberInstance.ReadRow(dataR, null);
                OnAddRow(memberInstance);
            }
            Log.i(string.Format("Parse Success TDLanguage"));
        }

        private static void OnAddRow(TDLanguage memberInstance)
        {
            string key = memberInstance.id;
            if (m_DataCache.ContainsKey(key))
            {
                Log.e(string.Format("Invaild,  TDLanguageTable Id already exists {0}", key));
            }
            else
            {
                m_DataCache.Add(key, memberInstance);
                m_DataList.Add(memberInstance);
            }
        }    
        
        public static void Reload(byte[] fileData)
        {
            Parse(fileData);
        }

        public static int count
        {
            get 
            {
                return m_DataCache.Count;
            }
        }

        public static List<TDLanguage> dataList
        {
            get 
            {
                return m_DataList;
            }    
        }

        public static bool TryGet(string key, out string value)
        {
            if (m_DataCache.ContainsKey(key))
            {
                value = m_DataCache[key].value;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public static string Get(string key)
        {
            if (!Helper.IsConfigStringValid(key))
            {
                return string.Empty;
            }

            if (m_DataCache.ContainsKey(key))
            {
                return m_DataCache[key].value;
            }
            else
            {
                Log.w(string.Format("Can't find key {0} in TDLanguage", key));
                return key;
            }
        }
    }
}//namespace LR
