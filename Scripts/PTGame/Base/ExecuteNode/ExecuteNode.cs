using UnityEngine;
using System.Collections;

namespace PTGame.Framework
{
    //带进度回调的执行节点
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