//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine.UI;
using DG.Tweening;

namespace Qarth
{
    public class StarAnim : MonoBehaviour
    {
        [SerializeField]
        private Image[] m_Stars;
        [SerializeField]
        private bool m_IsAutoPlay = true;


        private void Awake()
        {
            if (m_Stars.Length == 0)
            {
                m_Stars = transform.GetComponentsInChildren<Image>();
            }

            if (m_IsAutoPlay)
            {
                for (int i = 0; i < m_Stars.Length; ++i)
                {
                    DOTween.Sequence().Append(m_Stars[i].DOFade(0, RandomHelper.Range(1.0f, 2.0f)))
                        .Append(m_Stars[i].DOFade(1, RandomHelper.Range(0.5f, 2.0f)))
                        .SetDelay(RandomHelper.Range(1.0f, 4.0f))
                        .SetLoops(-1);
                }
            }
        }

        public void ShowOnce()
        {
            for (int i = 0; i < m_Stars.Length; ++i)
            {
                m_Stars[i].DOKill();
                DOTween.Sequence().Append(m_Stars[i].DOFade(1, RandomHelper.Range(0.2f, 0.5f)))
                    .SetDelay(1)
                    .Append(m_Stars[i].DOFade(0, RandomHelper.Range(0.5f, 1.0f)));
            }
        }

        public void ShowLoop()
        {
            StartCoroutine(ShowLoopInternal());
        }

        private IEnumerator ShowLoopInternal()
        {
            while (true)
            {
                for (int i = 0; i < m_Stars.Length; ++i)
                {
                    m_Stars[i].DOKill();
                    DOTween.Sequence().Append(m_Stars[i].DOFade(1, RandomHelper.Range(0.2f, 0.3f)))
                        .SetDelay(0.8f)
                        .Append(m_Stars[i].DOFade(0, RandomHelper.Range(0.5f, 0.6f)));
                }

                yield return new WaitForSeconds(1.8f);
            }
        }
    }
}
