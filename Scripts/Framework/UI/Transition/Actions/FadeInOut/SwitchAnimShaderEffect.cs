//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;

namespace Qarth
{
    public class SwitchAnimShaderEffect : MonoBehaviour
    {
        [SerializeField]
        private RawImage m_Image;
        [SerializeField]
        private Material m_Material;
        [SerializeField]
        private float m_FadeInDuration = 1.0f;
        [SerializeField]
        private float m_FadeOutDuration = 1.0f;

        private float m_Radium = 0.5f;

        private bool m_HasInit = false;

        private void Awake()
        {
            InitEnvironment();

            Update();
        }

        private void InitEnvironment()
        {
            if (m_HasInit)
            {
                return;
            }

            m_HasInit = true;

            if (m_Image == null)
            {
                m_Image = GetComponent<RawImage>();
            }
        }

        public void FadeIn(TweenCallback callBack)
        {
            InitEnvironment();

            m_Radium = 0.46f;

            DOGetter<float> getter = () =>
            {
                return m_Radium;
            };

            DOSetter<float> setter = (x) =>
            {
                m_Radium = x;
            };


            DOTween.To(getter, setter, -0.1f, m_FadeInDuration)
            //.SetEase(Ease.Linear)
            .OnComplete(callBack);
        }

        private void Update()
        {
            if (m_Material != null)
            {
                m_Material.SetFloat("_Radium", m_Radium);
            }

            if (m_Image != null)
            {
                m_Image.material = m_Material;
            }
        }

        public void FadeOut(TweenCallback callBack)
        {
            InitEnvironment();

            m_Radium = -0.1f;

            DOGetter<float> getter = () =>
            {
                return m_Radium;
            };

            DOSetter<float> setter = (x) =>
            {
                m_Radium = x;
            };

            DOTween.To(getter, setter, 0.46f, m_FadeOutDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(callBack);
        }
        
    }
}
