using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qarth;

namespace Qarth
{
    public class TweenBehaviour : MonoBehaviour
    {
        [SerializeField]
        private bool m_DebugFlag = false;

        public virtual void StartAnim(float delayTime = 0)
        {

        }

        public virtual void StopAnim()
        {

        }

        private void OnValidate()
        {
            if (m_DebugFlag)
            {
                StartAnim(0);
                m_DebugFlag = false;
            }
        }
    }
}
