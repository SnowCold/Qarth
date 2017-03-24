using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class LuaPanel : AbstractPanel
    {
        [SerializeField]
        private string m_LuaSciptName;

        protected override void OnUIInit()
        {
            InitLuaBind();
        }

        protected void InitLuaBind()
        {

        }
    }
}
