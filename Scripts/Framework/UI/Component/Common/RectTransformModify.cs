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
using Qarth;

namespace Qarth
{
    public class RectTransformModify : MonoBehaviour
    {
        private RectTransform m_RectTransform;

        public void SetLocatingPoints(Vector3[] vec)
        {
            SetLocatingPoints(vec, Vector2.zero, Vector2.zero);
        }

        public void SetLocatingPoints(Vector3[] vec, Vector2 sizeOffset, Vector2 positionOffset)
        {
            if (vec == null || vec.Length < 4)
            {
                return;
            }

            UpdateRectTransform(vec, sizeOffset, positionOffset);
        }

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
            if (m_RectTransform == null)
            {
                m_RectTransform = gameObject.AddComponent<RectTransform>();
            }
        }

        private void UpdateRectTransform(Vector3[] vec, Vector2 sizeOffset, Vector2 posOffset)
        {
            float centerX = (vec[0].x + vec[2].x) * 0.5f + posOffset.x;
            float centerY = (vec[0].y + vec[2].y) * 0.5f + posOffset.y;

            transform.localPosition = new Vector3(centerX, centerY, 0);

            float sizeX = Mathf.Abs((vec[2].x - vec[0].x)) + sizeOffset.x;
            float sizeY = Mathf.Abs((vec[2].y - vec[0].y)) + sizeOffset.y;
            m_RectTransform.sizeDelta = new Vector2(sizeX, sizeY);
        }
    }
}
