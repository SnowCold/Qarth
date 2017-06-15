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
      
        private EInt m_Id = 0;
        private string m_Key;
        private string m_Value;

        /// <summary>
        /// 序号
        /// </summary>
        public int id {get { return m_Id; }}
      
        /// <summary>
        /// 键值
        /// </summary>
        public string key {get { return m_Key; }}
      
        /// <summary>
        /// 数值
        /// </summary>
        public string value {get { return m_Value; }}

        public void ReadRow(DataStreamReader dataR, int[] filedIndex)
        {
          int col = 0;
          while(true)
          {
            col = dataR.MoreFieldOnRow();
            if (col == -1)
            {
              break;
            }
            switch (filedIndex[col])
            { 
                  case 0:
                  m_Id = dataR.ReadInt();

                  break;
                  case 1:
                  m_Key = dataR.ReadString();

                  break;
                  case 2:
                  m_Value = dataR.ReadString();

                  break;
                default:
                  break;
            }
          }

        }
        
        public DataStreamReader.FieldType GetFieldTypeInNew(string fieldName)
        {
            return DataStreamReader.FieldType.Unkown;
        }
        
        public static Dictionary<string, int> GetFieldHeadIndex()
        {
          Dictionary<string, int> ret = new Dictionary<string, int>(3);
          
          ret.Add("Id", 0);
          ret.Add("Key", 1);
          ret.Add("Value", 2);
          return ret;
        }
        
        
    }
}//namespace LR
