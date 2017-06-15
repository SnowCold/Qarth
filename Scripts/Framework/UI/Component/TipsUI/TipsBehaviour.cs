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
using UnityEngine.EventSystems;

namespace Qarth
{
    public class TipsBehaviour : MonoBehaviour
    {
        private List<GraphicRaycaster> m_GraphicRayCasterList;
        private List<Transform> m_WhiteObjectList;//白名单:点击白名单是无处理的
        private static List<RaycastResult> m_Result = new List<RaycastResult>();
        private bool m_HasDown = false;

        protected virtual void Awake()
        {
            GraphicRaycaster[] selfGraphicRayCaster = gameObject.GetComponentsInChildren<GraphicRaycaster>();//gameObject.GetComponent<GraphicRaycaster>();

            AddGraphicRaycaster(selfGraphicRayCaster);
            AddWhiteObject(transform, false);
        }

        public void AddWhiteObject(Transform obj, bool findRaycaster = true)
        {
            if (obj == null)
            {
                return;
            }

            if (m_WhiteObjectList == null)
            {
                m_WhiteObjectList = new List<Transform>();
            }

            if (m_WhiteObjectList.Contains(obj))
            {
                return;
            }

            m_WhiteObjectList.Add(obj);

            if (findRaycaster)
            {
                AddGraphicRaycaster(obj.GetComponentInParent<GraphicRaycaster>());
            }

        }

        private void AddGraphicRaycaster(GraphicRaycaster[] rayArray)
        {
            if (rayArray == null || rayArray.Length == 0)
            {
                return;
            }

            for (int i = rayArray.Length - 1; i >= 0; --i)
            {
                AddGraphicRaycaster(rayArray[i]);
            }
        }

        private void AddGraphicRaycaster(GraphicRaycaster ray)
        {
            if (ray == null)
            {
                return;
            }

            if (m_GraphicRayCasterList == null)
            {
                m_GraphicRayCasterList = new List<GraphicRaycaster>();
            }

            if (m_GraphicRayCasterList.Contains(ray))
            {
                return;
            }

            m_GraphicRayCasterList.Add(ray);
        }

        void Update()
        {
            if (m_GraphicRayCasterList == null)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (CheckIsTouchInEmptyObject())
                {
                    m_HasDown = true;
                }
            }

            if (!m_HasDown)
            {
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_HasDown = false;

                if (CheckIsTouchInEmptyObject())
                {
                    OnClickEmptyArea();
                }

            }
        }

        private bool CheckIsTouchInEmptyObject()
        {
            PointerEventData pd = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            pd.position = Input.mousePosition;
            for (int i = m_GraphicRayCasterList.Count - 1; i >= 0; --i)
            {
                m_Result.Clear();
                m_GraphicRayCasterList[i].Raycast(pd, m_Result);
                if (m_Result.Count == 0)
                {
                    continue;
                }

                if (IsHitWhiteObject(m_Result))
                {
                    return false;
                }
            }

            m_Result.Clear();
            return true;
        }

        private bool IsHitWhiteObject(List<RaycastResult> result)
        {
            if (result == null || result.Count == 0)
            {
                return false;
            }

            if (m_WhiteObjectList == null || m_WhiteObjectList.Count == 0)
            {
                return false;
            }

            for (int i = result.Count - 1; i >= 0; --i)
            {
                GameObject go = result[i].gameObject;
                if (go != null)
                {
                    if (IsHitWhiteObject(go.transform))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsHitWhiteObject(Transform tr)
        {
            for (int i = m_WhiteObjectList.Count - 1; i >= 0; --i)
            {
                if (m_WhiteObjectList[i] == null)
                {
                    m_WhiteObjectList.RemoveAt(i);
                    continue;
                }

                if (tr.IsChildOf(m_WhiteObjectList[i]))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual void OnClickEmptyArea()
        {
            
        }
    }
}
