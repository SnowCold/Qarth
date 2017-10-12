using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine.UI;

namespace Qarth
{
    public class SpriteAnim : MonoBehaviour
    {
        [SerializeField]
        private Image m_TargetImage;
        [SerializeField]
        private Sprite[] m_Sprites;
        [SerializeField]
        private float m_Duration = 1.0f;

        private int m_TimerID;
        private int m_CurrentIndex = 0;

        private void Awake()
        {
            if (m_TargetImage == null)
            {
                m_TargetImage = GetComponent<Image>();
            }

            if (m_TargetImage == null || m_Sprites == null || m_Sprites.Length == 0)
            {
                return;
            }

            m_TimerID = Timer.S.Post2Scale(OnTimeReach, m_Duration, -1);
        }

        private void OnTimeReach(int count)
        {
            m_CurrentIndex = (++m_CurrentIndex) % m_Sprites.Length;
            m_TargetImage.sprite = m_Sprites[m_CurrentIndex];
        }

        private void OnDestroy()
        {
            if (m_TimerID > 0)
            {
                Timer.S.Cancel(m_TimerID);
                return;
            }
        }
    }
}
