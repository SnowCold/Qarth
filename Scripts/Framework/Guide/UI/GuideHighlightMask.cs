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
    public class GuideHighlightMask : MonoBehaviour
    {
        public enum Shape
        {
            NONE = 0,
            Square = 1,
            Circle = 2,
            CircleSquare = 3,
        }

        [SerializeField]
        private Shape m_Shape = Shape.Square;
        [SerializeField]
        private RectTransform m_Target;
        [SerializeField]
        private Material m_Material;
        [SerializeField]
        private RawImage m_RawImage;
        [SerializeField]
        private float m_Radio;
        [SerializeField]
        private float m_RadiumRadio = 1.0f;
        [SerializeField]
		protected bool m_IsDirty = true;

        private Vector4 m_Edge;
        private bool    m_NeedCheckEdge;

        public Shape shape
        {
            get { return m_Shape; }
            set { m_Shape = value; }
        }

        public float radiumRadio
        {
            get { return m_RadiumRadio; }
            set { m_RadiumRadio = value; }
        }

        public RectTransform target
        {
            get { return m_Target; }
            set
            {
                m_Target = value;
                m_IsDirty = true;
            }
        }

        private void Awake()
        {
            m_Radio = 1.0f / UIMgr.S.uiRoot.uiCamera.aspect;

            if (m_RawImage == null)
            {
                m_RawImage = GetComponent<RawImage>();
            }

            if (m_RawImage != null)
            {
                m_RawImage.material = m_Material;
                m_RawImage.color = Color.black;
            }
        }

        private void Update()
        {
            if (m_Target == null || m_Material == null)
            {
                return;
            }

            if (m_IsDirty)
            {
                ScrollRect scr = m_Target.GetComponentInParent<ScrollRect>();

                if (scr == null)
                {
                    m_NeedCheckEdge = false;
                }
                else
                {
                    Vector2[] edge = RectTransformHelper.GetRectTransformViewPort(scr.GetComponent<RectTransform>());
                    m_Edge.Set(edge[0].x, edge[0].y, edge[1].x, edge[1].y);

                    m_NeedCheckEdge = true;
                }

                m_IsDirty = false;
            }

            Vector2[] uvPos = RectTransformHelper.GetRectTransformViewPort(m_Target);
            Vector2 center = new Vector2((uvPos[0].x + uvPos[1].x) * 0.5f, (uvPos[0].y + uvPos[1].y) * 0.5f);
            m_Material.SetFloat("_CenterX", center.x);
            m_Material.SetFloat("_CenterY", center.y);

            float w = (uvPos[1].x - uvPos[0].x) * 0.49f;
            float h = (uvPos[1].y - uvPos[0].y) * 0.49f;
            m_Material.SetFloat("_Width", w);
            m_Material.SetFloat("_Height", h);
            m_Material.SetFloat("_Radio", m_Radio);
            m_Material.SetInt("_Shape", (int)m_Shape);

            if (m_Shape == Shape.CircleSquare)
            {
                float r = Mathf.Min(w, h);
                m_Material.SetFloat("_Radium", r * m_RadiumRadio);
            }

            if (m_NeedCheckEdge)
            {
                m_Material.SetFloat("_CheckEdge", 1);
                m_Material.SetVector("_Edge", m_Edge);
            }
            else
            {
                m_Material.SetFloat("_CheckEdge", -1);
            }
        }
	
	}
}
