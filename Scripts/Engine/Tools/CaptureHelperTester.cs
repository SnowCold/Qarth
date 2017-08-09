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
using UnityEngine.UI;

namespace Qarth
{
    public class CaptureHelperTester : MonoBehaviour
    {
        [SerializeField]
        protected RawImage m_Image;

        protected Texture2D m_Tex;

        private void Awake()
        {
            StartCoroutine(CaptureHelper.Capture(UIMgr.S.uiRoot.uiCamera, OnCaptureFinish));
        }

        private void OnCaptureFinish(Texture2D tex)
        {
            m_Image.texture = tex;
        }

        private void OnDestroy()
        {
            if (m_Tex != null)
            {
                GameObject.Destroy(m_Tex);
            }
        }
    }
}
