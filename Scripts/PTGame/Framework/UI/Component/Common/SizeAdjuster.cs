using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class SizeAdjuster : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_Target;
        [SerializeField]
        private Vector2 m_Offset;

        private RectTransform m_SelfTransform;

        private void Awake()
        {
            m_SelfTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (m_Target == null)
            {
                return;
            }

            Vector2 size = m_Target.rect.size + m_Offset;
            m_SelfTransform.sizeDelta = size;
        }
    }
}
