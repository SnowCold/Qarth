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
    #region 技能释放过滤器
    public interface ISkillReleaseFilter
    {
        //过滤器的排序
        int filterSort
        {
            get;
        }

        bool CheckSkillReleaseAble(ISkill skill, ISkillReleaser releaser);
    }
    #endregion

    public class AbstractSkillSystem
    {
        //对技能列表的增删操作优化空间比较大
        private List<ISkillReleaseFilter>    m_SkillFilterList;  //技能释放过滤器列表
        private Dictionary<ISkillReleaseFilter, bool> m_skillFilterMap;
        private List<SkillInfo>             m_SkillInfoList;    //释放技能列表
        private bool                        m_IsPause = false;

        public AbstractSkillSystem()
        {
            m_SkillFilterList = new List<ISkillReleaseFilter>();
            m_skillFilterMap = new Dictionary<ISkillReleaseFilter, bool>();
            m_SkillInfoList = new List<SkillInfo>();
        }

        public bool isPause
        {
            get { return m_IsPause; }
            set { m_IsPause = value; }
        }

        public void RegisterSkillFilter(ISkillReleaseFilter filter)
        {
            if (m_skillFilterMap.ContainsKey(filter))
            {
                return;
            }

            m_skillFilterMap.Add(filter, true);
            m_SkillFilterList.Add(filter);
        }

        public void UnRegisterSkillFilter(ISkillReleaseFilter filter)
        {
            if (!m_skillFilterMap.ContainsKey(filter))
            {
                return;
            }

            m_skillFilterMap.Remove(filter);
            m_SkillFilterList.Remove(filter);
        }

        #region Public Func
        public bool ReleaseSkill(ISkill skill, ISkillReleaser releaser)
        {
            if (skill == null)
            {
                return false;
            }

            if (skill.skillInfo != null)
            {
                Log.w("Skill Already Release.");
                return false;
            }

            for(int i = m_SkillFilterList.Count - 1; i >= 0; --i)
            {
                if(!m_SkillFilterList[i].CheckSkillReleaseAble(skill, releaser))
                {
                    return false;
                }
            }

            SkillInfo info = SkillInfo.Allocate();

            info.skill = skill;
            skill.skillInfo = info;

            m_SkillInfoList.Add(info);

            skill.DoSkillRelease(this, releaser);

            return true;
        }

        public void RemoveSkill(ISkill skill)
        {
            if (skill == null)
            {
                return;
            }

            DoSkillRemove(skill.skillInfo);
        }

        public void RemoveAllSkill()
        {
            for (int i = m_SkillInfoList.Count - 1; i >= 0; --i)
            {
                if (m_SkillInfoList[i].skill != null)
                {
                    ISkill skill = m_SkillInfoList[i].skill;
                    DoSkillRemove(m_SkillInfoList[i]);
                }
            }
        }

        public void RemoveSkillByReleaser(ISkillReleaser releaser)
        {
            if(releaser == null)
            {
                return;
            }

            for(int i = m_SkillInfoList.Count - 1; i >= 0; --i)
            {
                if (m_SkillInfoList[i].skill != null)
                {
                    ISkill skill = m_SkillInfoList[i].skill;
                    if (skill.skillReleaser == releaser)
                    {
                        DoSkillRemove(m_SkillInfoList[i]);
                    }
                }
            }
        }

        public void Update(float time)
        {
            if (m_IsPause)
            {
                return;
            }

            for (int i = m_SkillInfoList.Count - 1; i >= 0; --i)
            {
                SkillInfo info = m_SkillInfoList[i];
                if (info.skillState == SkillState.kRemove || info.skill == null)
                {
                    DoSkillRemove(info);

                    m_SkillInfoList.RemoveAt(i);
                    info.Recycle2Cache();
                    continue;
                }

                info.skill.DoSkillUpdate(time);
            }
        }

        private void DoSkillRemove(SkillInfo info)
        {
            if (info == null)
            {
                return;
            }

            info.skillState = SkillState.kRemove;

            if (info.skill == null)
            {
                return;
            }

            info.skill.DoSkillRemove();
            info.skill.skillInfo = null;
            info.skill = null;
        }

        #endregion
    }
}
