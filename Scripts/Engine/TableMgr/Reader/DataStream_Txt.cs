//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Net;
using System.Collections.Generic;
using UnityEngine;

#if !USE_TABLE_XC 
//&& (UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_STANDALONE_OSX)
namespace Qarth
{
    public class DataStreamReader
    {
        //Don't Edit it (����Ԥ��,��Ҫ�༭)
        public enum FieldType
        {
            Unkown,
            Int,
            Float,
            String,
            Vector2,
            Vector3,
            Bool,
        }
        private TxtReader   m_TxtReader;
        private string[]    m_SchemeNames;
        private int         m_AllLen;
        private int         m_Col;
        private int         m_CurRow = 0;
        private int         m_CurCol;
        private const int   FILETYPE_ROW = 1;
        private const int   FIELDNAME_ROW = 2;
        private const int   SCHEME_ROW_LEN = 3;
        private FieldType[] m_FieldTypes;

        public FieldType[] fieldTypes
        {
            get { return m_FieldTypes; }
        }

        public DataStreamReader(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            m_TxtReader = new TxtReader();
            m_TxtReader.ParseData(System.Text.UTF8Encoding.UTF8.GetString(data), 0, FIELDNAME_ROW);
            m_SchemeNames = m_TxtReader.GetColName();
            m_Col = m_TxtReader.GetCols();
            PreReadFieldType();
            SkipSchemeName();
        }

        private void SkipSchemeName()
        {
            m_CurRow += SCHEME_ROW_LEN;
        }

        public int GetRowCount()
        {
            return m_TxtReader.GetRows() - SCHEME_ROW_LEN;
        }

        public string[] GetSchemeName()
        {
            return m_SchemeNames;
        }

        // ��ȡtype����
        private void PreReadFieldType()
        {
            m_FieldTypes = new FieldType[m_Col];
            for (int i = 0; i < m_FieldTypes.Length; ++i)
            {
                string ret = m_TxtReader.ReadString(FILETYPE_ROW, m_SchemeNames[i]);
                switch (ret)
                {
                    case "bool":
                        m_FieldTypes[i] = FieldType.Bool;
                        break;
                    case "int":
                        m_FieldTypes[i] = FieldType.Int;
                        break;
                    case "float":
                        m_FieldTypes[i] = FieldType.Float;
                        break;
                    case "string":
                        m_FieldTypes[i] = FieldType.String;
                        break;
                    default:
                        m_FieldTypes[i] = FieldType.Unkown;
                        break;
                }
            }
        }

        public FieldType NextFiledType()
        {
            return m_FieldTypes[m_CurCol];
        }

        public int[] GetFieldIndex(Dictionary<string, int> fieldSourceMap)
        {
            int[] ret = new int[m_Col];
            for (int i = 0; i < m_Col; ++i)
            {
                string key = m_SchemeNames[i];
                if (fieldSourceMap.ContainsKey(key))
                {
                    ret[i] = fieldSourceMap[key];
                }
                else
                {
                    ret[i] = -1;
                }
            }
            return ret;
        }

        public void CheckFieldMatch(Dictionary<string, int> fieldSourceMap, string tableName)
        {
            // Check New Field Add
            for (int i = 0; i < m_Col; ++i)
            {
                string key = m_SchemeNames[i];
                if (!fieldSourceMap.ContainsKey(key))
                {
                    Log.w(string.Format("Found a new Field :{0} In {1}",
                        key, tableName));
                }
            }

            //Check Filed Remove
            foreach (string key in fieldSourceMap.Keys)
            {
                bool findKey = false;
                for (int i = 0; i < m_SchemeNames.Length; ++i)
                {
                    if (m_SchemeNames[i].Equals(key))
                    {
                        findKey = true;
                        break;
                    }
                }
                if (!findKey)
                {
                    Log.e(string.Format("Field :{0}  removed In {1}",
                        key, tableName));
                }
            }
        }

        /// <summary>
        /// ����-1 ���ʾ������ĩβ
        /// </summary>
        /// <returns></returns>
        public int MoreFieldOnRow()
        {
            if (m_CurCol >= m_Col)
            {
                m_CurCol = 0;
                m_CurRow += 1;
                return -1;
            }
            else
            {
                return m_CurCol;
            }
        }

        public void SkipField()
        {
            m_CurCol += 1;
        }


        public Vector3 ReadVector3()
        {
            string str = ReadString();
            return Helper.String2Vector3(str);
        }

        public Vector2 ReadVector2()
        {
            string str = ReadString();
            return Helper.String2Vector2(str);
        }

        public int ReadInt()
        {
            int ret = m_TxtReader.ReadInt(m_CurRow, m_SchemeNames[m_CurCol]);
            m_CurCol += 1;
            return ret;
        }

        public bool ReadBool()
        {
            bool ret = m_TxtReader.ReadBool(m_CurRow, m_SchemeNames[m_CurCol]);
            m_CurCol += 1;
            return ret;

        }

        public float ReadFloat()
        {
            float ret = m_TxtReader.ReadFloat(m_CurRow, m_SchemeNames[m_CurCol]);
            m_CurCol += 1;
            return ret;
        }

        public string ReadString()
        {
            string ret = m_TxtReader.ReadString(m_CurRow, m_SchemeNames[m_CurCol]);
            m_CurCol += 1;
            return ret;
        }

    }
}
#endif
