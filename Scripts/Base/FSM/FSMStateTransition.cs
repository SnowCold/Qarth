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
    //	Map-<key, state>:key = sourceCode << 16 + targetCode
    //		Transition
    [System.Serializable]
    public class FSMStateTransition<T>
    {
        [SerializeField]
        protected string m_SourceStateName;
        [SerializeField]
        protected string m_TargetStateName;

        public FSMStateTransition(string sourceStateName, string targetStateName)
        {
            m_SourceStateName = sourceStateName;
            m_TargetStateName = targetStateName;
        }

        public string sourceStateName
        {
            get { return m_SourceStateName; }
        }

        public string targetStateName
        {
            get { return m_TargetStateName; }
        }
    }

}
