using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qarth;

namespace Qarth
{
    public class OpenPanelCommand : AbstractGuideCommand
    {
        private string m_TypeName;
        private string m_EnumName;
        private int m_UIID = -1;

        public override void SetParam(object[] param)
        {
            m_TypeName = param[0] as string;
            m_EnumName = param[1] as string;

            try
            {
                Type enumType = Type.GetType(m_TypeName);
                m_UIID = (int)Enum.Parse(enumType, m_EnumName);
            }
            catch (Exception e)
            {
                Log.e(e);
            }

        }

        protected override void OnStart()
        {
            if (m_UIID >= 0)
            {
                UIMgr.S.OpenPanel(m_UIID);
            }
            FinishStep();
        }
    }
}
