using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PTGame.Framework
{
    public class PopButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private Vector3 m_ClickDownScale = new Vector3(0.95f, 0.95f, 0.95f);
        [SerializeField]
        private Vector3 m_NormalScale = Vector3.one;

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.localScale = m_ClickDownScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.localScale = m_NormalScale;
        }
    }
}
