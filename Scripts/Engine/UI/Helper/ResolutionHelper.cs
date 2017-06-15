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

namespace Qarth
{
    public enum ResolutionMode
    {
        Normal,
        KeepRatio,
    }

    [TMonoSingletonAttribute("[Tools]/ResolutionHelper")]
    public class ResolutionHelper : TMonoSingleton<ResolutionHelper>
    {
        [SerializeField]
        private CanvasScaler m_CanvasScaler;
        [SerializeField]
        private RectTransform m_CanvasRoot;
        
        public override void OnSingletonInit()
        {

            m_CanvasScaler = UIMgr.S.uiRoot.rootCanvas.GetComponent<CanvasScaler>();

            m_CanvasRoot = m_CanvasScaler.GetComponent<RectTransform>();   
        }

        public Vector3 TranslateScale(Vector3 standardScale, ResolutionMode mode)
        {
            Vector2 scale = new Vector2();
            scale.x = m_CanvasRoot.rect.width / m_CanvasScaler.referenceResolution.x;
            scale.y = m_CanvasRoot.rect.height / m_CanvasScaler.referenceResolution.y;

            if (mode == ResolutionMode.KeepRatio)
            {
                float scaleValue = Mathf.Min(scale.x, scale.y);
                scale.Set(scaleValue, scaleValue);
            }

            return new Vector3(scale.x * standardScale.x, scale.y * standardScale.y);
        }

        public void AdapterResolution(RectTransform target, ResolutionMode mode)
        {
            Vector2 scale = new Vector2();
            scale.x = m_CanvasRoot.rect.width / m_CanvasScaler.referenceResolution.x;
            scale.y = m_CanvasRoot.rect.height / m_CanvasScaler.referenceResolution.y;

            if (mode == ResolutionMode.KeepRatio)
            {
                float scaleValue = Mathf.Min(scale.x, scale.y);
                scale.Set(scaleValue, scaleValue);
            }
            Vector3 standardScale = target.localScale;

            target.localScale = new Vector3(standardScale.x * scale.x, standardScale.y * scale.y, standardScale.z);

            Vector2 anchoredPos = target.anchoredPosition;
            target.anchoredPosition = new Vector2(anchoredPos.x * scale.x, anchoredPos.y * scale.y);
        }
    }
}
