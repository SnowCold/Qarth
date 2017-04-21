using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PTGame.Framework
{
    [TMonoSingletonAttribute("[Tools]/ResolttionHelper")]
    public class ResolutionHelper : TMonoSingleton<ResolutionHelper>
    {
        [SerializeField]
        private float m_StandardCanvasScale;
        [SerializeField]
        private float m_CurrentCanvasScale;
        [SerializeField]
        private bool m_IsInStandardMode = true;

        public override void OnSingletonInit()
        {
            Canvas uiCanvas = UIMgr.S.uiRoot.rootCanvas;
            Camera uiCamera = UIMgr.S.uiRoot.uiCamera;
            CanvasScaler canvasScaler = uiCanvas.GetComponent<CanvasScaler>();
            m_StandardCanvasScale = uiCamera.orthographicSize / canvasScaler.referenceResolution.y * 2;

            m_CurrentCanvasScale = uiCanvas.transform.localScale.y;

            float offset = m_StandardCanvasScale - m_CurrentCanvasScale;
            if (offset < 0.0001 && offset > -0.0001)
            {
                m_IsInStandardMode = true;
            }
            else
            {
                m_IsInStandardMode = false;
            }
        }

        public Vector3 TranslateScale(Vector3 standardScale)
        {
            if (m_IsInStandardMode)
            {
                return standardScale;
            }

            standardScale.y = m_StandardCanvasScale * standardScale.y / m_CurrentCanvasScale;

            return standardScale;
        }
    }
}
