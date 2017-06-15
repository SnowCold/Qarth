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

namespace Qarth
{
    public class UIRoot : MonoBehaviour
    {
        public const int FLOAT_PANEL_INDEX = 20000001;
        public const int GM_PANEL_INDEX = 20000000;
        public const int TOP_PANEL_INDEX = 10000000;

        [SerializeField]
        private Transform m_PanelRoot;
        [SerializeField]
        private Transform m_HideRoot;
        [SerializeField]
        private Camera m_UICamera;
        [SerializeField]
        private Canvas m_RootCanvas;

        private int m_AutoPanelOrder = 10;
        private int m_TopPanelOrder = TOP_PANEL_INDEX;

        private Vector3 m_NextFreePos = new Vector3(5000, 5000, 0);
        public int RequireNextPanelSortingOrder(PanelType panelType)
        {
            switch (panelType)
            {
                case PanelType.Auto:
                    m_AutoPanelOrder += 10;
                    return m_AutoPanelOrder;
                case PanelType.Top:
                    m_TopPanelOrder += 10;
                    return m_TopPanelOrder;
                case PanelType.Bottom:
                    return 0;
                default:
                    break;
            }

            return m_AutoPanelOrder;
        }

        public Camera uiCamera
        {
            get { return m_UICamera; }
        }

        public Canvas rootCanvas
        {
            get { return m_RootCanvas; }
        }

        public Vector3 RequireNextFreePos()
        {
            m_NextFreePos.y += 1000;
            return m_NextFreePos;
        }

        public void ReleaseFreePos(Vector3 pos)
        {
            if (m_NextFreePos.y == pos.y)
            {
                m_NextFreePos.y -= 1000;
            }
        }

        public void ReleasePanelSortingOrder(int sortingIndex)
        {
            if (m_AutoPanelOrder == sortingIndex)
            {
                m_AutoPanelOrder -= 10;
            }
            else if (m_TopPanelOrder == sortingIndex)
            {
                m_TopPanelOrder -= 10;
            }
        }

        public Transform panelRoot
        {
            get { return m_PanelRoot; }
        }

        public Transform hideRoot
        {
            get { return m_HideRoot; }
        }
    }
}
