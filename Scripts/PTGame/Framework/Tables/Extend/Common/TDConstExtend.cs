using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{

    public partial class TDConst
    {
        public void Reset()
        {
            m_IntVal = Helper.String2Int(m_Value);
            m_FloatVal = Helper.String2Float(m_Value);
        }
    }
}