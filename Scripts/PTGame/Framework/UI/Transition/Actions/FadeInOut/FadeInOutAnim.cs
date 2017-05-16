using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PTGame.Framework;
using DG.Tweening;

namespace PTGame.Framework
{
    public class FadeInOutAction : ITransitionAction
    {
        private ITransitionHandler m_Handler;
        private ResLoader m_ResLoader;

        public ITransitionHandler transitionHandler
        {
            get
            {
                return m_Handler;
            }

            set
            {
                m_Handler = value;
            }
        }

        public bool transitionSameTime
        {
            get
            {
                return false;
            }
        }


        private void OnFadeInFinish()
        {
            m_Handler.OnTransitionInFinish();
        }

        private void OnFadeOutFinish()
        {
            m_Handler.OnTransitionOutFinish();
        }

        public void PrepareTransition()
        {
            if (m_ResLoader == null)
            {
                m_ResLoader = ResLoader.Allocate("FadeInOutAnim");
            }
            else
            {
                m_ResLoader.ReleaseAllRes();
            }

            m_ResLoader.Add2Load("SwitchAnimShaderAnim", OnResLoadFinish);

            m_ResLoader.LoadAsync();
        }

        private SwitchAnimShaderEffect m_Effect;

        private void OnResLoadFinish(bool result, IRes res)
        {
            GameObject prefab = res.asset as GameObject;
            GameObject node = GameObject.Instantiate(prefab, m_Handler.transitionPanel.transform, false);
            UIHelper.AttachUI(node, m_Handler.transitionPanel.transform);

            m_Effect = node.GetComponent<SwitchAnimShaderEffect>();

            m_Handler.OnTransitionPrepareFinish();
        }

        public void TransitionIn(AbstractPanel panel)
        {
            m_Effect.FadeIn(OnFadeInFinish);
        }

        public void TransitionOut(AbstractPanel panel)
        {
            m_Effect.FadeOut(OnFadeOutFinish);
        }

        public void OnTransitionDestroy()
        {
            if (m_ResLoader != null)
            {
                m_ResLoader.Recycle2Cache();
                m_ResLoader = null;
            }

        }

        public void OnNextPanelReady()
        {
            
        }
    }
}
