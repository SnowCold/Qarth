using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public enum SkillState
    {
        kUnInit = 0,
        kReleased = 1,
        kRunning = 2,
        kRemove = 4,//标记需要删除
    }

    public class SkillInfo
    {
        private int         m_SkillID = -1;
        private SkillState m_SkillState = SkillState.kUnInit;

        public int skillID
        {
            get { return m_SkillID; }
            set { m_SkillID = value; }
        }

        public SkillState skillState
        {
            get { return m_SkillState; }
            set { m_SkillState = value; }
        }

        public void Reset()
        {
            m_SkillID = -1;
            m_SkillState = SkillState.kUnInit;
        }
    }
}
