using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class KeyCodeEventInfo
    {
        private bool m_IsProcess = false;

        public void Process()
        {
            m_IsProcess = true;
        }

        public bool IsProcess()
        {
            return m_IsProcess;
        }

        public void Reset()
        {
            m_IsProcess = false;
        }

    }
}
