using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine.UI;
using DG.Tweening;

namespace Qarth
{
    public class ColorFadeTransition : TransitionPanel
    {
        [SerializeField]
        private RawImage m_FadeImage;
        [SerializeField]
        private float m_FadeInDuration = 0.5f;
        [SerializeField]
        private float m_FadeOutDelayTime;
        [SerializeField]
        private float m_FadeOutDuration = 1.0f;

        public override void TransitionIn(AbstractPanel panel)
        {
            var color = m_FadeImage.color;
            color.a = 0;
            m_FadeImage.color = color;
            m_FadeImage.DOFade(1, m_FadeInDuration).SetEase(Ease.Linear)
                .OnComplete(OnFadeInFinish);
        }

        public override void TransitionOut(AbstractPanel panel)
        {
            if (m_FadeOutDelayTime > 0)
            {
                DOTween.Sequence().AppendInterval(m_FadeOutDelayTime)
                    .Append(m_FadeImage.DOFade(0, m_FadeOutDuration).SetEase(Ease.Linear))
                    .OnComplete(OnFadeOutFinish);
            }
            else
            {
                m_FadeImage.DOFade(0, m_FadeOutDuration).SetEase(Ease.Linear)
                    .OnComplete(OnFadeOutFinish);
            }
        }

        private void OnFadeInFinish()
        {
            transitionHandler.OnTransitionInFinish();
        }

        private void OnFadeOutFinish()
        {
            transitionHandler.OnTransitionOutFinish();
        }
    }
}
