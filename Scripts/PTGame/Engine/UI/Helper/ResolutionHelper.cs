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
        private CanvasScaler m_CanvasScaler;
        [SerializeField]
        private RectTransform m_CanvasRoot;

        private float m_StandardCanvasScale;
        
        public override void OnSingletonInit()
        {
            Camera uiCamera = UIMgr.S.uiRoot.uiCamera;
            Canvas uiCanvas = UIMgr.S.uiRoot.rootCanvas;
            m_CanvasScaler = UIMgr.S.uiRoot.rootCanvas.GetComponent<CanvasScaler>();
            m_StandardCanvasScale = uiCamera.orthographicSize / m_CanvasScaler.referenceResolution.y * 2;

            m_CanvasRoot = m_CanvasScaler.GetComponent<RectTransform>();   
        }

        public Vector3 TranslateScale(Vector3 standardScale)
        {
            //面板的缩放比例
            float nX = m_CanvasRoot.rect.width / m_CanvasScaler.referenceResolution.x;
            float nY = m_CanvasRoot.rect.height / m_CanvasScaler.referenceResolution.y;

            float scaleX = standardScale.x * (nX * m_StandardCanvasScale) / m_CanvasRoot.localScale.x;
            float scaleY = standardScale.y * (nY * m_StandardCanvasScale) / m_CanvasRoot.localScale.x;
            //float scaleX = standardScale.x * m_CanvasRoot.rect.width * m_CanvasRoot.localScale.x / (m_CanvasScaler.referenceResolution.x * m_StandardCanvasScale);
            //float scaleY = standardScale.y * m_CanvasRoot.rect.height * m_CanvasRoot.localScale.y / (m_CanvasScaler.referenceResolution.y * m_StandardCanvasScale);

            //float scaleX = m_StandardCanvasScale / m_CanvasRoot.localScale.x;
            //float scaleY = m_StandardCanvasScale / m_CanvasRoot.localScale.y;

            return new Vector3(scaleX, scaleY, 1);
        }
    }
}
