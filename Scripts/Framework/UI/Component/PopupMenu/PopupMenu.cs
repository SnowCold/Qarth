//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

namespace Qarth
{
    public class PopupMenu : MonoBehaviour
    {
        enum eState
        {
            kNone,
            kPull,
            kPop,
        }

        [SerializeField]
        private Vector3 m_Direction = Vector3.up;
        [SerializeField]
        private Transform m_ContextRoot;
        [SerializeField]
        private float m_AnimSpeed = 200;//每秒距离
        [SerializeField]
        private Button m_ControllerButton;
        [SerializeField]
        private Image m_DirImage;

        private eState m_CurrentState = eState.kNone;

        private int m_ButtonCount;

        public string controllerButtonText
        {
            set 
            {
                Text text = UIFinder.Find<Text>(m_ControllerButton.transform, "Text");
                text.text = value;
            }
        }

        public void ResetSubButtons()
        {
            m_ButtonCount = 0;
        }

        public Transform AddCustomSubButton(UnityEngine.Events.UnityAction l)
        {
            if (m_ButtonCount >= m_ContextRoot.childCount)
            {
                return null;
            }

            ++m_ButtonCount;

            Transform child = m_ContextRoot.GetChild(m_ContextRoot.childCount - m_ButtonCount);

            Button button = child.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            if (l != null)
            {
                button.onClick.AddListener(l);
            }

            return child;
        }

        public bool AddSubButton(string name, UnityEngine.Events.UnityAction l)
        {
            if (m_ButtonCount >= m_ContextRoot.childCount)
            {
                return false;
            }

            ++m_ButtonCount;

            Transform child = m_ContextRoot.GetChild(m_ContextRoot.childCount - m_ButtonCount);
            Text text = UIFinder.Find<Text>(child, "Text");
            text.text = name;

            Button button = child.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            if (l != null)
            {
                button.onClick.AddListener(l);
            }

            return true;
        }

        protected void Awake()
        {
            m_ControllerButton.onClick.AddListener(OnClickSelf);
            Pull();
        }

        private void OnClickSelf()
        {
            switch (m_CurrentState)
            {
                case eState.kPop:
                    Pull();
                    break;
                case eState.kNone:
                case eState.kPull:
                    Pop();
                    break;
                default:
                    break;
            }
        }

        public void Pull(bool anim = true)
        {
            if (m_ContextRoot == null)
            {
                return;
            }

            m_CurrentState = eState.kPull;

            int childCount = m_ContextRoot.childCount;
            float duration = 0;

            for (int i = childCount - 1; i >= 0; --i)
            {
                int order = childCount - i - 1;

                Transform tr = m_ContextRoot.GetChild(i);

                tr.DOKill();
                if (order >= m_ButtonCount)
                {
                    tr.gameObject.SetActive(false);
                    continue;
                }

                Vector3 curPos = tr.localPosition;

                duration = Mathf.Abs(curPos.magnitude) / m_AnimSpeed;

                Tweener tw = tr.DOLocalMove(Vector3.zero, duration);
                tw.OnComplete(() => 
                {
                    tr.gameObject.SetActive(false);
                });
            }

            RotateDirImage(Vector3.zero, duration);
        }

        private void RotateDirImage(Vector3 angle, float duration)
        {
            if (m_DirImage == null)
            {
                return;
            }

            m_DirImage.transform.DOKill();

            m_DirImage.transform.DOLocalRotate(angle, duration);
        }

        public void Pop(bool anim = true)
        {
            if (m_ContextRoot == null)
            {
                return;
            }

            Vector3 nextPos = new Vector3(m_Direction.x * 5, m_Direction.y * 5, 0);

            m_CurrentState = eState.kPop;

            Vector3 dir = Vector3.zero;

            int childCount = m_ContextRoot.childCount;

            float duration = 0;
            for (int i = childCount - 1; i >= 0; --i)
            {
                int order = childCount - i - 1;
                RectTransform tr = m_ContextRoot.GetChild(i) as RectTransform;

                tr.DOKill();
                if (order >= m_ButtonCount)
                {
                    tr.gameObject.SetActive(false);
                    continue;
                }

                Vector3 curPos = tr.localPosition;
                dir.x = tr.rect.width;
                dir.y = tr.rect.height;

                Vector3 offsetPos = new Vector3(dir.x * m_Direction.x, dir.y * m_Direction.y, 0);

                nextPos += offsetPos;

                Vector3 moveOffset = nextPos - curPos;

                duration = moveOffset.magnitude / m_AnimSpeed;

                tr.DOLocalMove(nextPos, duration);
                tr.gameObject.SetActive(true);
            }

            RotateDirImage(new Vector3(0, 0, 180), duration);
        }
    }
}
