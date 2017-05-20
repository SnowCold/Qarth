using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PTGame.Framework
{
    public class GuideHighlightMask : MonoBehaviour
    {
        [SerializeField]
        private int m_Shape = 1;
        [SerializeField]
        private RectTransform m_Target;
        [SerializeField]
        private Material m_Material;
        [SerializeField]
        private RawImage m_RawImage;
        [SerializeField]
        private float m_Radio;

        public RectTransform target
        {
            get { return m_Target; }
            set { m_Target = value; }
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

            Vector2[] uvPos = RectTransformHelper.GetRectTransformViewPort(m_Target);
            Vector2 center = new Vector2((uvPos[0].x + uvPos[1].x) * 0.5f, (uvPos[0].y + uvPos[1].y) * 0.5f);
            m_Material.SetFloat("_CenterX", center.x);
            m_Material.SetFloat("_CenterY", center.y);

            m_Material.SetFloat("_Width", (uvPos[1].x - uvPos[0].x) * 0.5f);
            m_Material.SetFloat("_Height", (uvPos[1].y - uvPos[0].y) * 0.5f);
            m_Material.SetFloat("_Radio", m_Radio);
            m_Material.SetInt("_Shape", m_Shape);
        }
    }
}
