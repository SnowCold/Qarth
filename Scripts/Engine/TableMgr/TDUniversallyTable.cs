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
using System.Runtime.InteropServices;

namespace Qarth
{
    public class TDTableMetaData
    {
        private DTableOnParse m_OnParse;
        public DTableOnParse onParse
        {
            get
            {
                return m_OnParse;
            }
        }

        public string tableName { get; set;}
        public TDTableMetaData (DTableOnParse onParse, string tName)
        {
            m_OnParse = onParse;
            tableName = tName;
        }
    }

    public class TDUniversallySchemeHeader
    {
        public Dictionary<string, int>         fieldTypeIndex = new Dictionary<string, int>();
        public DataStreamReader.FieldType[]    fieldTypes = null;
        public string                          tableName;

        public int GetFieldTypeIndex(string fieldName)
        {
            if (fieldTypeIndex.ContainsKey (fieldName)) 
            {
                return fieldTypeIndex[fieldName];
            }
            Debug.Assert (false);
            return 0;
        }
    }
    /*
    public partial class TDUniversally
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct FieldData
        {
            [FieldOffset(0)]
            public DataStreamReader.FieldType fieldType;
            [FieldOffset(0)]
            private EInt intVal;
            [FieldOffset(0)]
            private EFloat floatVal;
            [FieldOffset(0)]
            private string stringVal;
            [FieldOffset(0)]
            private Vector2 vector2Val;
            [FieldOffset(0)]
            private Vector3 vector3Val;

            public FieldData(bool val)
            {
                fieldType = DataStreamReader.FieldType.Int;
                intVal = val?1:0;
                floatVal = 0.0f;
                stringVal = string.Empty;
                vector2Val = Vector2.zero;
                vector3Val = Vector3.zero;
            }

            public FieldData(int val)
            {
                fieldType = DataStreamReader.FieldType.Int;
                intVal = val;
                floatVal = 0.0f;
                stringVal = string.Empty;
                vector2Val = Vector2.zero;
                vector3Val = Vector3.zero;
            }

            public FieldData(float val)
            {
                fieldType = DataStreamReader.FieldType.Float;
                intVal = 0;
                floatVal = val;
                stringVal = string.Empty;
                vector2Val = Vector2.zero;
                vector3Val = Vector3.zero;
            }

            public FieldData(string val)
            {
                fieldType = DataStreamReader.FieldType.String;
                intVal = 0;
                floatVal = 0.0f;
                stringVal = val;
                vector2Val = Vector2.zero;
                vector3Val = Vector3.zero;
            }


            public FieldData(Vector2 val)
            {
                fieldType = DataStreamReader.FieldType.Vector2;
                intVal = 0;
                floatVal = 0.0f;
                stringVal = string.Empty;
                vector2Val = val;
                vector3Val = Vector3.zero;
            }


            public FieldData(Vector3 val)
            {
                fieldType = DataStreamReader.FieldType.Vector2;
                intVal = 0;
                floatVal = 0.0f;
                stringVal = string.Empty;
                vector2Val = Vector3.zero;
                vector3Val = val;
            }

            public static implicit operator FieldData(int value)
            {
                return (new FieldData(value));
            }

            public static implicit operator FieldData(float value)
            {
                return (new FieldData(value));
            }

            public static implicit operator FieldData(string value)
            {
                return (new FieldData(value));
            }

            public static implicit operator int(FieldData value)
            {
                return value.intVal;
            }

            public static implicit operator float(FieldData value)
            {
                return value.floatVal;
            }

            public static implicit operator string(FieldData value)
            {
                return value.stringVal;
            }

            public static implicit operator Vector2(FieldData value)
            {
                return value.vector2Val;
            }

            public static implicit operator Vector3(FieldData value)
            {
                return value.vector3Val;
            }
        }

        private FieldData[] m_ListData;
        private TDUniversallySchemeHeader m_SchemeHeader;


        public FieldData[] listData { 
            get
            {
                return m_ListData;
            }
        }

        public void SetSchemeHeader(TDUniversallySchemeHeader schemeHeader)
        {
            m_SchemeHeader = schemeHeader;
        }
            
        public void ReadRow(DataStreamReader dataR)
        {
            m_ListData = new FieldData[dataR.GetRowCount ()];
            int col = 0;
            while(true)
            {
                col = dataR.MoreFieldOnRow();
                if (col == -1)
                {
                    break;
                }
                switch (dataR.NextFiledType())
                { 
                case DataStreamReader.FieldType.Int:
                    m_ListData[col] = dataR.ReadInt();

                    break;
                case DataStreamReader.FieldType.Float:
                    m_ListData[col] = dataR.ReadFloat();

                    break;
                case DataStreamReader.FieldType.String:
                    m_ListData[col] = dataR.ReadString();

                    break;
                default:
                    dataR.SkipField();
                    break;
                }
            }
        } 

        public DataStreamReader.FieldType GetFieldTypeInNew(string fieldName)
        {
            if (m_SchemeHeader.fieldTypeIndex.ContainsKey (fieldName)) 
            {
                int index = m_SchemeHeader.fieldTypeIndex [fieldName]; 
                return m_SchemeHeader.fieldTypes[index];
            }
            return DataStreamReader.FieldType.Unkown;
        }
    }


    public partial class TDUniversallyTable
    {
        
        private Dictionary<int, TDUniversally> m_DataCache = new Dictionary<int, TDUniversally>();
        private List<TDUniversally> m_DataList = new List<TDUniversally>();
        TDUniversallySchemeHeader m_SchemeHeader = new TDUniversallySchemeHeader();

        public static Dictionary<string, TDUniversallyTable> tableClassDict = new Dictionary<string, TDUniversallyTable> ();

        public TDUniversallyTable(string tableName)
        {
            m_SchemeHeader.tableName = tableName;
            if (tableClassDict.ContainsKey (tableName) == false)
            {
                tableClassDict.Add(tableName, this);
            }
            else
            {
                Debug.AssertFormat(false, "{0} aleady Register Parse", tableName);
            }
        }

        public static void ClearRegisterTableClassDict()
        {
            tableClassDict.Clear();
        }

        private void BuildFieldTypeIndex(string[] fieldSchemeName)
        {
            for (int i = 0; i < fieldSchemeName.Length; ++i) {
                m_SchemeHeader.fieldTypeIndex.Add (fieldSchemeName[i], i);
            }
        }

        public  void Parse(byte[] fileData)
        {
            m_DataCache.Clear();
            m_DataList.Clear();
            DataStreamReader dataR = new DataStreamReader(fileData);
            int rowCount = dataR.GetRowCount();
            BuildFieldTypeIndex (dataR.GetSchemeName ());
            m_SchemeHeader.fieldTypes = dataR.fieldTypes;

            for (int i = 0; i < rowCount; ++i)
            {
                TDUniversally memberInstance = new TDUniversally();
                memberInstance.ReadRow(dataR);
                OnAddRow(memberInstance);
                memberInstance.SetSchemeHeader (m_SchemeHeader);
            }
            Log.i(string.Format("Parse Success {0}", m_SchemeHeader.tableName));
        }

        private  void OnAddRow(TDUniversally memberInstance)
        {
            int key = memberInstance.listData[m_SchemeHeader.GetFieldTypeIndex("Id")];
            if (m_DataCache.ContainsKey(key))
            {
                Log.e(string.Format("Invaild,  TD{0}Table Id aPTGame.Frameworkeady exists {1}", m_SchemeHeader.tableName, key));
            }
            else
            {
                m_DataCache.Add(key, memberInstance);
                m_DataList.Add(memberInstance);
            }
        }    
        
        public  void Reload(byte[] fileData)
        {
            Parse(fileData);
        }

        public  int Count
        {
            get 
            {
                return m_DataCache.Count;
            }
        }


        public TDUniversally GetDataByIndex(int index)
        {
            if (index >= 0 && index < m_DataList.Count) {
                return m_DataList [index];
            }
            return null;
        }

        private TDUniversally GetData(int id)
        {
            if (m_DataCache.ContainsKey(id))
            {
                return m_DataCache[id];
            }
            else
            {
                Log.e(string.Format("Can't find key {0} in TD{1}", id, m_SchemeHeader.tableName));
                return null;
            }
        }
    }
    */
}
