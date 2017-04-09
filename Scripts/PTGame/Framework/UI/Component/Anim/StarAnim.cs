using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PTGame.Framework;
using UnityEngine.UI;
using DG.Tweening;

namespace PTGame.Framework
{
    public class StarAnim : MonoBehaviour
    {
        [SerializeField]
        private Image[] m_Stars;

        private void Awake()
        {
            if (m_Stars.Length == 0)
            {
                m_Stars = transform.GetComponentsInChildren<Image>();
            }

            for (int i = 0; i < m_Stars.Length; ++i)
            {
                DOTween.Sequence().Append(m_Stars[i].DOFade(0, RandomHelper.Range(1.0f, 2.0f)))
                    .Append(m_Stars[i].DOFade(1, RandomHelper.Range(0.5f, 2.0f)))
                    .SetDelay(RandomHelper.Range(1.0f, 4.0f))
                    .SetLoops(-1);
            }
        }
    }
}
