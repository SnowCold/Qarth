//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Net;
using System.Collections.Generic;

#if USE_TABLE_XC
namespace PTGame.Framework
{
    public class DataStreamReader
    {
        //Don't Edit it (高能预警,不要编辑)
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
        
        private byte[] m_Data;
        private string[] m_SchemeNames;
        public FieldType[] fieldTypes;
        
        private int m_AllLen;
        private int m_Col;
        private int m_Row;
        private int m_SchemeLen;
        private int m_FieldTypeRawOff; //字段类型开始的offse, 从文件头开始算0
        private int m_FieldNameRawOff; //字段名开始的offset, 从文件头开始算0
        private int m_Cur;             //当前字节偏移量
        private int m_CurFieldCol = 0;          // 读到了第几列
        
        private const int SHEME_ROW_COUNT = 4;
        private const int FIXED_HEADER_LEN = 28;
        private const int FILE_TAG_LEN = 4;
        private const byte INT0TYPE = 0;
        private const byte INT1TYPE = 1;
        private const byte INT2TYPE = 2;
        private const byte INT3TYPE = 3;
        private const byte INT4TYPE = 4;
        private const byte FLOATTYPE = 5;
        private const byte STRINGTYPE = 6;
        
        public DataStreamReader(byte[] data)
        {
            m_Data = data;
            ReadHead();
            ReadScheme();
            PreReadFieldType();
        }
        
        private void ReadHead()
        {
            m_Cur += FILE_TAG_LEN;
            m_AllLen = ReadInt4();
            m_SchemeLen = ReadInt4();
            m_Col = ReadInt4();
            m_Row = ReadInt4();
            m_FieldTypeRawOff = ReadInt4();
            m_FieldNameRawOff = ReadInt4();
        }
        
        
        
        private void SkipScheme()
        {
            m_Cur = (FIXED_HEADER_LEN + m_SchemeLen);
            
        }
        
        public void DebugConsole()
        {
            SkipScheme();
            int fileSize = m_Data.Length;
            while (m_Cur < fileSize)
            {
                int typeVal = m_Data[m_Cur] & 0xf;
                if (typeVal <= 4)
                {
                    System.Console.WriteLine(ReadInt());
                }
                else if (typeVal == FLOATTYPE)
                {
                    System.Console.WriteLine(ReadFloat());
                }
                else if(typeVal == STRINGTYPE)
                {
                    System.Console.WriteLine(ReadString());
                }
            }
        }
        
        private void ReadScheme()
        {
            m_Cur = m_FieldNameRawOff;
            m_SchemeNames = new string[m_Col];
            for (int i = 0; i < m_Col; ++i)
            {
                m_SchemeNames[i] = ReadString();
            }
            // 读完Scheme 头信息
            m_Cur = (FIXED_HEADER_LEN + m_SchemeLen);
            m_CurFieldCol = 0;
        }
        
        //预读一行 获取type类型
        private void PreReadFieldType()
        {
            fieldTypes = new FieldType[m_Col];
            while (true) 
            {
                int col = MoreFieldOnRow ();
                if (col == -1) 
                {
                    break;
                }
                fieldTypes[col] = NextFiledType ();
                SkipField ();
            }
            
            //重置读取游标
            m_Cur = (FIXED_HEADER_LEN + m_SchemeLen);
            m_CurFieldCol = 0;
        }
        
        public int GetRowCount()
        {
            return m_Row - SHEME_ROW_COUNT;
        }
        
        public string[] GetSchemeName()
        {
            return m_SchemeNames;
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
                if ( !fieldSourceMap.ContainsKey(key))
                {
                    Log.iWarning(string.Format("Found a new Field :{0} In {1}",
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
                    Log.iError(string.Format("Field :{0}  removed In {1}",
                                                 key, tableName));
                }
            }
        }
        
        /// <summary>
        /// 返回-1 则表示读到行末尾
        /// </summary>
        /// <returns></returns>
        public int MoreFieldOnRow()
        {
            if (m_CurFieldCol >= m_Col)
            {
                m_CurFieldCol = 0;
                return -1;
            }
            else
            {
                return m_CurFieldCol;
            }
        }
        
        public FieldType NextFiledType()
        {
            int typeVal = m_Data[m_Cur] & 0xf;
            
            if (typeVal == INT0TYPE ||typeVal == INT1TYPE || typeVal == INT2TYPE
                || typeVal == INT3TYPE || typeVal == INT4TYPE)
            {
                return FieldType.Int;
            }
            else if (typeVal == FLOATTYPE)
            {
                return FieldType.Float;
            }
            else
            {
                return FieldType.String;
            }
            
        }
        
        public void SkipField()
        {
            int typeVal = m_Data[m_Cur] & 0xf;
            int sLenType = (m_Data[m_Cur] & 0xf0) >> 4;
            m_Cur += 1;
            
            if (typeVal == INT1TYPE)
            {
                m_Cur += 1;
            }
            else if (typeVal == INT2TYPE)
            {
                m_Cur += 2;
            }
            else if (typeVal == INT3TYPE)
            {
                m_Cur += 3;
            }
            else if (typeVal == INT4TYPE)
            {
                m_Cur += 4;
            }
            else if (typeVal == FLOATTYPE)
            {
                m_Cur += 4;
            }
            else if (typeVal == STRINGTYPE)
            {
                
                int sbLen = 0;
                if (sLenType == INT1TYPE)
                {
                    sbLen = ReadInt1();
                }
                else if (sLenType == INT2TYPE)
                {
                    sbLen = ReadInt2();
                }
                else if (sLenType == INT3TYPE)
                {
                    sbLen = ReadInt3();
                }
                else if (sLenType == INT4TYPE)
                {
                    sbLen = ReadInt4();
                }
                m_Cur += sbLen;
            }
            m_CurFieldCol += 1;
        }
        
        private int ReadInt1()
        {
            SByte ret = (SByte)m_Data[m_Cur];
            m_Cur += 1;
            return ret;
            
        }
        
        private short ReadInt2()
        {
            short ret = BitConverter.ToInt16(m_Data, m_Cur);
            m_Cur += 2;
            return IPAddress.NetworkToHostOrder(ret);
            
        }
        
        private Int32 ReadInt3()
        {
            Int32 ret = 0;
            if ((m_Data[m_Cur] & 0x80) == 0)
            {
                ret = 0;
            }
            else
            {
                ret = 0xff << 24;
            }
            ret |= m_Data[m_Cur] << 16;
            ret |= m_Data[m_Cur + 1] << 8;
            ret |= m_Data[m_Cur + 2];
            m_Cur += 3;
            return ret;
            
        }
        
        private Int32 ReadInt4()
        {
            int x1 = BitConverter.ToInt32(m_Data, m_Cur);
            m_Cur += 4;
            return IPAddress.NetworkToHostOrder(x1);
        }
        
        
        public int ReadInt()
        {
            int typeVal = m_Data[m_Cur] & 0xf;
            m_Cur += 1;
            
            int ret = 0;
            if (typeVal == INT0TYPE)
            {
                ret = 0;
            }
            else if (typeVal == INT1TYPE)
            {
                ret = ReadInt1();
            }
            else if (typeVal == INT2TYPE)
            {
                ret = ReadInt2();
                
            }
            else if (typeVal == INT3TYPE)
            {
                ret = ReadInt3();
            }
            else if (typeVal == INT4TYPE)
            {
                ret = ReadInt4();
            }
            m_CurFieldCol += 1;
            return ret;
        }
        
        public bool ReadBool()
        {
            int typeVal = m_Data[m_Cur] & 0xf;
            m_Cur += 1;
            int ret = 0;
            if (typeVal == INT1TYPE)
            {
                ret = ReadInt1();
            }
            m_CurFieldCol += 1;
            return ret > 0;
            
        }

        public Vector3 ReadVector3()
        {
            float x = ReadFloat(false);
            float y = ReadFloat(false);
            float z = ReadFloat(false);
            m_CurFieldCol += 1;
            return new Vector3(x,y,z);
        }
        
        public Vector2 ReadVector2()
        {
            float x = ReadFloat(false);
            float y = ReadFloat(false);
            m_CurFieldCol += 1;
            return new Vector2(x,y);
        }        

        public float ReadFloat(bool isInsFieldIndex = true)
        {
            int typeVal = m_Data[m_Cur] & 0xf;
            m_Cur += 1;
            if (typeVal != FLOATTYPE)
            {
                throw new Exception("Type Mismatch Must be Float");
            }
            else 
            {
                float ret = BitConverter.ToSingle(m_Data, m_Cur);
                m_Cur += 4;
                if (isInsFieldIndex)
                {
                    m_CurFieldCol += 1;
                }

                return ret;
            }
        }
        
        public string ReadString()
        {
            int typeVal = m_Data[m_Cur] & 0xf;
            int sLenType = (m_Data[m_Cur] & 0xf0) >> 4;
            int sbLen = 0;
            m_Cur += 1;
            if (typeVal != STRINGTYPE)
            {
                throw new Exception("Type Mismatch Must be String");
            }
            else 
            {
                string ret = String.Empty;
                if (sLenType == INT0TYPE)
                {
                    sbLen = 0;
                }
                else if (sLenType == INT1TYPE)
                {
                    sbLen = ReadInt1();
                }
                else if (sLenType == INT2TYPE)
                {
                    sbLen = ReadInt2();
                }
                else if (sLenType == INT3TYPE)
                {
                    sbLen = ReadInt3();
                }
                else if (sLenType == INT4TYPE)
                {
                    sbLen = ReadInt4();
                }
                
                ret = System.Text.Encoding.UTF8.GetString(m_Data, m_Cur, sbLen);
                m_Cur += sbLen;
                m_CurFieldCol += 1;
                return ret;
            }
        }
        
    }
}
#endif
