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

namespace Qarth
{
    public enum SkillState
    {
        kUnInit = 0,
        kReleased = 1,
        kRunning = 2,
        kRemove = 4,//标记需要删除
    }

    public class SkillInfo : ICacheAble, ICacheType
    {
        private static int  s_NextSkillID = 0;
        private int         m_SkillID = -1;
        private SkillState  m_SkillState = SkillState.kUnInit;
        private ISkill      m_Skill;
        private bool        m_CacheFlag;

        public static SkillInfo Allocate()
        {
            SkillInfo info = ObjectPool<SkillInfo>.S.Allocate();
            info.m_SkillID = ++s_NextSkillID;
            return info;
        }

        public static void ResetSkillID()
        {
            s_NextSkillID = 0;
        }

        public int skillID
        {
            get { return m_SkillID; }
        }

        public ISkill skill
        {
            get { return m_Skill; }
            set { m_Skill = value; }
        }

        public SkillState skillState
        {
            get { return m_SkillState; }
            set { m_SkillState = value; }
        }

        public bool cacheFlag
        {
            get
            {
                return m_CacheFlag;
            }

            set
            {
                m_CacheFlag = value;
            }
        }

        public void OnCacheReset()
        {
            m_SkillID = -1;
            m_SkillState = SkillState.kUnInit;
        }

        public void Recycle2Cache()
        {
            ObjectPool<SkillInfo>.S.Recycle(this);
        }
    }
}
