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
    public partial class TDLanguage
    {
        private string m_Id;
        private string m_Value;
        private static int m_Index;

        // 主键
        public string id { get { return m_Id; } }

        // 当前值
        public string value
        {
            get
            {
                return m_Value;
            }
        }

        public void ReadRow(DataStreamReader dataR, int[] filedIndex)
        {
            dataR.MoreFieldOnRow();
            m_Id = dataR.ReadString();
            if (dataR.MoreFieldOnRow() != -1)
            {
                m_Value = dataR.ReadString().Replace("\\n", "\n");
            }
            else
            {
                m_Value = string.Empty;
            }


        }
    }
}//namespace LR
