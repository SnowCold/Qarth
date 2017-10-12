using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qarth;

namespace Qarth
{
    public class CameraSizeAdjuster : MonoBehaviour
    {
        public enum MatchMode
        {
            Width,
            Height,
        }

        [SerializeField]
        private Vector2 m_ReferenceResolution;
        [SerializeField]
        private MatchMode m_MatchMode = MatchMode.Height;
        [SerializeField]
        private bool m_NeedFit = true;
        [SerializeField]
        private float m_ReferencePixelsPerUnit = 100;
        [SerializeField]
        private Camera m_Camera;

        private void Awake()
        {
            if (m_Camera == null)
            {
                m_Camera = GetComponent<Camera>();
            }
        }

        private void Update()
        {
            Adjust();
        }

        private void Adjust()
        {
            Vector2 designSize = m_ReferenceResolution / m_ReferencePixelsPerUnit;
            float aspectRatio = Screen.width * 1.0f / Screen.height;

            //尽量撑满Y
            if (m_MatchMode == MatchMode.Height)
            {
                float orthographicSizeHeight = designSize.y * 0.5f;
                if (m_NeedFit)
                {
                    float cameraWidth = orthographicSizeHeight * 2 * aspectRatio;
                    if (cameraWidth < designSize.x)
                    {
                        orthographicSizeHeight = designSize.x / (2 * aspectRatio);
                    }
                }
                m_Camera.orthographicSize = orthographicSizeHeight;
            }
            else
            {
                float orthographicSizeWidth = Screen.width / (m_ReferencePixelsPerUnit * 2 * aspectRatio);
                if (m_NeedFit)
                {
                    float cameraHeight = orthographicSizeWidth * 2;
                    if (cameraHeight < designSize.y)
                    {
                        orthographicSizeWidth = designSize.y / 2;
                    }
                }
                m_Camera.orthographicSize = orthographicSizeWidth;
            }

        }
    }
}
