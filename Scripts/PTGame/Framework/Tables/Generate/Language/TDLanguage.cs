//Auto Generate Don't Edit it
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
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