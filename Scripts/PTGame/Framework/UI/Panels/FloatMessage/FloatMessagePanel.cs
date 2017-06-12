//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;


namespace SCEngine
{
    public class FloatMsg
    {
        private string m_Message;

        public string message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }
    }

    public class FloatMessagePanel : AbstractPanel
    {
        [SerializeField]
        private GameObject m_Prefab;
        [SerializeField]
        private Transform m_Root;
        [SerializeField]
        private Vector3 m_StartPos;
        [SerializeField]
        private Vector3 m_EndPos;
        [SerializeField]
        private float m_AnimTime = 1.0f;
        [SerializeField]
        private float m_SendOffsetTime = 0.1f;

        private Stack<FloatMsg> m_MsgList;
        private GameObjectPool m_GameObjectPool;
        private float m_LastSendTime;

        public override int sortIndex
        {
            get
            {
                return UIRoot.FLOAT_PANEL_INDEX;
            }
        }

        protected override void OnUIInit()
        {
            m_MsgList = new Stack<FloatMsg>();
            m_GameObjectPool = GameObjectPoolMgr.S.CreatePool("FloatMessagePool", m_Prefab, -1, 5, UIPoolStrategy.S);
            m_Prefab.SetActive(false);
        }

        protected override void OnPanelOpen(params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return;
            }

            string msg = args[0] as string;

            if (msg == null)
            {
                return;
            }

            PlayFloatMessage(msg);
        }

        public void ShowMsg(string msg)
        {
            if (msg == null)
            {
                return;
            }

            PlayFloatMessage(msg);
        }

        protected override void BeforDestroy()
        {
            if (m_GameObjectPool != null)
            {
                m_GameObjectPool.RemoveAllObject(true, false);
                m_GameObjectPool = null;
            }
        }

        public void PlayFloatMessage(string msg)
        {
            PlayFloatMessage(msg, Vector3.zero, Vector3.zero);
        }

        public void PlayFloatMessage(string msg, Vector3 from, Vector3 to)
        {
            if (UIMgr.isApplicationQuit)
            {
                return;
            }

            FloatMsg fm = new FloatMsg();
            fm.message = msg;
            ShowMsg(fm);
        }

        private bool CheckIsShowAble()
        {
            if (Time.realtimeSinceStartup - m_LastSendTime > m_SendOffsetTime)
            {
                return true;
            }

            return false;
        }

        private void ShowMsg(FloatMsg msgVo, bool check = true)
        {
            if (check)
            {
                if (!CheckIsShowAble())
                {
                    m_MsgList.Push(msgVo);
                    return;
                }
            }

            GameObject obj = m_GameObjectPool.Allocate();
            if (obj)
            {
                FloatMessageItem item = obj.GetComponent<FloatMessageItem>();

                item.SetFloatMsg(msgVo);

                obj.transform.SetParent(m_Root, true);

                obj.transform.localPosition = m_StartPos;

                Tweener tweener = obj.transform.DOLocalMove(m_EndPos, m_AnimTime);
                tweener.SetEase(Ease.Linear);
                tweener.OnComplete<Tweener>(() => 
                {
                    m_GameObjectPool.Recycle(obj);
                });

                m_LastSendTime = Time.realtimeSinceStartup;
            }
        }

        private void Update()
        {
            if (m_MsgList.Count == 0)
            {
                return;
            }

            if (CheckIsShowAble())
            {
                ShowMsg(m_MsgList.Pop(), false);
            }
        }
    }
}
