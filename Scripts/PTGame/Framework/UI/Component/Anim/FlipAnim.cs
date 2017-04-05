using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace PTGame.Framework
{
    public class FlipAnim : MonoBehaviour
    {
        [SerializeField]
        private Transform m_FrontPage;
        [SerializeField]
        private Transform m_BackPage;

        private void Awake()
        {
            m_FrontPage.localScale = new Vector3(1, 1, 1);
            m_BackPage.localScale = new Vector3(0, 1, 1);
            m_FrontPage.DOScaleX(0, 0.31f);
            m_BackPage.DOScaleX(1, 1).SetDelay(0.3f);
        }


    }
}
