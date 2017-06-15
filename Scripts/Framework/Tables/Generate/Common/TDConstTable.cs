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
    public partial class TDConst
    {
        private bool m_IsInited;

        private EInt m_IntVal = 0;
        private EFloat m_FloatVal = 0f;

        public int IntVal
        {
            get
            {
                return m_IntVal;
            }
        }

        public float FloatVal
        {
            get
            {
                return m_FloatVal;
            }
        }

        public string StringVal
        {
            get
            {
                return m_Value;
            }
        }
    }

    public static partial class TDConstTable
    {
        private static TDTableMetaData m_MetaData = new TDTableMetaData(TDConstTable.Parse, "const");
        public static TDTableMetaData metaData
        {
            get { return m_MetaData; }
        }

        private static Dictionary<string, TDConst> m_DataCache = new Dictionary<string, TDConst>();
        private static TDConst[] m_DataArray;


        public static void Parse(byte[] fileData)
        {
            m_DataCache.Clear();

            DataStreamReader dataR = new DataStreamReader(fileData);

            int rowCount = dataR.GetRowCount();

            int[] fieldIndex = dataR.GetFieldIndex(TDConst.GetFieldHeadIndex());

#if (UNITY_STANDALONE_WIN) || UNITY_EDITOR || UNITY_STANDALONE_OSX

            dataR.CheckFieldMatch(TDConst.GetFieldHeadIndex(), "TDConstTable");

#endif

            for (int i = 0; i < rowCount; ++i)

            {

                TDConst memberInstance = new TDConst();

                memberInstance.ReadRow(dataR, fieldIndex);

                OnAddRow(memberInstance);

            }

            Log.i(string.Format("Parse Success TDConst"));
        }

        public static void OnAddRow(TDConst memberInstance)
        {
            memberInstance.Reset();

            string strKey = memberInstance.key;

            if (m_DataCache.ContainsKey(strKey))
            {
                Log.e(string.Format("Invaild,  TDConstTable Id already exists {0}", strKey));
            }
            else
            {
                m_DataCache.Add(strKey, memberInstance);
            }
        }

        public static void Reload(byte[] fileData)
        {
            Parse(fileData);
        }

        public static int Count
        {
            get
            {
                return m_DataCache.Count;
            }
        }

        public static int QueryInt<T>(T type, int defaultValue = 0) where T : IConvertible
        {
            TDConst td = GetData(type.ToInt32(null));

            if (td != null)
            {
                return td.IntVal;
            }

            return defaultValue;
        }

        public static float QueryFloat<T>(T type, float defaultValue = 0) where T : IConvertible
        {
            TDConst td = GetData(type.ToInt32(null));

            if (td != null)
            {
                return td.FloatVal;
            }

            return defaultValue;
        }

        public static string QueryString<T>(T type, string defaultValue = "") where T : IConvertible
        {
            TDConst td = GetData(type.ToInt32(null));

            if (td != null)
            {
                return td.StringVal;
            }

            return defaultValue;
        }

        public static void InitArrays(Type type)
        {
            Array enumValues = Enum.GetValues(type);

            int maxCount = (int)enumValues.GetValue(enumValues.Length - 1) + 1;
            m_DataArray = new TDConst[maxCount];

            for (int i = 0; i < maxCount; ++i)
            {
                string name = Enum.GetName(type, i);

                if (m_DataCache.ContainsKey(name))
                {
                    m_DataArray[i] = m_DataCache[name];
                }
                else
                {
                    m_DataArray[i] = null;
                }
            }
        }

        static TDConst GetData(int index)
        {
            if (index >= 0 && index < m_DataArray.Length)
            {
                return m_DataArray[index];
            }
            else
            {
                Log.e(string.Format("Can't find key {0} in TDConst", index));
                return null;
            }
        }
    }
}
