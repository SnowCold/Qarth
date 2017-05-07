using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PTGame.Framework
{
    public class ResHolder : TSingleton<ResHolder>
    {
        protected ResLoader m_Loader;

        public override void OnSingletonInit()
        {
            m_Loader = ResLoader.Allocate("ResHolder");
        }

        public void AddRes(string res)
        {
            m_Loader.Add2Load(res);
        }

        protected void AddAssistUI2Holder(EngineUI uiid)
        {
            var data = UIDataTable.Get(uiid);
            m_Loader.Add2Load(data.fullPath);
        }
    }
}
