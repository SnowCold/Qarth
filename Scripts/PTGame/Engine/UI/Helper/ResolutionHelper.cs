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

        public override void OnSingletonInit()
        {
            m_CanvasScaler = UIMgr.S.uiRoot.rootCanvas.GetComponent<CanvasScaler>();
            m_CanvasRoot = m_CanvasScaler.GetComponent<RectTransform>();   
        }

        public Vector3 TranslateScale(Vector3 standardScale)
        {
            float scaleX = standardScale.x * m_CanvasRoot.rect.width / m_CanvasScaler.referenceResolution.x * 2;
            float scaleY = standardScale.y * m_CanvasRoot.rect.height / m_CanvasScaler.referenceResolution.y * 2;

            return new Vector3(scaleX, scaleY, 1);
        }
    }
}
