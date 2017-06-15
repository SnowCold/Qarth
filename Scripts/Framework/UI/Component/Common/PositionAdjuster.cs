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
using DG.Tweening;

namespace Qarth
{
    public class PositionAdjuster : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_Root;
        [SerializeField]
        private Vector2 m_Offset;
        [SerializeField]
        private float m_AnimTime = 0.2f;
        [SerializeField]
        private Ease m_AnimEase = Ease.OutQuint;

        private Rect m_ParentRect;
        private Vector2 m_CenterVec = new Vector2(0.5f, 0.5f);

        void Awake()
        {
            m_ParentRect = ((RectTransform)transform).rect;
        }

        public void AdjustPosition()
        {
            m_Root.DOKill();
            m_Root.localScale = Vector3.one;
            m_Root.pivot = m_CenterVec;

            Vector3 viewPos = UIMgr.S.uiRoot.uiCamera.ScreenToViewportPoint(Input.mousePosition);

            int prefectX = 0;
            int prefectY = 0;
            if (viewPos.x > 0.5f)
            {
                prefectX = 1;
            }

            if (viewPos.y > 0.5f)
            {
                prefectY = 1;
            }

            Vector3 worldPos = UIMgr.S.uiRoot.uiCamera.ScreenToWorldPoint(Input.mousePosition);
            m_Root.position = worldPos;

            Vector3 localPos = m_Root.localPosition;

            localPos.z = 0f;

            localPos.x += m_Offset.x;
            localPos.y += m_Offset.y;

            m_Root.localPosition = localPos;

            Vector2 pivot = new Vector2(prefectX, prefectY);
            Vector2 minPos = m_Root.offsetMin;

            float halfWidth = m_Root.rect.width * 0.5f;
            float halfHeight = m_Root.rect.height * 0.5f;

            minPos.x -= halfWidth;
            minPos.y -= halfHeight;

            if (minPos.x < m_ParentRect.xMin)
            {
                pivot.x = 0;
            }

            if (minPos.y < m_ParentRect.yMin)
            {
                pivot.y = 0;
            }

            m_Root.pivot = pivot;
            m_Root.localPosition = localPos;

            if (m_AnimTime > 0)
            {
                m_Root.localScale = Vector3.zero;
                Tweener tw = m_Root.DOScale(1.0f, m_AnimTime);
                tw.SetEase(m_AnimEase);
            }
        }

    }
}
