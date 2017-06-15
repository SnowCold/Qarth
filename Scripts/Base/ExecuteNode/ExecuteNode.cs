//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System.Collections;

namespace Qarth
{
    //�����Ȼص���ִ�нڵ�
	public class ExecuteNode
	{
        protected string        m_Tips = "Default";
        private float           m_Progress;
        private bool            m_IsFinish = false;

        public virtual float progress
        {
            get { return m_Progress; }
            set { m_Progress = value; }
        }

        public virtual string tips
        {
            get { return m_Tips; }
            set { m_Tips = value; }
        }

        public bool isFinish
        {
            get { return m_IsFinish; }
            protected set { m_IsFinish = value; }
        }

        public virtual void OnBegin() { }
        public virtual void OnExecute() { }
        public virtual void OnEnd() { }

    }

}
