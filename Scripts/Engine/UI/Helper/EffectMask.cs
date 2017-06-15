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
    public class EffectMask : MonoBehaviour
    {
        [SerializeField]
        private float minX;
        [SerializeField]
        private float maxX;
        [SerializeField]
        private float minY;
        [SerializeField]
        private float maxY;
        protected void Update()
        {
            int width = Screen.width;
            int height = Screen.height;
            int designWidth = 1136;//开发时分辨率宽
            int designHeight = 640;//开发时分辨率高
            float s1 = (float)designWidth / (float)designHeight;
            float s2 = (float)width / (float)height;

            //目标分辨率小于 960X640的 需要计算缩放比例
            float contentScale = 1f;
            if (s1 > s2)
            {
                //contentScale = s1 / s2;
            }

            Camera uiCamera = UIMgr.S.uiRoot.uiCamera;
            Canvas canvas = UIMgr.S.uiRoot.rootCanvas;

            Vector2 pos;
            Vector2 center = RectTransformUtility.WorldToScreenPoint(uiCamera, transform.position);
            //Vector3 viewPoint = uiCamera.ScreenToViewportPoint(center);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, center, uiCamera, out pos))
            {
                
                RectTransform rectTransform = transform as RectTransform;

                minX = rectTransform.rect.x + pos.x;
                minY = rectTransform.rect.y + pos.y;
                maxX = minX + rectTransform.rect.width;
                maxY = minY + rectTransform.rect.height;

                ParticleSystem[] particlesSystems = transform.GetComponentsInChildren<ParticleSystem>();

                foreach (ParticleSystem particleSystem in particlesSystems)
                {
                    particleSystem.GetComponent<Renderer>().material.SetFloat("_MinX", minX / 100 / contentScale);
                    particleSystem.GetComponent<Renderer>().material.SetFloat("_MinY", minY / 100 / contentScale);
                    particleSystem.GetComponent<Renderer>().material.SetFloat("_MaxX", maxX / 100 / contentScale);
                    particleSystem.GetComponent<Renderer>().material.SetFloat("_MaxY", maxY / 100 / contentScale);
                }
            }
        }
    }
}
