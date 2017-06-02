using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class ProgressPositionSetter : MonoBehaviour
    {
        [SerializeField, Range(0, 1)]
        private float m_Progress;
        [SerializeField]
        private Transform m_Target;
        [SerializeField]
        private Vector3 m_StartLocalPosition;
        [SerializeField]
        private Vector3 m_EndLocalPosition;

        public float progress
        {
            get { return m_Progress; }
            set
            {
                m_Progress = value;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (m_Target == null)
            {
                return;
            }

            Vector3 dis = m_EndLocalPosition - m_StartLocalPosition;
            dis = dis * m_Progress;
            Vector3 pos = m_StartLocalPosition + dis;
            m_Target.localPosition = pos;
        }
    }
}
