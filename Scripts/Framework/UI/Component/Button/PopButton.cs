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
using UnityEngine.EventSystems;

namespace Qarth
{
    public class PopButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private Vector3 m_ClickDownScale = new Vector3(0.95f, 0.95f, 0.95f);
        [SerializeField]
        private Vector3 m_NormalScale = Vector3.one;

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.localScale = m_ClickDownScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.localScale = m_NormalScale;
        }
    }
}
